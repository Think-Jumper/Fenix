﻿//AUTOGEN, do not modify it!

using Fenix.Common.Utils;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using MessagePack; 
using System.ComponentModel;
using System; 

namespace Fenix.Common.Message
{
    [MessageType(OpCode.SYNC_FIELD_NTF)]
    [MessagePackObject]
    public class SyncFieldNtf : IMessage
    {
        [Key(0)]
        public global::System.UInt64 actorId { get; set; }

        [Key(1)]
        public global::System.UInt64 dataKey { get; set; }

        [Key(2)]
        public global::Fenix.DataType dataType { get; set; }

        [Key(3)]
        public global::System.UInt32 field { get; set; }

        [Key(4)]
        public global::System.Byte[] data { get; set; }

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<SyncFieldNtf>(this);
        }
        public new static SyncFieldNtf Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<SyncFieldNtf>(data);
        }
    }
}

