﻿using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.KCP; 
using fec;
using Fenix.Common;
using Fenix.Common.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace Fenix
{
    public class KcpHostClient : KcpListener
    {
        public event Action<Ukcp, IByteBuffer> OnReceive;

        public event Action<Ukcp, Exception> OnException;

        public event Action<Ukcp> OnClose;

        protected static KcpClient client;

        protected Ukcp _ukcp;

        public IPEndPoint RemoteAddress => (IPEndPoint)(_ukcp?.user().RemoteAddress);

        public IPEndPoint LocalAddress => (IPEndPoint)(_ukcp?.user().LocalAddress);

        public string ChannelId => _ukcp?.user().Channel.Id.AsLongText();

        public bool IsActive => _ukcp.isActive(); 

        public KcpHostClient(ChannelConfig channelConfig, IPEndPoint remoteAddress)
        {
            if(client == null)
            {
                client = KcpClient.Instance;
                client.init(channelConfig);
            }

            this._ukcp = client.connect(remoteAddress, channelConfig, this);
        }

        public static KcpHostClient Create(IPEndPoint remoteAddress)
        {
            ChannelConfig channelConfig = new ChannelConfig();
            channelConfig.KcpTag = false;
            channelConfig.Crc32Check = true;
            channelConfig.initNodelay(true, 0, 2, true);
            channelConfig.Sndwnd = 512;
            channelConfig.Rcvwnd = 512;
            channelConfig.Mtu = 512;
            channelConfig.FecDataShardCount = 3;
            channelConfig.FecParityShardCount = 1;
            channelConfig.AckNoDelay = true;
            //channelConfig.Conv = 10;//.AutoSetConv = true;
            channelConfig.UseConvChannel = false;

            var listener = new KcpHostClient(channelConfig, remoteAddress);
               
            return listener;
        } 
       

        public void handleReceive(IByteBuffer byteBuf, Ukcp ukcp)
        {
            //OneThreadSynchronizationContext.Instance.Post((obj) =>
            //{
            //byte[] bytes = new byte[byteBuf.ReadableBytes];
            //byteBuf.GetBytes(byteBuf.ReaderIndex, bytes);
            //OnReceive?.Invoke(bytes, ukcp, protocolType);
            OnReceive?.Invoke(ukcp, byteBuf);
                //OnReceive?.Invoke(byteBuf, ukcp, protocolType);
            //}, null); 
        }

        public void handleException(Ukcp ukcp, Exception ex)
        {
            //Console.WriteLine(ex.ToString());
           
            OnException?.Invoke(ukcp, ex); 
        }

        public void handleClose(Ukcp ukcp)
        { 
            OnClose?.Invoke(ukcp);

            Log.Info(Snmp.snmp.ToString());
            Snmp.snmp = new Snmp();
        }

        public void Send(byte[] bytes)
        {
            IByteBuffer buf = Unpooled.WrappedBuffer(bytes);
            //int dataLen = buf.ReadableBytes;
            _ukcp.writeMessage(buf);
        }

        public void handleConnect(Ukcp ukcp)
        {
            
        }

        public void Stop()
        {
            this._ukcp.notifyCloseEvent();
        }
    }
}
