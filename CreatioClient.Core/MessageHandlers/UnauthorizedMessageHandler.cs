using CreatioClient.Core.Services;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CreatioClient.Core.MessageHandlers
{
    internal class UnauthorizedMessageHandler : DelegatingHandler, IDisposable
    {
        private readonly ILogin _login;
        internal UnauthorizedMessageHandler(ILogin login)
        {
            _login = login;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //Execute login when State is unknown or LoggedOut
            if (_login.CurrentState == Login.State.Unknown || _login.CurrentState == Login.State.LoggedOut)
            {
                await _login.Execute();
            }

            //Send the request
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            
            
            //If request comes back with Unauthorized, RELOGIN
            if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _login.SetState(Login.State.LoggedOut);
                await _login.Execute();
                
                // RESEND the original request
                return await base.SendAsync(request, cancellationToken);
            }
            return response;
                                    
        }       
    }
}
