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
                if (_login.CurrentState == Login.State.LoggedIn)
                {
                    //Send the request
                    return await base.SendAsync(request, cancellationToken);
                }
            }


            var response = await base.SendAsync(request, cancellationToken);
            
            //If request comes back with Unauthorized, RELOGIN
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _login.SetCurrentLogInState(Login.State.LoggedOut);
                await _login.Execute();


                if (_login.CurrentState == Login.State.LoggedIn)
                {
                    // RESEND the original request
                    response = await base.SendAsync(request, cancellationToken);
                }
            }
            return response;                   
        }       
    }
}
