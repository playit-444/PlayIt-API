using System;
using Common.Networking.Handlers;

namespace Common.Networking.Data.Server
{
    /// <summary>
    /// Interface for grouping server data needed to indentify individual servers
    /// PacketId = 1
    /// </summary>
    public interface IGameServerData : IMagicNumber
    {
        /// <summary>
        /// The guid of the server
        /// </summary>
        public Guid ServerID { get; set; }
    }
}
