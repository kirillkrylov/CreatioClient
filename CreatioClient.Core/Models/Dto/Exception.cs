using System.Text.Json.Serialization;

namespace CreatioClient.Core.Models.Dto
{
    internal class Exception
    {
        public Exception(string helpLink, string innerException, string message, string stackTrace, string type)
        {
            HelpLink = helpLink;
            InnerException = innerException;
            Message = message;
            StackTrace = stackTrace;
            Type = type;
        }

        [JsonPropertyName("HelpLink")]
        public string HelpLink { get; }
        
        [JsonPropertyName("InnerException")]
        public string InnerException { get; }
        
        [JsonPropertyName("Message")]
        public string Message { get; }
        
        [JsonPropertyName("StackTrace")]
        public string StackTrace { get; }
        
        [JsonPropertyName("Type")]
        public string Type { get; }
    }
}
