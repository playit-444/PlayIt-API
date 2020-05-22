﻿﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Networking.Mediation
{
    /// <summary>
    /// Base interface for mediation, containing the servers own ID and the port which mediation occurs on
    /// </summary>
    public interface IServerMediator
    {
        /// <summary>
        /// The server ID represented in a GUID
        /// </summary>
        public Guid ServerID { get; }

        /// <summary>
        /// The port that mediation will occur on
        /// </summary>
        public int Port { get; }
    }
}
