﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using PlayIt_Api.Models.GameServer;

namespace PlayIt_Api.Services.GameType
{
    public interface IGameTypeService : IDisposable
    {
        /// <summary>
        /// Get all game types
        /// </summary>
        Task<IPagedList<Models.Entities.GameType>> GetGameTypes();

        /// <summary>
        /// Get all game types in a simple format
        /// </summary>
        Task<ICollection<GameTypeSimple>> GetGameTypesSimple();

        /// <summary>
        /// Get specific game type
        /// </summary>
        Task<Models.Entities.GameType> GetGameType(int gameTypeId);

    }
}
