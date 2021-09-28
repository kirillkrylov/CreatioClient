using CreatioClient.Core.Exceptions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CreatioClient.Core.Services
{
    internal interface IRestRequestBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestData"></param>
        /// <returns>Response</returns>
        Task<HttpResponseMessage> InternalExecute(Models.Domain.IRestRequest requestData);
        void Dispose();
    }

    abstract class RestRequestBase : IRestRequestBase, IDisposable
    {
        private readonly HttpClient _httpClient;
        private protected RestRequestBase(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        #region Method : Private
        
        public async Task<HttpResponseMessage> InternalExecute(Models.Domain.IRestRequest requestData)
        {
            
#if !DEBUG
            _httpClient.Timeout = requestData.TimeOut;
#endif
            try
            {
                HttpResponseMessage response = await _httpClient.SendAsync(requestData.HttpRequestMessage);
                return response;
            }
            catch (HttpRequestException ex)
            {
                //TODO: Add Logging
                throw new CreatioRestRequestException(ex.Message, ex);
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            ((IDisposable)_httpClient).Dispose();
        }
        #endregion
    }
}
