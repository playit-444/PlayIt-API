﻿﻿using System;
using Common.Networking.Data.Player;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;
using Common.Networking.Data.Room;

namespace Common.Networking.Handlers.Encoders
{
    public class GameRoomDataEncoder : MessageToMessageEncoder<IGameRoomData>
    {
        protected override void Encode(IChannelHandlerContext context, IGameRoomData message, List<object> output)
        {
            if (string.IsNullOrEmpty(message.RoomID) || message.GameType.Equals(0))
                return;

            output.Add(
                ByteBufferUtil.EncodeString(
                    context.Allocator,
                    $"{message.PacketId}{Environment.NewLine}{message.RoomID}:{message.Name}:{message.MaxUsers}:{message.CurrentUsers}:{message.Private}:{message.GameType}",
                    System.Text.Encoding.UTF8,
                    10));
        }
    }
}
