using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PlayIt_Api.Logging;
using PlayIt_Api.Models.GameServer;

namespace PlayIt_Api.Services.Game
{
    /// <summary>
    /// GameType
    /// </summary>
    public sealed class GameService : IGameService
    {
        private readonly ILogger _logger;

        private static IDictionary<string, ICollection<IRoomData>> _roomDatas =
            new Dictionary<string, ICollection<IRoomData>>();

        public GameService(
            [FromServices] ILogger logger)
        {
            _logger = logger;
        }

        public IList<IRoomData> GetGameByType(int gameType)
        {
            var games = new List<IRoomData>();

            lock (_roomDatas)
            {
                foreach (var roomData in _roomDatas)
                {
                    games.AddRange(roomData.Value.Where(a => a.GameType == gameType));
                }
            }

            return games;
        }

        public void AddRoomData(string serverId, IRoomData roomData)
        {
            lock (_roomDatas)
            {
                if (!_roomDatas.ContainsKey(serverId))
                {
                    _roomDatas[serverId] = new List<IRoomData> {roomData};
                }

                if (!_roomDatas[serverId].Contains(roomData))
                    _roomDatas[serverId].Add(roomData);
            }
        }

        public bool RemoveRoomData(string serverId, string roomId)
        {
            lock (_roomDatas)
            {
                if (_roomDatas.ContainsKey(serverId))
                {
                    foreach (var room in _roomDatas[serverId])
                    {
                        if (room.RoomID == roomId)
                        {
                            _roomDatas[serverId].Remove(room);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool UpdateRoomData(string serverId, IRoomData roomData)
        {
            lock (_roomDatas)
            {
                if (_roomDatas.ContainsKey(serverId))
                {
                    var room = _roomDatas[serverId].Single(r => r.RoomID == roomData.RoomID);
                    room.CurrentUsers = roomData.CurrentUsers;
                    return true;
                }
            }

            return false;
        }

        public bool CloseServer(string serverId)
        {
            lock (_roomDatas)
            {
                if (_roomDatas.ContainsKey(serverId))
                {
                    _roomDatas.Remove(serverId);
                    return true;
                }
            }

            return false;
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
            }
        }
    }
}
