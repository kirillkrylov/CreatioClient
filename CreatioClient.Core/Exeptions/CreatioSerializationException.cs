using System;

namespace CreatioClient.Core.Exceptions
{
    public class CreatioSerializationException : Exception
    {
        public SerializedWith Serializer { get; set; }

        public CreatioSerializationException(string message, SerializedWith serializer, Exception innerException): 
            base(message, innerException)
        {
            Serializer = serializer;
        }
    }
}
