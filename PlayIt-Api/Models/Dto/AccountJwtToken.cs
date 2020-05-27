namespace PlayIt_Api.Models.Dto
{
    public class AccountJwtToken
    {
        public string JwtToken { get; set; }

        public AccountJwtToken(string jwtToken)
        {
            JwtToken = jwtToken;
        }
    }
}
