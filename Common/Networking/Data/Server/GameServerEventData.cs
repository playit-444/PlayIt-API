﻿﻿using System;

namespace Common.Networking.Data.Server
{
    public class GameServerEventData : IGameServerEventData
    {
        public GameServerEventData(string action, Guid serverID)
        {
            Action = action;
            ServerID = serverID;
        }

        public string Action { get; set; }
        public Guid ServerID { get; set; }
        public int PacketId => 2;
    }
}
