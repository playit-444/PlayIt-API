﻿﻿using System;
using System.Collections.Generic;
using System.Text;
using Common.Networking.Handlers;

namespace Common.Networking.Data.Server
{
    /// <summary>
    /// Interface for grouping server event data
    /// PacketId = 2
    /// </summary>
    public interface IGameServerEventData : IGameServerData, IPacketId
    {
        public string Action { get; set; }
    }
}
