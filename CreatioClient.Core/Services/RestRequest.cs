using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CreatioClient.Core.Services
{
    /// <summary>
    /// Methods for calling any service
    /// </summary>
    internal interface IRestRequest
    {
        Task<HttpResponseMessage> Execute(Models.Domain.IRestRequest requestParam);
        Task<TResult> Execute<TResult>(Models.Domain.IRestRequest requestParam, SerializedWith serializedWith) where TResult : class;
       
    }

    internal class RestRequest : RestRequestBase, IRestRequest, IDisposable
    {
       #region Constructor
        public RestRequest(HttpClient httpClient) : base(httpClient) { }
        
        #endregion


        #region Methods : Public
        public async Task<HttpResponseMessage> Execute(Models.Domain.IRestRequest requestParam)
        {
            if (requestParam == null)
            {
                throw new ArgumentNullException(nameof(requestParam), "_requestParam cannot be null, use SetRequestParameters method first");
            }
            return await InternalExecute(requestParam);
        }
        public async Task<TResult> Execute<TResult>(Models.Domain.IRestRequest requestParam, SerializedWith serializedWith) where TResult: class
        {
            HttpResponseMessage response = await Execute(requestParam);
            return await CreatioSerializer.DeserializeResponse<TResult>(response, serializedWith);
        }
        #endregion
    }
}
