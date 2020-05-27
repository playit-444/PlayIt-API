namespace PlayIt_Api.Models.GameServer
{
    public class GameTypeSimple
    {
        public int GameTypeId { get; }
        public string Name { get; }
        public int MaxPlayers { get; }
        public int MinimumPlayers { get; }


        public GameTypeSimple(int gameTypeId, string name, int maxPlayers, int minimumPlayers)
        {
            GameTypeId = gameTypeId;
            Name = name;
            MaxPlayers = maxPlayers;
            MinimumPlayers = minimumPlayers;
        }
    }
}
