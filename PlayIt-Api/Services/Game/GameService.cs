using System;
using System.Collections.Generic;
using System.Linq;
using Common.Networking.Data.Room;
using Microsoft.AspNetCore.Mvc;
using PlayIt_Api.Logging;

namespace PlayIt_Api.Services.Game
{
    /// <summary>
    /// GameType
    /// </summary>
    public sealed class GameService : IGameService
    {
        private readonly ILogger _logger;

        public GameService(
            [FromServices] ILogger logger)
        {
            _logger = logger;
        }

        public IList<Models.Dto.Game> GetGameByType(int gameType)
        {
            var games = new List<Models.Dto.Game>();
            var serverRooms = ServerMediatorMaster.ServerMediatorMaster.GetServerRooms();
            if (serverRooms != null)
            {
                foreach (var serverRoomsValue in serverRooms.Values)
                {
                    foreach (IGameRoomData roomData in serverRoomsValue.OfType<IGameRoomData>())
                    {
                        if (roomData.GameType == gameType)
                        {
                            games.Add(new Models.Dto.Game(roomData.RoomID, roomData.Name, roomData.MaxUsers,
                                roomData.CurrentUsers, roomData.Private));
                        }
                    }
                }
            }

            return games;
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
