using CreatioClient.Core.Exceptions;
using CreatioClient.Core.Models.Domain;
using CreatioClient.Core.Models.Dto;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ICredentials = CreatioClient.Core.Models.Domain.ICredentials;

namespace CreatioClient.Core.Services
{

    internal interface ILogin 
    {
        Login.State CurrentState { get; }
        DateTime LastLogin { get; }
    
        Task<bool> Execute();
        void SetCurrentLogInState(Login.State state);
    }

    internal class Login : RestRequestBase, ILogin, IDisposable
    {
        public enum State
        {
            LoggedIn,
            LoggedOut,
            Unknown
        }

        private readonly ICredentials _credentials;
        private readonly IConfiguration _configuration;
        private const string _relativeUrl = "ServiceModel/AuthService.svc/Login";

        #region Constructor
        public Login(HttpClient httpClient, ICredentials credentials, IConfiguration configuration) : base(httpClient)
        {
            _credentials = credentials;
            _configuration = configuration;
        }
        #endregion
        private readonly object _lock = new object();
        public DateTime LastLogin { get; private set; }
        public State CurrentState { get; private set; } = State.Unknown;



       
        public async Task<bool> Execute()
        {
            var loginRequest = new LoginRequest(_credentials.UserName, _credentials.UserPassword);

            Models.Domain.IRestRequest request = new Models.Domain.RestRequest(_configuration,
                _relativeUrl,HttpMethod.Post, JsonSerializer.Serialize(loginRequest));

            HttpResponseMessage response = await InternalExecute(request);

            if(SuccessStatusCodes.IsStatusSuccess(typeof(ILogin), response.StatusCode))
            {
                try
                {
                    var loginResponse = await CreatioSerializer.DeserializeResponse<LoginResponse>(
                        response, SerializedWith.Microsoft);
                    
                    if(loginResponse.Code == 0)
                    {
                        SetCurrentLogInState(State.LoggedIn);
                    }
                    return loginResponse.Code == 0;
                }
                catch (CreatioSerializationException)
                {
                    return false;
                }
            }       
            return false;
        }

        public void SetCurrentLogInState(State state)
        {
            lock (_lock)
            {
                CurrentState = state;
                LastLogin = DateTime.UtcNow;
            }
        }
    }
}
