﻿﻿using DotNetty.Transport.Channels;
using System;

namespace Common.Networking.Mediation.Slave
{
    /// <summary>
    /// Mediator interface for a slave mediator to connect to its respective master
    /// </summary>
    public interface IServerMediatorSlave : IServerMediator
    {
        /// <summary>
        /// The GUID of the master server
        /// </summary>
        public Guid MasterID { get; }

        /// <summary>
        /// The socket channel to communicate with the master server
        /// </summary>
        public IChannel MasterChannel { get; }
    }
}
