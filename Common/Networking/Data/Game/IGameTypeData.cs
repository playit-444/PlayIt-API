﻿﻿using Common.Networking.Handlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Networking.Data.Game
{
    /// <summary>
    /// Request data containing the game id, min and max players needed to play the game
    /// </summary>
    public interface IGameTypeData : IPacketId
    {
        /// <summary>
        /// The game type ID
        /// </summary>
        public int GameTypeID { get; set; }

        /// <summary>
        /// The minimum amount of players needed to play the game
        /// </summary>
        public byte MinPlayers { get; set; }

        /// <summary>
        /// The maximum amount of players needed to play the game
        /// </summary>
        public byte MaxPlayers { get; set; }
    }
}
