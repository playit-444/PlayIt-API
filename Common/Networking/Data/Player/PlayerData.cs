﻿﻿
namespace Common.Networking.Data.Player
{
    public class PlayerData : IPlayerData
    {
        public PlayerData(string sessionKey, long playerID, string username)
        {
            SessionKey = sessionKey;
            PlayerID = playerID;
            Username = username;
        }

        public long PlayerID { get; set; }
        public string Username { get; set; }
        public string SessionKey { get; set; }
        public int PacketId => 3;
    }
}
