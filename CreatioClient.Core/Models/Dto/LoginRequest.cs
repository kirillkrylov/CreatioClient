using System.Text.Json.Serialization;

namespace CreatioClient.Core.Models.Dto
{
    internal class LoginRequest
    {
        public LoginRequest(string userName, string userPassword)
        {
            UserName = userName;
            UserPassword = userPassword;
        }

        [JsonPropertyName("UserName")]
        public string UserName { get;}

        [JsonPropertyName("UserPassword")]
        public string UserPassword { get;}
    }
}
