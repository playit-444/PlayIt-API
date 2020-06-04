namespace PlayIt_Api.Models.Dto
{
    public class GamePlayerCount
    {
        public int GameTypeId { get; set; }
        public int Amount { get; set; }

        public GamePlayerCount(int gameTypeId, int amount)
        {
            GameTypeId = gameTypeId;
            Amount = amount;
        }
    }
}
