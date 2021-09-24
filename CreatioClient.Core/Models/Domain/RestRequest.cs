using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CreatioClient.Core.Models.Domain
{
    /// <summary>
    /// Abstraction over <b>GET</b> Rest Request parameters
    /// </summary>
    internal interface IRestRequest
    {
        
        /// <value>
        /// Timeout
        /// </value>
        TimeSpan TimeOut { get; }
        HttpRequestMessage HttpRequestMessage { get;}
    }

    internal class RestRequest : IRestRequest
    {
        private readonly IConfiguration _configuration;

        #region Constructor
        public RestRequest(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="relativeUrl">Relative url of the remote endpoint</param>
        /// <param name="method">HttpMethod to execute</param>
        /// <param name="requestData">utf8Json payload</param>
        /// <param name="timeout">Request timeout in Milliseconds</param>
        public RestRequest(IConfiguration configuration, string relativeUrl, HttpMethod method, string requestData, int timeout = 10000):this(configuration)
        {
            TimeOut = TimeSpan.FromMilliseconds(timeout);

            HttpRequestMessage = new HttpRequestMessage(method,
                new Uri(_configuration.AppUri, relativeUrl));
            HttpRequestMessage.Content = new StringContent(requestData);
            HttpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");  
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestMessage">Message to send</param>
        /// <param name="timeout"><inheritdoc cref="RestRequest.RestRequest(IConfiguration, string, HttpMethod, string, int)"/></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RestRequest(HttpRequestMessage requestMessage, int timeout = 10000)
        {
            HttpRequestMessage = requestMessage ?? throw new ArgumentNullException(nameof(requestMessage), "RequestMessage cannot be null");
            TimeOut = TimeSpan.FromMilliseconds(timeout);
        }
        #endregion

        public TimeSpan TimeOut { get; }
        //public string RelativeUrl { get; }
        //public HttpMethod HttpMethod { get; }
        //public string RequestData { get; }
        public HttpRequestMessage HttpRequestMessage { get; }
    }

}
