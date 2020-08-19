﻿using Fenix.Common.Rpc;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Shared.DataModel
{
    //public class UserAttr : IMessage
    //{
    //    [Key(0)]
    //    public int Coin;

    //    [Key(1)]
    //    public int BlueCoin;

    //    [Key(2)]
    //    public int RedCoin;

    //    [Key(3)]
    //    public int Strength;

    //    [Key(4)]
    //    public int Spirit;

    //    [Key(5)]
    //    public int Power;
    //}

    [MessagePackObject]
    public partial class User : IMessage
    {
        //[Key(0)]
        //public UserAttr Attr;

        [Key(0)]
        public string uid;

        [Key(1)]
        public string Name; 

        [Key(5)]
        public int Exp;

        [Key(10)]
        public int Coin;

        [Key(11)]
        public int BlueCoin;

        [Key(12)]
        public int RedCoin;

        [Key(13)]
        public int Strength;

        [Key(14)]
        public int Spirit;

        [Key(15)]
        public int Power;

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize<User>(this);
        }

        public new static User Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<User>(data);
        }

        public override void UnPack(byte[] data)
        {
            var obj = Deserialize(data);
            Copier<User>.CopyTo(obj, this);
        }
    }
}
