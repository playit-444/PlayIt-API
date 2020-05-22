﻿﻿using System;
using System.Collections.Generic;
using Common.Networking.Data.Room;

namespace Common.Networking.Mediation.Master
{
    /// <summary>
    /// Mediator master interface for a master server to manage multiple mediator slaves
    /// </summary>
    public interface IGameServerMediatorMaster : IServerMediatorMaster
    {
        /// <summary>
        /// The available game types, indexed by their respective ID
        /// </summary>
        public IDictionary<int, string> GameTypes { get; }

        /// <summary>
        /// The different server rooms grouped by the servers ID
        /// </summary>
        public IDictionary<Guid, HashSet<IRoomData>> ServerRooms { get; set; }
    }
}
