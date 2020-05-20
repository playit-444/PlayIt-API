using System;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;

namespace PlayIt_Api.Services.GameType
{
    public interface IGameTypeService : IDisposable
    {
        /// <summary>
        /// Get all game types
        /// </summary>
        /// <returns></returns>
        Task<IPagedList<Models.Entities.GameType>> GetGameTypes();
        /// <summary>
        /// Get specific game type
        /// </summary>
        /// <returns></returns>
        Task<Models.Entities.GameType> GetGameType(int gameTypeId);

    }
}
