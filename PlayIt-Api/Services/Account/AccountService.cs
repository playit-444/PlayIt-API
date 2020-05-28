using System;
using System.Data.Common;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.IdentityModel.Tokens;
using PlayIt_Api.Logging;
using PlayIt_Api.Models.Dto;
using PlayIt_Api.Models.Entities;
using PlayIt_Api.Models.GameServer;
using PlayIt_Api.Services.Mail;
using PlayIt_Api.Services.Security.Account;
using PlayIt_Api.Services.Token;

namespace PlayIt_Api.Services.Account
{
    /// <summary>
    /// Account service
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;
        private readonly IMailService _mailService;
        private readonly ILogger _logger;

        public AccountService([FromServices] IPasswordService passwordService, [FromServices] IUnitOfWork unitOfWork,
            [FromServices] ITokenService tokenService, [FromServices] IMailService mailService,
            [FromServices] ILogger logger)
        {
            _passwordService = passwordService;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _mailService = mailService;
            _logger = logger;
        }

        /// <summary>
        /// Create account
        /// </summary>
        /// <param name="accountSignUp"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<EntityEntry<Models.Entities.Account>> CreateAccount(AccountSignUp accountSignUp)
        {
            if (accountSignUp == null)
                throw new ArgumentNullException(nameof(accountSignUp));

            //Create a new password for the customer
            byte[] password = new byte[64],
                salt = new byte[32];

            EntityEntry<Models.Entities.Account> account = null;
            try
            {
                password = _passwordService.CreatePassword(accountSignUp.Password, out salt);

                //Create customer in database
                var accountRepo = _unitOfWork.GetRepository<Models.Entities.Account>();
                account = await accountRepo.InsertAsync(new Models.Entities.Account
                {
                    Created = DateTime.Now, Email = accountSignUp.Email, Password = password, Salt = salt,
                    Verified = false, UserName = accountSignUp.UserName, AvatarFilePath = ""
                });
                await _unitOfWork.SaveChangesAsync();

                //Check if created in db
                if (account != null)
                {
                    //Create Token
                    var token = await _tokenService.CreateToken(account.Entity.AccountId, 1);
                    _mailService.SendMail("444.dk - Godkendelse af bruger oprettelse",
                        "<a href = \"https://444.dk?token=" + token.Entity.TokenId + "\">Godkend</a>",
                        account.Entity.Email);
                }
            }
            catch (ArgumentNullException e)
            {
                await _logger.LogAsync(e.Message);
            }
            catch (DbException e)
            {
                await _logger.LogAsync(e.Message);
            }
            catch (Exception e)
            {
                await _logger.LogAsync(e.Message);
            }

            return account;
        }

        /// <summary>
        /// Account username exists
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<bool> AccountExists(string userName)
        {
            var accountRepo = _unitOfWork.GetRepository<Models.Entities.Account>();

            return await accountRepo.GetFirstOrDefaultAsync(predicate: e => e.UserName == userName) != null;
        }

        /// <summary>
        /// Account email exists
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<bool> EmailExists(string email)
        {
            var accountRepo = _unitOfWork.GetRepository<Models.Entities.Account>();
            return await accountRepo.GetFirstOrDefaultAsync(predicate: e => e.Email == email) != null;
        }

        /// <summary>
        /// Login Account
        /// </summary>
        /// <param name="accountSignIn"></param>
        /// <returns></returns>
        public async Task<AccountJwtToken> LoginAccount(AccountSignIn accountSignIn)
        {
            var accountRepo = _unitOfWork.GetRepository<Models.Entities.Account>();

            //Check how many login attempts ip have
            var count = await CountLoginAttempt(accountSignIn);
            //TODO Config instead of value
            if (count <= 6)
            {
                var account =
                    await accountRepo.GetFirstOrDefaultAsync(predicate: a => a.UserName == accountSignIn.UserName);
                if (account == null)
                {
                    account = await accountRepo.GetFirstOrDefaultAsync(
                        predicate: a => a.Email == accountSignIn.UserName);
                }

                if (account.Verified)
                {
                    //Compare user password with database
                    if (_passwordService.ComparePasswords(accountSignIn.Password, account.Password, account.Salt))
                    {
                        return CreateJwtToken(account);
                    }

                    //Create login attempt
                    await CreateLoginAttempt(accountSignIn);
                }

                return null;
            }

            return null;
        }

