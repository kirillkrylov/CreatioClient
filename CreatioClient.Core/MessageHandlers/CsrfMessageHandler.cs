using CreatioClient.Core.Models.Domain;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CreatioClient.Core.MessageHandlers
{
    /// <summary>
    /// Adds BPMCSRF authentication cookie value as header for all non GET requests
    /// </summary>
    /// <remarks>
    /// To disable CSRF protection, see
    /// <seealso href="https://academy.creatio.com/docs/developer/integrations_and_api/request_authentication/request_authentication#title-1391-1">Disabling the CSRF protection</seealso>
    /// </remarks>
    public class  CsrfMessageHandler : DelegatingHandler, IDisposable
    {
        private readonly CookieContainer _cookieContainer;
        private readonly IConfiguration _configuration;

        internal CsrfMessageHandler(CookieContainer cookieContainer, IConfiguration configuration)
        {
            _cookieContainer = cookieContainer;
            _configuration = configuration;
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Get requests do not require BPMCSRF
            if(request.Method == HttpMethod.Get)
            {
                return base.SendAsync(request, cancellationToken);
            }

            const string bpmcsrf = "BPMCSRF";
            CookieCollection cookies = _cookieContainer.GetCookies(_configuration.AppUri);
            for (int i = 0; i < cookies.Count; i++)
            {
                if(cookies[i].Name == bpmcsrf)
                {
                    request.Headers.Add(bpmcsrf, cookies[i].Value);
                    break;
                }
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}
