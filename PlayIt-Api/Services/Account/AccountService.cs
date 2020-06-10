using System;
using System.Data.Common;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
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

        public async ValueTask<EntityEntry<Models.Entities.Account>> CreateAccount(AccountSignUp accountSignUp)
        {
            if (accountSignUp == null)
                throw new ArgumentNullException(nameof(accountSignUp));

            if (string.IsNullOrEmpty(accountSignUp.UserName))
                throw new ArgumentNullException(accountSignUp.UserName);

            if (string.IsNullOrEmpty(accountSignUp.Email))
                throw new ArgumentNullException(accountSignUp.Email);

            if (string.IsNullOrEmpty(accountSignUp.Password))
                throw new ArgumentNullException(accountSignUp.Password);

            //Create a new password for the customer
            byte[] password = new byte[64],
                salt = new byte[32];

            EntityEntry<Models.Entities.Account> account;
            //The avatar path
            string avatarPath = SaveAvatar(accountSignUp.Avatar, accountSignUp.UserName);

            try
            {
                //Create account password
                password = _passwordService.CreatePassword(accountSignUp.Password, out salt);

                //Create customer in database
                var accountRepo = _unitOfWork.GetRepository<Models.Entities.Account>();
                account = await accountRepo.InsertAsync(new Models.Entities.Account
                {
                    Created = DateTime.Now, Email = accountSignUp.Email, Password = password, Salt = salt,
                    Verified = false, UserName = accountSignUp.UserName, AvatarFilePath = avatarPath
                });
                await _unitOfWork.SaveChangesAsync();

                //Check if created in db
                if (account != null)
                {
                    //Create Token
                    var token = await _tokenService.CreateToken(account.Entity.AccountId, 1);
                    //Send mail to user with verification token
                    _mailService.SendMail("444.dk - Godkendelse af bruger oprettelse",
                        "<!DOCTYPE html><html>    <head>        <style>            .logo {                text-align: center;            }            .main {                text-align: center;            }            .footer {                text-align: center;            }            .btn {                display: inline-block;                font-weight: 400;                color: #212529;                text-align: center;                vertical-align: middle;                -webkit-user-select: none;                -moz-user-select: none;                -ms-user-select: none;                user-select: none;                background-color: transparent;                border: 1px solid transparent;                padding: .375rem .75rem;                font-size: 1rem;                line-height: 1.5;                border-radius: .25rem;                transition: color .15s ease-in-out,background-color .15s ease-in-out,border-color .15s ease-in-out,box-shadow .15s ease-in-out;            }            .btn-success {                color: #fff;                background-color: #28a745;                border-color: #28a745;            }            .footer span {                color: #777;            }        </style>    </head>    <body>        <div class=\"container\">            <div class=\"logo\">                <img src=\"https://image.444.dk/logo/logo-without-banner.png\" alt=\"444.dk Logo\" width=\"180\" height=\"140\">            </div>            <br>            <div class=\"main\">                <span>Tryk på godkend herunder for at verificere din email</span><br>                <a class=\"btn btn-success\" href=\"https://444.dk?token=" +
                        token.Entity.TokenId +
                        "\">Godkend</a>            </div>            <br>            <div class=\"footer\">                <span>Har du ikke oprettet dig på 444.dk?</span><br>                <span>Venligst ignorer denne mail.</span>            </div>        </div>    </body></html>",
                        account.Entity.Email);
                }
            }
            catch (ArgumentNullException e)
            {
                await _logger.LogAsync(e.Message);
                throw;
            }
            catch (DbException e)
            {
                await _logger.LogAsync(e.Message);
                throw;
            }
            catch (Exception e)
            {
                await _logger.LogAsync(e.Message);
                throw;
            }

            return account;
        }

        /// <summary>
        /// Creates a folder and
        /// </summary>
        /// <param name="avatar"></param>
        /// <param name="username"></param>
        /// <returns>Account Avatar Path</returns>
        private string SaveAvatar(string avatar, string username)
        {
            try
            {
                // Splits the string to get the image format
                string format = avatar.Split("/")[1].Split(';')[0];

                // Path to user folder
                string path = $@"C:\inetpub\wwwroot\image.444.dk\players\{username}";

                // Splits the string to get the only the image without the headers
                var bytes = Convert.FromBase64String(avatar.Split(',')[1]);

                // Creates a new directory if it doesn't exist.
                Directory.CreateDirectory(path);

                // Saves the image in the newly creates folder.
                File.WriteAllBytes(Path.Combine(path, $"Avatar.{format}"), bytes);

                // Returns the url path to the image.
                return $"https://image.444.dk/players/{username}/Avatar.{format}";
            }
            catch (Exception)
            {
                return $"https://image.444.dk/players/no-image/no-user-image.png";
            }
        }

        public async Task<bool> AccountExists(string userName)
        {
            var accountRepo = _unitOfWork.GetRepository<Models.Entities.Account>();

            return await accountRepo.GetFirstOrDefaultAsync(predicate: e => e.UserName == userName) != null;
        }

        public async Task<bool> EmailExists(string email)
        {
            var accountRepo = _unitOfWork.GetRepository<Models.Entities.Account>();
            return await accountRepo.GetFirstOrDefaultAsync(predicate: e => e.Email == email) != null;
        }

        public async Task<AccountJwtToken> LoginAccount(AccountSignIn accountSignIn)
        {
            var accountRepo = _unitOfWork.GetRepository<Models.Entities.Account>();

            //Check how many login attempts ip have
            var count = await CountLoginAttempt(accountSignIn);
            //TODO Config instead of value
            if (count <= 6)
            {
                //Get account by username
                var account =
                    await accountRepo.GetFirstOrDefaultAsync(predicate: a => a.UserName == accountSignIn.UserName);
                if (account == null)
                {
                    //If account did not exists try to get with email
                    account = await accountRepo.GetFirstOrDefaultAsync(
                        predicate: a => a.Email == accountSignIn.UserName);
                }

                //Check if account is verified
                if (account.Verified)
                {
                    //Compare account password with database
                    if (_passwordService.ComparePasswords(accountSignIn.Password, account.Password, account.Salt))
                    {
                        //Return the created token for account
                        return CreateJwtToken(account);
                    }
                }

                //Create login attempt
                await CreateLoginAttempt(accountSignIn);
                return null;
            }

            //Create login attempt
            await CreateLoginAttempt(accountSignIn);
            return null;
        }

        /// <summary>
        /// Create JWTToken
        /// </summary>
        /// <param name="account"></param>
        /// <returns>The token object that was created from account</returns>
        private AccountJwtToken CreateJwtToken(Models.Entities.Account account)
        {
            //Check if customer or gameServer login in.
            var role = "customer";
            if (account.AccountId == 30)
            {
                role = "gameServer";
            }

            //Expire now +1 day
            var expire = DateTime.Now.AddDays(1);
            //Create token with account information
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
                    //TODO move encryption key to config file
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("YdCnz8X4!dvLvtu8c&q*9JSd$BZD#^P5Wrb^PsvvJm5XfxbHW3X@8YD8D4^pe8nx")),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            //Return the created token
            return new AccountJwtToken(token);
        }

        /// <summary>
        /// Count login attempt for the specific user
        /// </summary>
        /// <param name="accountSignIn"></param>
        /// <returns>A count of tried login attempts</returns>
        private async Task<int> CountLoginAttempt(AccountSignIn accountSignIn)
        {
            var loginAttemptRepo = _unitOfWork.GetRepository<LoginAttempt>();
            return (await loginAttemptRepo.GetPagedListAsync(a =>
                a.Ipv4 == accountSignIn.Ipv4 && a.Created > DateTime.Now.AddHours(-1))).TotalCount;
        }

        /// <summary>
        /// Create a new Login Attempt
        /// </summary>
        /// <param name="accountSignIn"></param>
        private async Task CreateLoginAttempt(AccountSignIn accountSignIn)
        {
            var loginAttemptRepo = _unitOfWork.GetRepository<LoginAttempt>();
            await loginAttemptRepo.InsertAsync(new LoginAttempt
                {Created = DateTime.Now, Ipv4 = accountSignIn.Ipv4, UserName = accountSignIn.UserName});
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Models.Entities.Account> VerifyAccount(string tokenId)
        {
            var tokenRepo = _unitOfWork.GetRepository<Models.Entities.Token>();
            var token = await tokenRepo.FindAsync(tokenId);
            //Check if token is expired
            if (token.Expiration < DateTime.Now) return null;
            var accountRepo = _unitOfWork.GetRepository<Models.Entities.Account>();
            //Find the queried account
            var account = await accountRepo.FindAsync(token.AccountId);
            account.Verified = true;
            accountRepo.Update(account);
            await _unitOfWork.SaveChangesAsync();
            return account;
        }

        public async Task<AccountJwtToken> RenewLoginToken(int employeeId)
        {
            var accountRepo = _unitOfWork.GetRepository<Models.Entities.Account>();
            var account = await accountRepo.GetFirstOrDefaultAsync(predicate: a => a.AccountId == employeeId);
            return CreateJwtToken(account);
        }

        public async Task<PlayerVerificationResponse> VerifyToken(string jwtToken)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
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
            catch (Exception e)
            {
                await _logger.LogAsync(e.ToString());
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
            catch (Exception e)
            {
                _logger.LogAsync(e.ToString());
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
                _logger.Dispose();
            }
        }
    }
}
