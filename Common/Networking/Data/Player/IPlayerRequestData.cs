﻿﻿using System;
using System.Collections.Generic;
using System.Text;
using Common.Networking.Handlers;

namespace Common.Networking.Data.Player
{
    /// <summary>
    /// Simple object containing a user session key,
    /// as a way to identify a current user on the site and make a request for additional data with
    /// PacketId = 4
    /// </summary>
    public interface IPlayerRequestData : IPacketId
    {
        /// <summary>
        /// The session key that the user is currently using
        /// </summary>
        public string SessionKey { get; set; }
    }
}
