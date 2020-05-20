using Common.Networking.Handlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Networking.Data.Game
{
    public class GameTypeData : IGameTypeData
    {
        public GameTypeData(int gameTypeID, byte minPlayers, byte maxPlayers)
        {
            GameTypeID = gameTypeID;
            MinPlayers = minPlayers;
            MaxPlayers = maxPlayers;
        }

        public int GameTypeID { get; set; }
        public byte MinPlayers { get; set; }
        public byte MaxPlayers { get; set; }
        public int PacketId => 10;
    }
}
