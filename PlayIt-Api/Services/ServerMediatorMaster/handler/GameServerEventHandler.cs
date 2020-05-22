using Common.Networking.Data.Server;
using DotNetty.Transport.Channels;

namespace PlayIt_Api.Services.ServerMediatorMaster.handler
{
    public class GameServerEventHandler : SimpleChannelInboundHandler<IGameServerEventData>
    {
        protected override void ChannelRead0(IChannelHandlerContext ctx, IGameServerEventData msg)
        {
        }
    }
}
