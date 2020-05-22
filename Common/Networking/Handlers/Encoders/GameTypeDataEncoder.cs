﻿﻿using System;
using Common.Networking.Data.Game;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;
using System.Linq;

namespace Common.Networking.Handlers.Encoders
{
    public class GameTypeDataEncoder : MessageToMessageEncoder<ICollection<IGameTypeData>>
    {
        protected override void Encode(IChannelHandlerContext context, ICollection<IGameTypeData> message,
            List<object> output)
        {
            if (message == null)
                return;

            output.Add(ByteBufferUtil.EncodeString(
                context.Allocator,
                $"{message.First().PacketId}",
                System.Text.Encoding.UTF8));
            foreach (var gameTypeData in message)
            {
                output.Add(ByteBufferUtil.EncodeString(
                    context.Allocator,
                    $"{Environment.NewLine}{gameTypeData.GameTypeID}:{gameTypeData.MinPlayers}:{gameTypeData.MaxPlayers}",
                    System.Text.Encoding.UTF8));
            }
        }
    }
}
