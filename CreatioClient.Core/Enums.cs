namespace CreatioClient.Core
{

    

    public enum SerializedWith
    {
        /// <summary>
        /// Microsft JSON serializer
        /// </summary>
        /// <remarks>
        /// See <see href="https://docs.microsoft.com/en-us/dotnet/api/system.text.json"></see> for details
        /// </remarks>
        Microsoft,

        /// <summary>
        /// Newtonsoft JSON serializer
        /// </summary>
        /// <remarks>
        /// See <see href="https://www.newtonsoft.com/json">project repo</see> for details
        /// </remarks>
        Newtonsoft,

        /// <summary>
        /// Contract based serializer that writes data in the "protocol buffers" serialization format engineered by Google.
        /// </summary>
        /// <remarks>
        /// See <see href="https://github.com/protobuf-net/protobuf-net">project repo</see> for details
        /// </remarks>
        ProtobufNet
    }
}
