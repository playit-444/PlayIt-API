using System;
using System.Collections.Generic;
using PlayIt_Api.Models.GameServer;

namespace PlayIt_Api.Services.Game
{
    public interface IGameService : IDisposable
    {
        /// <summary>
        /// Get game types
        /// </summary>
        /// <returns></returns>
        IList<IRoomData> GetGameByType(int gameType);

        void AddRoomData(string serverId, IRoomData roomData);
        bool RemoveRoomData(string serverId, string roomId);
        bool UpdateRoomData(string serverId, IRoomData roomData);
        bool CloseServer(string serverId);
    }
}
