﻿using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;

namespace Common.Networking.Mediation.Master
{
    /// <summary>
    /// Base mediator for a master server, holding many connections to remote slaves
    /// </summary>
    public interface IServerMediatorMaster : IServerMediator
    {
        /// <summary>
        /// The connected server channels indexed by their respective ID
        /// </summary>
        public IDictionary<Guid, IChannel> ServerChannels { get; }

        /// <summary>
        /// The server ids indexed by the socket id
        /// </summary>
        public IDictionary<IChannelId, Guid> ConnectedServers { get; set; }
    }
}
