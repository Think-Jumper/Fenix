﻿using Fenix.Common.Attributes;
using MessagePack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Shared.Protocol
{
    [RpcArg("code")]
    [DefaultValue(ErrCode.ERROR)]
    public enum ErrCode : Int16
    {
        OK = 0,
        ERROR = -1,

        LOGIN_WRONG_USR_OR_PSW = -1000,
        LOGIN_KICKOUT = -1001,
        LOGIN_CREATE_ACCOUNT_FAIL = -1002,

        MIN_CODE = -32768
    }
}
