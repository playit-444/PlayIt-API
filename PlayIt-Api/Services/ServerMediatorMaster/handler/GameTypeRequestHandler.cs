using System.Collections.Generic;
using Arch.EntityFrameworkCore.UnitOfWork;
using Common.Networking.Data.Game;
using DotNetty.Transport.Channels;

namespace PlayIt_Api.Services.ServerMediatorMaster.handler
{
    public class GameTypeRequestHandler : SimpleChannelInboundHandler<IGameTypeRequestData>
    {
        /// <summary>
        /// Add Game server to list
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="msg"></param>
        private IUnitOfWork UnitOfWork;

        public GameTypeRequestHandler(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        protected override async void ChannelRead0(IChannelHandlerContext ctx, IGameTypeRequestData msg)
        {
            var gameTypeRepo = UnitOfWork.GetRepository<Models.Entities.GameType>();
            var gameTypes = await gameTypeRepo.GetPagedListAsync();

            var gameTypeData = new List<IGameTypeData>();
            foreach (var gameType in gameTypes.Items)
            {
                gameTypeData.Add(new GameTypeData(gameType.GameTypeId, gameType.MinimumPlayers, gameType.MaxPlayers));
            }

            await ctx.Channel.WriteAndFlushAsync(gameTypeData);
        }
    }
}
