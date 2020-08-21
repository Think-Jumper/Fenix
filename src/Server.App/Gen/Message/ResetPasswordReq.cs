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
    [MessageType(ProtocolCode.__SERVERGMODULE__LOGINSERVICE__RESET_PASSWORD_REQ)]
    [MessagePackObject]
    public class ResetPasswordReq : IMessage
    {
        [Key(0)]
        public global::System.String username { get; set; }

        [Key(1)]
        public global::System.String email { get; set; }

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<ResetPasswordReq>(this);
        }

        public new static ResetPasswordReq Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<ResetPasswordReq>(data);
        }

        public override void UnPack(byte[] data)
        {
            var obj = Deserialize(data);
            Copier<ResetPasswordReq>.CopyTo(obj, this);
        }
    }
}

