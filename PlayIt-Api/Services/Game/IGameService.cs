using System;
using System.Collections.Generic;

namespace PlayIt_Api.Services.Game
{
    public interface IGameService : IDisposable
    {
        /// <summary>
        /// Get game types
        /// </summary>
        /// <returns></returns>
        IList<Models.Dto.Game> GetGameByType(int gameType);
    }
}
