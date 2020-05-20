using Common.Networking.Data.Server;
using DotNetty.Transport.Channels;

namespace PlayIt_Api.Services.ServerMediatorMaster.handler
{
    public class GameServerHandler : SimpleChannelInboundHandler<IGameServerData>
    {
        /// <summary>
        /// Add Game server to list
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="msg"></param>
        protected override void ChannelRead0(IChannelHandlerContext ctx, IGameServerData msg)
        {
            ServerMediatorMaster.AddGameServer(msg.ServerID, ctx.Channel.Id);
        }
    }
}
