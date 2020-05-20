using System;
using Common.Networking.Data.Server;
using DotNetty.Transport.Channels;

namespace PlayIt_Api.Services.ServerMediatorMaster.handler
{
    public class GameServerEventHandler : SimpleChannelInboundHandler<IGameServerData>
    {
        protected override void ChannelRead0(IChannelHandlerContext ctx, IGameServerData msg)
        {
            Console.WriteLine();
            //ServerMediatorMaster.AddGameServer(msg.ServerID, ctx.Channel);
        }
    }
}
