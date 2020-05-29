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
        /// Create Account
        /// </summary>
        /// <param name="accountSignUp"></param>
        /// <returns></returns>
        ValueTask<EntityEntry<Models.Entities.Account>> CreateAccount(AccountSignUp accountSignUp);

        /// <summary>
        /// Account username exists
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<bool> AccountExists(string userName);

        /// <summary>
        /// Account email exists
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<bool> EmailExists(string email);

        /// <summary>
        /// Login account exists
        /// </summary>
        /// <param name="accountSignIn"></param>
        /// <returns></returns>
        Task<AccountJwtToken> LoginAccount(AccountSignIn accountSignIn);

        /// <summary>
        /// Verify Account
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        Task<Models.Entities.Account> VerifyAccount(string tokenId);

        /// <summary>
        /// Renew Login Token
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        Task<AccountJwtToken> RenewLoginToken(int employeeId);

        /// <summary>
        /// Verify if token is valid
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        Task<PlayerVerificationResponse> VerifyToken(string jwtToken);

        /// <summary>
        /// Get Account from
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<Models.Entities.Account> GetAccount(long accountId);

        /// <summary>
        /// Get account information from token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Models.Entities.Account> GetAccountFromToken(string token);
    }
}
