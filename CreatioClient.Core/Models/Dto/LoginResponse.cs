using System.Text.Json.Serialization;

namespace CreatioClient.Core.Models.Dto
{
    internal class LoginResponse
    {
        public LoginResponse(int code, string message, Exception exception, string passwordChangeUrl, string redirectUrl)
        {
            Code = code;
            Message = message;
            Exception = exception;
            PasswordChangeUrl = passwordChangeUrl;
            RedirectUrl = redirectUrl;
        }

        [JsonPropertyName("Code")]
        public int Code { get; } = 1;

        [JsonPropertyName("Message")]
        public string Message { get; }

        [JsonPropertyName("Exception")]
        public Exception Exception { get; }

        [JsonPropertyName("PasswordChangeUrl")]
        public string PasswordChangeUrl { get; }
        
        [JsonPropertyName("RedirectUrl")]
        public string RedirectUrl { get; }
    }
}
