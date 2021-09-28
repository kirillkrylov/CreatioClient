using CreatioClient.Core.MessageHandlers;
using CreatioClient.Core.Models.Domain;
using CreatioClient.Core.Models.Dto;
using CreatioClient.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ICredentials = CreatioClient.Core.Models.Domain.ICredentials;

namespace CreatioClient.Core
{
    /// <summary>
    /// Main entry point
    /// </summary>
    public class Client
    {
        #region Fields and Properties
        private IHost _host;
        private readonly ICredentials _credentials;
        private readonly IConfiguration _configuration;
        private readonly object _loginStateLock = new object();
        internal bool LoginState {get; set;}
        #endregion

        #region Constructor
        public Client(string appUrl, string userName, string userPassword, bool UseUntrustedSSL=true, bool isNetCore = false)
        {          
            _credentials = new Credentials(userName, userPassword);
            Uri baseUri = new Uri(appUrl);
            _configuration = new Configuration(baseUri, isNetCore, UseUntrustedSSL);
            BuildHost();
        }
        public Client(string appUrl, bool isNetCore = false)
            :this(appUrl, string.Empty, string.Empty, isNetCore){}
        #endregion

        #region Login
        public bool Login()
        {
            bool result = false;
            Task.Run(async () => result = await LoginAsync()).Wait();
            return result;
        }
        public async Task<bool> LoginAsync()
        {
            var login = _host.Services.GetRequiredService<ILogin>();
            return await login.Execute();
        }
        #endregion

        #region Call ConfigurationService
        public string CallConfigurationService(string serviceName, string serviceMethod, string httpVerb, 
            string requestData, int requestTimeout = 10000)
        {
            string result = string.Empty;
            Task.Run(async () =>
            {
                var msg = await CallConfigurationServiceAsync(serviceName, serviceMethod, httpVerb,
                    requestData="", requestTimeout);
                result = await msg.Content?.ReadAsStringAsync();
            }).Wait();
            return result;
        }


        public async Task<TResult> CallConfigurationServiceDeserializedAsync<TResult>(string serviceName, string serviceMethod, 
            string httpVerb, SerializedWith serializedWith, string requestData = "", int requestTimeout = 10000)
            where TResult: class
        {
            HttpResponseMessage response = await CallConfigurationServiceAsync(serviceName, serviceMethod,
            httpVerb, requestData, requestTimeout);
            return await CreatioSerializer.DeserializeResponse<TResult>(response, serializedWith);
            
        }

        public async Task<HttpResponseMessage> CallConfigurationServiceAsync(string serviceName, string serviceMethod, 
            string httpVerb,string requestData = "", int requestTimeout = 10000)
        {
            Services.IRestRequest restService = _host.Services.GetRequiredService<Services.IRestRequest>();
            IConfiguration conf = _host.Services.GetRequiredService<IConfiguration>();
            string relativeUrl = (conf.IsNetCore) ? $"rest/{serviceName}/{serviceMethod}" : $"0/rest/{serviceName}/{serviceMethod}";

            HttpRequestMessage message = new HttpRequestMessage(new HttpMethod(httpVerb), relativeUrl);

            if (!string.IsNullOrEmpty(requestData))
            {
                HttpContent content = new StringContent(requestData);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                message.Content = content;
            }

            Models.Domain.IRestRequest model = new Models.Domain.RestRequest(message, requestTimeout);

            return await restService.Execute(model);           
        }

        

        #endregion

        #region Execute GetRequest
        public string ExecuteGetRequest(string url, int requestTimeout = 10000)
        {
            string result = string.Empty;
            Task.Run(async () =>
            {
                HttpResponseMessage response = await ExecuteGetRequestAsync(url,requestTimeout);
                result = await response.Content?.ReadAsStringAsync();
            }).Wait();
            return result;
        }

        public async Task<HttpResponseMessage> ExecuteGetRequestAsync(string url, int requestTimeout = 10000)
        {
            Services.IRestRequest restService = _host.Services.GetRequiredService<Services.IRestRequest>();
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, url);

            Models.Domain.IRestRequest model = new Models.Domain.RestRequest(message, requestTimeout);
            return await restService.Execute(model);
        }

        public async Task<TResult> ExecuteGetRequestDeserializedAsync<TResult>(string url, SerializedWith serializedWith,
            int requestTimeout = 10000) where TResult: class
        {
            HttpResponseMessage response = await ExecuteGetRequestAsync(url, requestTimeout);
            return await CreatioSerializer.DeserializeResponse<TResult>(response, serializedWith);
        }

        #endregion

