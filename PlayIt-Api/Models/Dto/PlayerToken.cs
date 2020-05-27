namespace PlayIt_Api.Models.Dto
{
    public class PlayerToken
    {
        public long AccountId { get; set; }

        public PlayerToken(long accountId)
        {
            AccountId = accountId;
        }
    }
}
