using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PlayIt_Api.Logging;
using PlayIt_Api.Models.Dto;
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
                //Loop through dictionary
                foreach (var roomData in _roomDatas)
                {
                    //Add to temp list
                    games.AddRange(roomData.Value.Where(a => a.GameType == gameType));
                }
            }

            return games;
        }

        public IList<GamePlayerCount> GetGamePlayerCountByGameType()
        {
            var games = new List<GamePlayerCount>();

            lock (_roomDatas)
            {
                //loop through dictionary
                foreach (var roomData in _roomDatas)
                {
                    //Loop all values
                    foreach (var room in roomData.Value)
                    {
                        //Check if GameType already exists in list
                        var gamePlayerCount = games.SingleOrDefault(a => a.GameTypeId == room.GameType);
                        if (gamePlayerCount == null)
                        {
                            games.Add(new GamePlayerCount(room.GameType, room.CurrentUsers));
                        }
                        else
                        {
                            gamePlayerCount.Amount += room.CurrentUsers;
                        }
                    }
                }
            }

            return games;
        }

        public void AddRoomData(string serverId, IRoomData roomData)
        {
            lock (_roomDatas)
            {
                //Check if gameServer does not exists
                if (!_roomDatas.ContainsKey(serverId))
                {
                    _roomDatas[serverId] = new List<IRoomData> {roomData};
                }

                //Check if room already exists
                if (!_roomDatas[serverId].Contains(roomData))
                    _roomDatas[serverId].Add(roomData);
            }
        }

        public bool RemoveRoomData(string serverId, string roomId)
        {
            lock (_roomDatas)
            {
                //Check if gameServer exists
                if (_roomDatas.ContainsKey(serverId))
                {
                    //Loop all rooms for the specific gameServer
                    foreach (var room in _roomDatas[serverId])
                    {
                        //Check if roomId matches the request to be remove
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
                //Check if gameServer exists
                if (_roomDatas.ContainsKey(serverId))
                {
                    //Update information about the room
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
                //Check if gameServer exists
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
                _logger.Dispose();
            }
        }
    }
}
