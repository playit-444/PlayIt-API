namespace PlayIt_Api.Models.Dto
{
    public class Account
    {
        public long AccountId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string AvatarFilePath { get; set; }

        public Account(long accountId, string email, string userName, string avatarFilePath)
        {
            AccountId = accountId;
            Email = email;
            UserName = userName;
            AvatarFilePath = avatarFilePath;
        }
    }
}
