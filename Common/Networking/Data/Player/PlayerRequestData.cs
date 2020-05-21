﻿
namespace Common.Networking.Data.Player
{
    public class PlayerRequestData : IPlayerRequestData
    {
        public PlayerRequestData(string sessionKey)
        {
            SessionKey = sessionKey;
        }

        public string SessionKey { get; set; }
        public int PacketId => 4;
    }
}
