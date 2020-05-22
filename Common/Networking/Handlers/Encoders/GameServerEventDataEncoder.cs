﻿﻿using System;
using Common.Networking.Data.Server;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;

namespace Common.Networking.Handlers.Encoders
{
    public class GameServerEventDataEncoder : MessageToMessageEncoder<IGameServerEventData>
    {
        protected override void Encode(IChannelHandlerContext context, IGameServerEventData message, List<object> output)
        {
            if (message == null)
                return;

            output.Add(ByteBufferUtil.EncodeString(context.Allocator, $"{message.PacketId}{Environment.NewLine}{message.ServerID}:{message.Action}", System.Text.Encoding.UTF8));
        }
    }
}
