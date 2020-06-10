using System;
using System.Collections.Generic;
using PlayIt_Api.Models.Dto;
using PlayIt_Api.Models.GameServer;

namespace PlayIt_Api.Services.Game
{
    public interface IGameService : IDisposable
    {
        /// <summary>
        /// Get games by gameType
        /// </summary>
        /// <returns>A list of games by a specific gameType</returns>
        IList<IRoomData> GetGameByType(int gameType);

        /// <summary>
        /// Get playerCount divided by gameType
        /// </summary>
        /// <returns></returns>
        IList<GamePlayerCount> GetGamePlayerCountByGameType();

        /// <summary>
        /// Add IRoomData to list
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="roomData"></param>
        void AddRoomData(string serverId, IRoomData roomData);

        /// <summary>
        /// Remove specific room from collection of rooms for the gameServer
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="roomId"></param>
        bool RemoveRoomData(string serverId, string roomId);

        /// <summary>
        /// Update specific room in collection for the gameServer
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="roomData"></param>
        bool UpdateRoomData(string serverId, IRoomData roomData);

        /// <summary>
        /// When a server close delete all rooms for the specific gameServer
        /// </summary>
        /// <param name="serverId"></param>
        bool CloseServer(string serverId);
    }
}
