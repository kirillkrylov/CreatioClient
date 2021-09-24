using ProtoBuf;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CreatioClient.Console.DTO
{
    [ProtoContract]
    public class Json
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("Email")]
        public string Email { get; set; }

        [JsonPropertyName("files")]
        public IEnumerable<CreatioFileJson> Files { get; set; }
    }

    public class CreatioFileJson
    {
        [JsonPropertyName("fileName")]
        public string FileName { get; set; }

        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; }

        [JsonPropertyName("data")]
        public string Data { get; set; }
    }

}
