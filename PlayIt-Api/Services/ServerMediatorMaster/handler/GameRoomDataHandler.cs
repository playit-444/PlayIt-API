using System;
using System.Collections.Generic;
using Common.Networking.Data.Room;
using Common.Networking.Data.Server;
using DotNetty.Transport.Channels;

namespace PlayIt_Api.Services.ServerMediatorMaster.handler
{
    public class GameRoomDataHandler : SimpleChannelInboundHandler<IGameRoomData>
    {
        protected override void ChannelRead0(IChannelHandlerContext ctx, IGameRoomData msg)
        {
            var gameServerGuid = ServerMediatorMaster.GetGameServerGuid(ctx.Channel.Id);
            ServerMediatorMaster.AddServerRooms(gameServerGuid,
                new GameRoomData(msg.RoomID, msg.Name, msg.MaxUsers, msg.CurrentUsers, msg.Private, msg.GameType));
        }
    }
}