        /// <summary>
        /// Create JWTToken
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private AccountJwtToken CreateJwtToken(Models.Entities.Account account)
        {
            var role = "customer";
            if (account.AccountId == 30)
            {
                role = "gameServer";
            }

            var expire = DateTime.Now.AddDays(1);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("AccountId", account.AccountId.ToString()),
                    new Claim("Username", account.UserName),
                    new Claim("Email", account.Email),
                    new Claim("Expires", expire.ToString(CultureInfo.InvariantCulture)),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = expire,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("YdCnz8X4!dvLvtu8c&q*9JSd$BZD#^P5Wrb^PsvvJm5XfxbHW3X@8YD8D4^pe8nx")),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            return new AccountJwtToken(token);
        }

        /// <summary>
        /// Count login attempt
        /// </summary>
        /// <param name="accountSignIn"></param>
        /// <returns></returns>
        private async Task<int> CountLoginAttempt(AccountSignIn accountSignIn)
        {
            var loginAttemptRepo = _unitOfWork.GetRepository<LoginAttempt>();
            return (await loginAttemptRepo.GetPagedListAsync(a =>
                a.Ipv4 == accountSignIn.Ipv4 && a.Created > DateTime.Now.AddHours(-1))).TotalCount;
        }

        /// <summary>
        /// Create Login Attempt
        /// </summary>
        /// <param name="accountSignIn"></param>
        /// <returns></returns>
        private async Task CreateLoginAttempt(AccountSignIn accountSignIn)
        {
            var loginAttemptRepo = _unitOfWork.GetRepository<LoginAttempt>();
            await loginAttemptRepo.InsertAsync(new LoginAttempt
                {Created = DateTime.Now, Ipv4 = accountSignIn.Ipv4, UserName = accountSignIn.UserName});
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// VerifyAccount with token from email
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public async Task<Models.Entities.Account> VerifyAccount(string tokenId)
        {
            var tokenRepo = _unitOfWork.GetRepository<Models.Entities.Token>();
            var token = await tokenRepo.FindAsync(tokenId);
            if (token.Expiration < DateTime.Now) return null;
            var accountRepo = _unitOfWork.GetRepository<Models.Entities.Account>();
            var account = await accountRepo.FindAsync(token.AccountId);
            account.Verified = true;
            accountRepo.Update(account);
            await _unitOfWork.SaveChangesAsync();
            return account;
        }

        /// <summary>
        /// Renew Login Token
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public async Task<AccountJwtToken> RenewLoginToken(int employeeId)
        {
            var accountRepo = _unitOfWork.GetRepository<Models.Entities.Account>();
            var account = await accountRepo.GetFirstOrDefaultAsync(predicate: a => a.AccountId == employeeId);
            return CreateJwtToken(account);
        }

        public async Task<PlayerVerificationResponse> VerifyToken(string jwtToken)
        {
            JwtSecurityTokenHandler jwtSecurityTokenHandler;
            jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = false,
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("YdCnz8X4!dvLvtu8c&q*9JSd$BZD#^P5Wrb^PsvvJm5XfxbHW3X@8YD8D4^pe8nx"))
            };


            SecurityToken validatedToken;
            try
            {
                //Validate token if not valid it throws a error
                jwtSecurityTokenHandler.ValidateToken(jwtToken, validationParameters, out validatedToken);

                //Handler for getting token values
                var handler = new JwtSecurityTokenHandler();
                //Get token's values in claims
                var tokenS = handler.ReadToken(jwtToken) as JwtSecurityToken;

                //Get specific value from claim
                var accountId = tokenS.Claims.First(claim => claim.Type == "AccountId").Value;

                //Account repository
                var accountRepo = _unitOfWork.GetRepository<Models.Entities.Account>();
                //Account information
                var account =
                    await accountRepo.GetFirstOrDefaultAsync(predicate: e => e.AccountId == Convert.ToInt32(accountId));
                if (account != null)
                {
                    return new PlayerVerificationResponse(jwtToken, account.AccountId, account.UserName);
                }
            }
            catch (Exception)
            {
                //Logger
            }

            return new PlayerVerificationResponse(jwtToken, 0, "");
        }

        public Task<Models.Entities.Account> GetAccount(long accountId)
        {
            var accountRepo = _unitOfWork.GetRepository<Models.Entities.Account>();
            return accountRepo.GetFirstOrDefaultAsync(predicate: a => a.AccountId == accountId);
        }
        
        public Task<Models.Entities.Account> GetAccountFromToken(string token)
        {
            //var tokenRepo = _unitOfWork.GetRepository<Models.Entities.Token>();
            //var accountRepo = _unitOfWork.GetRepository<Models.Entities.Account>();
            
            
            try
            {
                //Handler for getting token values
                var handler = new JwtSecurityTokenHandler();
                //Get token's values in claims

                //Get specific value from claim
                if (handler.ReadToken(token) is JwtSecurityToken tokenS)
                {
                    var accountId = tokenS.Claims.First(claim => claim.Type == "AccountId").Value;

                    //Account repository
                    var accountRepo = _unitOfWork.GetRepository<Models.Entities.Account>();
                    //Account information
                    var account =
                        accountRepo.GetFirstOrDefaultAsync(predicate: e => e.AccountId == Convert.ToInt32(accountId));
                    if (account != null)
                    {
                        return account;
                    }
                }
            }
            catch (Exception)
            {
                //Logger
            }

            return null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.SaveChangesAsync(true);
                _unitOfWork.Dispose();
            }
        }
    }
}
