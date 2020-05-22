﻿﻿using Common.Networking.Handlers;

namespace Common.Networking.Data.Player
{
    /// <summary>
    /// Mapping class to map a single player to all the rooms they're currently part of
    /// </summary>
    public interface IPlayerRoomData : IPacketId
    {
        /// <summary>
        /// The ID of the player
        /// </summary>
        public long PlayerID { get; }

        /// <summary>
        /// The rooms that the player is currently part of
        /// </summary>
        public string[] RoomIDs { get; }
    }
}
