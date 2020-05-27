namespace PlayIt_Api.Models.GameServer
{
    public class PlayerVerificationResponse
    {
        public PlayerVerificationResponse(string key, long playerId, string name)
        {
            Key = key;
            PlayerId = playerId;
            Name = name;
        }

        public string Key { get; set; }
        public long PlayerId { get; set; }
        public string Name { get; set; }
    }
}
