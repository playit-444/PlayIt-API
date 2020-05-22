using System;
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

        private readonly Guid _serverId;
        public GameServerHandler(Guid serverId)
        {
            _serverId = serverId;
        }
        protected override async void ChannelRead0(IChannelHandlerContext ctx, IGameServerData msg)
        {
            ServerMediatorMaster.AddGameServer(msg.ServerID, ctx.Channel.Id);
            await ctx.Channel.WriteAndFlushAsync(new GameServerData(_serverId));

        }
    }
}
