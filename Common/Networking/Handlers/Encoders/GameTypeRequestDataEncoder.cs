﻿﻿using Common.Networking.Data.Game;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;

namespace Common.Networking.Handlers.Encoders
{
    public class GameTypeRequestDataEncoder : MessageToMessageEncoder<IGameTypeRequestData>
    {
        protected override void Encode(IChannelHandlerContext context, IGameTypeRequestData message, List<object> output)
        {
            if (message == null)
                return;

            output.Add(ByteBufferUtil.EncodeString(context.Allocator, message.PacketId.ToString(), System.Text.Encoding.UTF8));
        }
    }
}
