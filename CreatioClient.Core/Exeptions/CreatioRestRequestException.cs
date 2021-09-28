using System;
using System.IO;
using System.Net;

namespace CreatioClient.Core.Exceptions
{
    public class CreatioRestRequestException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }
        public Type Type {  get; private set; }
        public Stream Stream { get; private set; }

        public CreatioRestRequestException(string content, Exception innerException) : base(content, innerException)
        {
            
        }

        public CreatioRestRequestException(string content) : base(content)
        {

        }

        public CreatioRestRequestException(HttpStatusCode statusCode, string content, Exception innerException) : this(content, innerException)
        {
            StatusCode = statusCode;
        }
        public CreatioRestRequestException(HttpStatusCode statusCode, Type type, string content, Exception innerException) : this(statusCode, content, innerException)
        {
            Type = type;
        }
        public CreatioRestRequestException(HttpStatusCode statusCode, Type type, Stream stream, string content, Exception innerException) : this(statusCode, type, content, innerException)
        {
            Stream = stream;
        }
    }
}
