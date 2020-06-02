namespace PlayIt_Api.Models.Dto
{
    public class AccountSignUp
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Avatar { get; set; }

        public AccountSignUp(string email, string userName, string password, string avatar)
        {
            Email = email;
            UserName = userName;
            Password = password;
            Avatar = avatar;
        }
    }
}
