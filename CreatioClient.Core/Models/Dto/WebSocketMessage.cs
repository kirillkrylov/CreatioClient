using System;
using System.Text.Json.Serialization;

namespace CreatioClient.Core.Models.Dto
{
    public class WebSocketMessage
    {
		[JsonPropertyName("Id")]
		public Guid MessageId { get; set; }

		[JsonPropertyName("Header")]
		public Header MessageHeader { get; set; }

		[JsonPropertyName("Body")]
		public string MessageBody { get; set; }
	}

	public class Header
	{
		[JsonPropertyName("Sender")]
		public string Sender { get; set; }

		[JsonPropertyName("BodyTypeName")]
		public string BodyTypeName { get; set; }
	}
}
