﻿using Common.Networking.Data.Game;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;

namespace Common.Networking.Handlers.Encoders
{
    class GameTypeDataEncoder : MessageToMessageEncoder<IGameTypeData>
    {
        protected override void Encode(IChannelHandlerContext context, IGameTypeData message, List<object> output)
        {
            if (message == null)
                return;

            output.Add(ByteBufferUtil.EncodeString(
                context.Allocator,
                $"{message.PacketId}:{message.GameTypeID}:{message.MinPlayers}:{message.MaxPlayers}",
                System.Text.Encoding.UTF8));
        }
    }
}
