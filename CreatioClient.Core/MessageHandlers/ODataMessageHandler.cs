using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace CreatioClient.Core.MessageHandlers
{
    /// <summary>
    /// Adds ForceUseSession and Accept headers to the request
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>
    ///     See <seealso href="https://academy.creatio.com/docs/developer/integrations_and_api/data_services/odata#title-2304-6">academy</seealso> for <b>ForceUseSession</b> and <b>Content-Type</b> headers
    /// </item>
    /// </list>
    /// </remarks>
    public class ODataMessageHandler : DelegatingHandler, IDisposable
    {    
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpRequestHeaders httpRequestHeaders = request.Headers;
            if (!httpRequestHeaders.Contains("ForceUseSession"))
            {
                request.Headers.Add("ForceUseSession", "true");
            }

            if (!httpRequestHeaders.Contains("Accept"))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json;odata=verbose"));
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
