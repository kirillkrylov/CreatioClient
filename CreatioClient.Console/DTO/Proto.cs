using ProtoBuf;
using System.Collections.Generic;

namespace CreatioClient.Console.DTO
{
    [ProtoContract]
    public class Proto
    {
        [ProtoMember(1)]
        public string FirstName { get; set; }
        
        [ProtoMember(2)]
        public string LastName { get; set; }
        
        [ProtoMember(3)]
        public string Email { get; set; }
        
        [ProtoMember(4)]
        public List<CreatioFileProto> Files{ get; set; }
    }

    [ProtoContract]
    public class CreatioFileProto
    {
        [ProtoMember(1)]
        public string FileName { get; set; }

        [ProtoMember(2)]
        public string MimeType { get; set; }

        [ProtoMember(3)]
        public byte[] Data { get; set; }
    }
}
