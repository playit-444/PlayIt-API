using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PlayIt_Api.Models.Dto;
using PlayIt_Api.Models.GameServer;

namespace PlayIt_Api.Services.Account
{
    public interface IAccountService : IDisposable
    {
        /// <summary>
        /// Create an account
        /// </summary>
        /// <param name="accountSignUp"></param>
        /// <returns>The created Account object </returns>
        /// <exception cref="ArgumentNullException"></exception>
        ValueTask<EntityEntry<Models.Entities.Account>> CreateAccount(AccountSignUp accountSignUp);

        /// <summary>
        /// Check if account username already exists
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>true false depending if user already exists</returns>
        Task<bool> AccountExists(string userName);

        /// <summary>
        /// check if account email already exists
        /// </summary>
        /// <param name="email"></param>
        /// <returns>true false depending is user already exists</returns>
        Task<bool> EmailExists(string email);

        /// <summary>
        /// Login Account
        /// </summary>
        /// <param name="accountSignIn"></param>
        /// <returns>A token linked to the account loggede in</returns>
        Task<AccountJwtToken> LoginAccount(AccountSignIn accountSignIn);

        /// <summary>
        /// VerifyAccount account with token from email
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns>The Account verified by the token</returns>
        Task<Models.Entities.Account> VerifyAccount(string tokenId);

        /// <summary>
        /// Renew Login Token
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns>The new token object created from account</returns>
        Task<AccountJwtToken> RenewLoginToken(int employeeId);

        /// <summary>
        /// Verify if the token given is legit and exists on the server.
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <returns>The token, account and username there was linked with the given token</returns>
        Task<PlayerVerificationResponse> VerifyToken(string jwtToken);

        /// <summary>
        /// Get the specific account that was queried for
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>The account object that was queried for</returns>
        Task<Models.Entities.Account> GetAccount(long accountId);

        /// <summary>
        /// Get account information from token
        /// </summary>
        /// <param name="token"></param>
        /// <returns>The account object that was queried for</returns>
        Task<Models.Entities.Account> GetAccountFromToken(string token);
    }
}
