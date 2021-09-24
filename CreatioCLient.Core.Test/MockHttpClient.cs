using CreatioClient.Core.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CreatioClient.Core.Test
{
    enum DesiredResponses{

        LoginSucceeded,
        LoginFailedWithBadCredentials,
        LoginFailedWithWrongRequest
    }

    internal static class MockHttpClient
    {
        private static Uri baseUri = new Uri("https://creatio.com");


        public static HttpClient Create(DesiredResponses desiredResponses)
        {

            switch (desiredResponses)
            {
                case DesiredResponses.LoginSucceeded:
                    var client = new HttpClient(new MockHttpMessageHandler(CreateLoginSucceeded()));
                    client.BaseAddress = baseUri;
                    return client;
                case DesiredResponses.LoginFailedWithBadCredentials:
                    client =  new HttpClient(new MockHttpMessageHandler(CreateLoginFailedWithBadCredentials()));
                    client.BaseAddress = baseUri;
                    return client;
                case DesiredResponses.LoginFailedWithWrongRequest:
                    client =  new HttpClient(new MockHttpMessageHandler(CreateLoginFailedWithWrongRequest()));
                    client.BaseAddress = baseUri;
                    return client;
                default:
                    throw new NotImplementedException("No mock exists for such request");                   
            }
        }

        public static HttpClient Create(HttpStatusCode code)
        {
            var response = new HttpResponseMessage(code);
            var handler = new MockHttpMessageHandler(response);
            return new HttpClient(handler);
        }


        private static HttpResponseMessage CreateLoginSucceeded()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            LoginResponse loginResponseMock = new LoginResponse(0, "", null, null, null);
            response.Content = new StringContent(JsonSerializer.Serialize(loginResponseMock));
            return response;

        }
        
        private static HttpResponseMessage CreateLoginFailedWithBadCredentials()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            Cookie BPMLOADER = new Cookie("BPMLOADER", "kvwchwfcdv1xwknq5lfsi4mr", "/");
            CookieHeaderValue chvBPMLOADER = new CookieHeaderValue(BPMLOADER.Name, BPMLOADER.Value);

            CookieHeaderValue[] cookies = new CookieHeaderValue[1] { chvBPMLOADER };
            response.Headers.AddCookies(cookies);

            Models.Dto.Exception exception = new Models.Dto.Exception(null, null,
                "Security error.", null, "System.Security.SecurityException");

            LoginResponse loginResponseMock = new LoginResponse(1, "Error Message",
                exception, null, null);
            response.Content = new StringContent(JsonSerializer.Serialize(loginResponseMock));
            return response;

        }
        private static HttpResponseMessage CreateLoginFailedWithWrongRequest()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            
            Cookie BPMLOADER = new Cookie("BPMLOADER", "kvwchwfcdv1xwknq5lfsi4mr", "/");
            CookieHeaderValue chvBPMLOADER = new CookieHeaderValue(BPMLOADER.Name, BPMLOADER.Value);
            
            CookieHeaderValue[] cookies = new CookieHeaderValue[1] { chvBPMLOADER };
            response.Headers.AddCookies(cookies);

            Models.Dto.Exception exception = new Models.Dto.Exception(null, null, "Security error.", null, "System.Security.SecurityException");
            
            LoginResponse loginResponseMock = new LoginResponse(1, "Error Message",
                exception, null, null);
            response.Content = new StringContent(JsonSerializer.Serialize(loginResponseMock));
            return response;

        }
    }

    internal class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _httpResponseMessage;

        public MockHttpMessageHandler(HttpResponseMessage httpResponseMessage)
        {
            _httpResponseMessage = httpResponseMessage;
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_httpResponseMessage);
        }
    }
}
