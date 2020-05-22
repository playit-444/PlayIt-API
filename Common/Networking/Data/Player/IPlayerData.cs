﻿﻿using Common.Networking.Handlers;

namespace Common.Networking.Data.Player
{
    /// <summary>
    /// PlayerData object for storing player data upon it being requested by the gameserver
    /// PacketId = 3
    /// </summary>
    public interface IPlayerData : IPacketId
    {
        /// <summary>
        /// The session key used when initially requesting the data
        /// </summary>
        public string SessionKey { get; set; }

        /// <summary>
        /// The persistent id of the player
        /// </summary>
        public long PlayerID { get; set; }

        /// <summary>
        /// The username of the player
        /// </summary>
        public string Username { get; set; }
    }
}
