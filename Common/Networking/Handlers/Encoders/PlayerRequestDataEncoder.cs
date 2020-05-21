﻿using System;
using Common.Networking.Data.Player;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;

namespace Common.Networking.Handlers.Encoders
{
    public class PlayerRequestDataEncoder : MessageToMessageEncoder<IPlayerRequestData>
    {
        protected override void Encode(IChannelHandlerContext context, IPlayerRequestData message, List<object> output)
        {
            if (message == null)
                return;

            output.Add(
                ByteBufferUtil.EncodeString(
                    context.Allocator,
                    $"{message.PacketId}:{message.SessionKey}",
                    System.Text.Encoding.UTF8));

        }
    }
}
