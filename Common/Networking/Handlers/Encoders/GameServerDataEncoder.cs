﻿using Common.Networking.Data.Server;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Common.Networking.Handlers.Encoders
{
    /// <summary>
    /// Basic encoding implementation for encoding server guid for sending
    /// </summary>
    public class GameServerDataEncoder : MessageToMessageEncoder<IGameServerData>
    {
        protected override void Encode(IChannelHandlerContext context, IGameServerData message, List<object> output)
        {
            if (message == null)
                return;

            output.Add(ByteBufferUtil.EncodeString(context.Allocator, $"{message.PacketId}:{message.ServerID}",
                Encoding.UTF8));
        }
    }
}
