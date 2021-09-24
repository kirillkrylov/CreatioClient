using CreatioClient.Core.Models.Domain;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CreatioClient.Core.MessageHandlers
{
    public class LoginMessageHandler : DelegatingHandler, IDisposable
    {
        private readonly CookieContainer _cookieContainer;
        private readonly IConfiguration _configuration;

        internal LoginMessageHandler(CookieContainer cookieContainer, IConfiguration configuration)
        {
            _cookieContainer = cookieContainer;
            _configuration = configuration;
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken);
        }
    }
}
