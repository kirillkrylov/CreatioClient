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

        public CreatioRestRequestException( string content) : base(content)
        {
            
        }
        public CreatioRestRequestException(HttpStatusCode statusCode, string content) : this(content)
        {
            StatusCode = statusCode;
        }
        public CreatioRestRequestException(HttpStatusCode statusCode, Type type, string content) : this(statusCode, content)
        {
            Type = type;
        }
        public CreatioRestRequestException(HttpStatusCode statusCode, Type type, Stream stream, string content) : this(statusCode, type, content)
        {
            Stream = stream;
        }
    }

}
