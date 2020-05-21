﻿using Common.Networking.Handlers;

namespace Common.Networking.Data.Room
{
    /// <summary>
    /// Game room transfer data for sending a simple game room object
    /// PacketId = 5
    /// </summary>
    public interface IGameRoomData : IRoomData
    {
        /// <summary>
        /// The game type ID of the game being played within the room
        /// </summary>
        public int GameType { get; }
    }
}
