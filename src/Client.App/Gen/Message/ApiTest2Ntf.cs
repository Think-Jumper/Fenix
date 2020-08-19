﻿//AUTOGEN, do not modify it!

using Fenix.Common.Utils;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using MessagePack; 
using System.ComponentModel;
using Shared;
using Shared.Protocol;
using Shared.DataModel;
using System; 

namespace Shared.Message
{
    [MessageType(ProtocolCode.API_TEST2_NTF)]
    [MessagePackObject]
    public class ApiTest2Ntf : IMessage
    {
        [Key(0)]
        public global::System.String uid { get; set; }

        [Key(1)]
        public global::System.Int32 match_type { get; set; }

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<ApiTest2Ntf>(this);
        }

        public new static ApiTest2Ntf Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<ApiTest2Ntf>(data);
        }

        public override void UnPack(byte[] data)
        {
            var obj = Deserialize(data);
            Copier<ApiTest2Ntf>.CopyTo(obj, this);
        }
    }
}

