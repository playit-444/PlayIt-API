using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using Microsoft.AspNetCore.Mvc;
using PlayIt_Api.Logging;
using PlayIt_Api.Models.GameServer;

namespace PlayIt_Api.Services.GameType
{
    /// <summary>
    /// GameType
    /// </summary>
    public sealed class GameTypeService : IGameTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public GameTypeService([FromServices] IUnitOfWork unitOfWork,
            [FromServices] ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IPagedList<Models.Entities.GameType>> GetGameTypes()
        {
            var gameRepo = _unitOfWork.GetRepository<Models.Entities.GameType>();
            //Get a pages list of gameTypes
            return await gameRepo.GetPagedListAsync();
        }

        public async Task<ICollection<GameTypeSimple>> GetGameTypesSimple()
        {
            var gameTypes = await GetGameTypes();
            var gameTypeSimples = new List<GameTypeSimple>();

            //Loop all gameTypes and get simple information
            foreach (var gameType in gameTypes.Items)
            {
                gameTypeSimples.Add(new GameTypeSimple(gameType.GameTypeId, gameType.Name, gameType.MaxPlayers,
                    gameType.MinimumPlayers));
            }

            return gameTypeSimples;
        }

        public async Task<Models.Entities.GameType> GetGameType(int gameTypeId)
        {
            var gameRepo = _unitOfWork.GetRepository<Models.Entities.GameType>();
            return await gameRepo.GetFirstOrDefaultAsync(predicate: a => a.GameTypeId == gameTypeId);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.SaveChangesAsync(true);
                _unitOfWork.Dispose();
                _logger.Dispose();
            }
        }
    }
}
