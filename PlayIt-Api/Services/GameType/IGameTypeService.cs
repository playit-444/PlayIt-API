using System;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;

namespace PlayIt_Api.Services.GameType
{
    public interface IGameTypeService : IDisposable
    {
        /// <summary>
        /// Get game types
        /// </summary>
        /// <returns></returns>
        Task<IPagedList<Models.Entities.GameType>> GetGameTypes();
    }
}
