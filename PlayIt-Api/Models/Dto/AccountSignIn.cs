namespace PlayIt_Api.Models.Dto
{
    public class AccountSignIn
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Ipv4 { get; set; }

        public AccountSignIn(string userName, string password, string ipv4)
        {
            UserName = userName;
            Password = password;
            Ipv4 = ipv4;
        }
    }
}
