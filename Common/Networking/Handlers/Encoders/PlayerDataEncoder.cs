﻿using Common.Networking.Data.Player;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;

namespace Common.Networking.Handlers.Encoders
{
    public class PlayerDataEncoder : MessageToMessageEncoder<IPlayerData>
    {
        protected override void Encode(IChannelHandlerContext context, IPlayerData message, List<object> output)
        {
            if (string.IsNullOrEmpty(message.SessionKey) || message.PlayerID.Equals(0) || message.Username.Equals(null))
                return;

            output.Add(
                ByteBufferUtil.EncodeString(
                    context.Allocator,
                    $"{message.PacketId}:{message.SessionKey}:{message.PlayerID}:{message.Username}",
                    System.Text.Encoding.UTF8));
        }
    }
}
