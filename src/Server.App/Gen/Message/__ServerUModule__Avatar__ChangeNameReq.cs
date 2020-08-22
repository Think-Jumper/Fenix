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
    [MessageType(ProtocolCode.__SERVERUMODULE__AVATAR__CHANGE_NAME_REQ)]
    [MessagePackObject]
    public class __ServerUModule__Avatar__ChangeNameReq : IMessageWithCallback
    {
        [Key(0)]
        public global::System.String name { get; set; }

        [Key(1)]

        public Callback callback
        {
            get => _callback as Callback;
            set => _callback = value;
        } 

        [MessagePackObject]
        public class Callback : IMessage
        {
            [Key(0)]
            public global::Shared.Protocol.ErrCode code { get; set; } = ErrCode.ERROR;

            public override byte[] Pack()
            {
                return MessagePackSerializer.Serialize<Callback>(this);
            }

            public new static Callback Deserialize(byte[] data)
            {
                return MessagePackSerializer.Deserialize<Callback>(data);
            }

            public override void UnPack(byte[] data)
            {
                var obj = Deserialize(data);
                Copier<Callback>.CopyTo(obj, this);
            }
        }

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<__ServerUModule__Avatar__ChangeNameReq>(this);
        }

        public new static __ServerUModule__Avatar__ChangeNameReq Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<__ServerUModule__Avatar__ChangeNameReq>(data);
        }

        public override void UnPack(byte[] data)
        {
            var obj = Deserialize(data);
            Copier<__ServerUModule__Avatar__ChangeNameReq>.CopyTo(obj, this);
        }
    }
}

