using System;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using Microsoft.AspNetCore.Mvc;
using PlayIt_Api.Logging;

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
            return await gameRepo.GetPagedListAsync();
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
            }
        }
    }
}
