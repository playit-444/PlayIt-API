﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Networking.Data.Server
{
    public class GameServerData : IGameServerData
    {
        public GameServerData(Guid serverID)
        {
            ServerID = serverID;
        }

        public Guid ServerID { get; set; }
        public int PacketId => 1;
    }
}