        #region Execute PostRequest
        public string ExecutePostRequest(string url, string requestData, int requestTimeout = 10000)
        {
            string result = string.Empty;
            Task.Run(async () =>
            {
                HttpResponseMessage response = await ExecutePostRequestAsync(url, requestData, requestTimeout);
                result = await response.Content?.ReadAsStringAsync();
            }).Wait();
            return result;
        }
        public async Task<TResult> ExecutePostRequestAsync<TResult>(string url, string requestData, SerializedWith serializedWith,
            int requestTimeout = 10000) where TResult : class
        {
            HttpResponseMessage response = await ExecutePostRequestAsync(url, requestData, requestTimeout);
            return await CreatioSerializer.DeserializeResponse<TResult>(response, serializedWith);
        }
        public async Task<HttpResponseMessage> ExecutePostRequestAsync(string url, string requestData, int requestTimeout = 10000)
        {
            Services.IRestRequest restService = _host.Services.GetRequiredService<Services.IRestRequest>();
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, url);

            if (!string.IsNullOrEmpty(requestData))
            {
                HttpContent content = new StringContent(requestData);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                message.Content = content;
            }
            Models.Domain.IRestRequest model = new Models.Domain.RestRequest(message, requestTimeout);
            return await restService.Execute(model);
        }

        #endregion

        #region WebSocketMessages
        public IDisposable SubscribeToWebSocketMessages(IFilteredObserver<WebSocketMessage> observer)
        {
            IMessageBroker messageBroker =  _host.Services.GetRequiredService<IMessageBroker>();
            return messageBroker.Subscribe(observer);
        }
        #endregion

        #region Methods : Not Implemented
        public string UploadFile(string url, string filePath) 
        {
            throw new NotImplementedException("Not yet implemented");
        }
        public void DownloadFile(string url, string filePath, string requestData)
        {
            throw new NotImplementedException("Not yet implemented");
        }

        #endregion

        #region Methods Private

        internal void SetLoginState(bool state)
        {
            lock (_loginStateLock)
            {
                LoginState = state;
            }
        }

        #endregion

        #region Initial Configuration
        private void BuildHost()
        {
            _host = new HostBuilder()
                .ConfigureServices(ConfigureServices)
                .Build();
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddSingleton((provider) =>
            {
                return _credentials;
            });

            services.AddSingleton((provider) =>
            {
                return _configuration;
            });

            services.AddSingleton(typeof(CookieContainer), new CookieContainer());

            services.AddSingleton<CsrfMessageHandler>();

            services.AddSingleton<HttpClientHandler>(provider =>
            {
                IConfiguration conf = provider.GetRequiredService<IConfiguration>();

                var _handler =  new HttpClientHandler
                {
                    UseCookies = true,
                    CookieContainer = provider.GetRequiredService<CookieContainer>()
                };
               
                if (conf.UseUntrustedSSL)
                {
                    _handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    _handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChail, sslPolicyErrors) =>
                    {
                        return true;
                    };
                };

                return _handler;
            });

            services.AddSingleton<ConcurrentQueue<WebSocketMessage>>();

            services.AddHttpClient<ILogin, Login>((provider, client) =>
            {
                IConfiguration conf = provider.GetRequiredService<IConfiguration>();
                client.BaseAddress = conf.AppUri;
            })
            .ConfigurePrimaryHttpMessageHandler(provider =>
            {
                return provider.GetRequiredService<HttpClientHandler>();
            });

            services.AddHttpClient<Services.IRestRequest, Services.RestRequest>((provider, client) =>
            {
                IConfiguration conf = provider.GetRequiredService<IConfiguration>();
                client.BaseAddress = conf.AppUri;

                // Keep-alive: true
                // see original implementation
                // https://github.com/Advance-Technologies-Foundation/creatioclient/blob/1eac828d5636f178ae38738296ebd58c88c1d0dc/creatioclient/CreatioClient.cs#L242
                client.DefaultRequestHeaders.ConnectionClose = false;
            })
            .ConfigurePrimaryHttpMessageHandler(provider =>
            {
                return provider.GetRequiredService<HttpClientHandler>();
            })
            .AddHttpMessageHandler(provider =>
            {
                ILogin _login = provider.GetRequiredService<ILogin>();
                return new UnauthorizedMessageHandler(_login);
            })
            .AddHttpMessageHandler(provider =>
            {
                var cookies = provider.GetRequiredService<CookieContainer>();
                IConfiguration conf = provider.GetRequiredService<IConfiguration>();
                return new CsrfMessageHandler(cookies, conf);
            });

            services.AddSingleton<IMessageListener, MessageListener>();
            services.AddSingleton<IMessageBroker, MessageBroker>();
        }

        #endregion
    }
}
