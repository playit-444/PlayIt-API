using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayIt_Api.Models.Dto;
using PlayIt_Api.Models.GameServer;
using PlayIt_Api.Services.Account;
using PlayIt_Api.Services.Security.Account;
using Account = PlayIt_Api.Models.Entities.Account;

namespace PlayIt_Api.Controllers
{
    /// <summary>
    /// Controller responsible for CRUD Account
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;

        public AccountController([FromServices] IPasswordService passwordService,
            [FromServices] IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _passwordService = passwordService;
            _mapper = mapper;
        }

        /// <summary>
        /// Create an unverified Account
        /// Send email with verification link to user
        /// </summary>
        /// <param name="accountSignUp"></param>
        /// <returns>Created user</returns>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(Response), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateAccount(AccountSignUp accountSignUp)
        {
            //Check parameters
            if (accountSignUp == null)
                return BadRequest("Udfyld alle felter");
            if (string.IsNullOrEmpty(accountSignUp.Email))
                return BadRequest("Email kan ikke være tomt");
            if (string.IsNullOrEmpty(accountSignUp.Password))
                return BadRequest("Adgangskode kan ikke være tomt");
            if (string.IsNullOrEmpty(accountSignUp.UserName))
                return BadRequest("Brugernavn kan ikke være tomt");

            //Validate Email
            if (!Regex.IsMatch(accountSignUp.Email,
                "^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")
            )
                return BadRequest("Email er ikke valid");

            //Validate Password
            if (!Regex.IsMatch(accountSignUp.Password, _passwordService.PasswordPattern))
                return BadRequest(
                    $"Adgangskoden var ikke valid, adgangskoden skal minimum indeholde 1 stortbogstav, 1 bogstav, 1 specialtegn, 1 tal og være {_passwordService.MinLength}-{_passwordService.MaxLength} tegn langt");

            //Check if account userName already exists
            if (await _accountService.AccountExists(accountSignUp.Email))
                return BadRequest("En bruger med denne brugernavn eksistere allerede");

            //Check if account email already exists
            if (await _accountService.EmailExists(accountSignUp.Email))
                return BadRequest("En bruger med denne email eksistere allerede");

            try
            {
                //Create account
                if (await _accountService.CreateAccount(accountSignUp) != null)
                {
                    //If successfully send ok created
                    return Ok(new Response("Oprettet bruger!"));
                }
            }
            catch (Exception)
            {
                return BadRequest(
                    "Der skete en fejl ved oprettelse af brugere, kontakt venligst Support@444.dk hvis dette fortsætter");
            }

            return BadRequest(
                "Der skete en fejl ved oprettelse af brugere, kontakt venligst Support@444.dk hvis dette fortsætter");
        }

        /// <summary>
        /// Return an specific account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>Account</returns>
        [HttpGet("{accountId}")]
        [ProducesResponseType(typeof(Models.Dto.Account), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAccount(long accountId)
        {
            //Check if accountId is not 0
            if (accountId == 0)
                return BadRequest("Email/Brugernavn eller password er forkert");
            //Return account
            return Ok(_mapper.Map<Account, Models.Dto.Account>(await _accountService.GetAccount(accountId)));
        }

        /// <summary>
        /// Get account from token
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Account</returns>
        [HttpGet("token/{token}")]
        [ProducesResponseType(typeof(Models.Dto.Account), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAccountFromToken(string token)
        {
            //Check if token is null
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token var enten tom eller ikke sat");
            //Return account
            return Ok(_mapper.Map<Account, Models.Dto.Account>(await _accountService.GetAccountFromToken(token)));
        }

        /// <summary>
        /// Login account
        /// </summary>
        /// <param name="accountSignIn"></param>
        /// <returns>Account</returns>
        [AllowAnonymous]
        [HttpPost("signin")]
        [ProducesResponseType(typeof(AccountJwtToken), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SigninAccount(AccountSignIn accountSignIn)
        {
            //Check parameters
            if (accountSignIn == null)
                return BadRequest("Email/Brugernavn eller password er forkert");
            if (string.IsNullOrEmpty(accountSignIn.UserName))
                return BadRequest("Email/Brugernavn eller password er forkert");
            if (string.IsNullOrEmpty(accountSignIn.Password))
                return BadRequest("Email/Brugernavn eller password er forkert");
            if (string.IsNullOrEmpty(accountSignIn.Ipv4))
                return BadRequest("IP Fejl, kontakt venligst Support@444.dk hvis dette fortsætter");

            try
            {
                //Try to login
                var jwtToken = await _accountService.LoginAccount(accountSignIn);
                if (jwtToken != null)
                    return Ok(jwtToken);
            }
            catch (Exception)
            {
                return BadRequest(
                    "Der skete en fejl ved log ind, kontakt venligst Support@444.dk hvis dette fortsætter");
            }

            return BadRequest("Der skete en fejl ved log ind, kontakt venligst Support@444.dk hvis dette fortsætter");
        }

        /// <summary>
        /// Verify token for account
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns>True</returns>
        [AllowAnonymous]
        [HttpGet("verify/{tokenId}")]
        [ProducesResponseType(typeof(Response), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Verify(string tokenId)
        {
            //Check parameters
            if (string.IsNullOrEmpty(tokenId))
                return BadRequest("tokenId blev ikke fundet");

            try
            {
                //Verify account
                var account = await _accountService.VerifyAccount(tokenId);
                if (account != null)
                {
                    return Ok(new Response("true"));
                }
            }
            catch (Exception)
            {
                return BadRequest("Der skete en fejl under verificering af token");
            }

            return BadRequest("Der skete en fejl under verificering af token");
        }

        /// <summary>
        /// Renew JwtToken
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>JwtToken</returns>
        [HttpGet("renew/{accountId}")]
        [ProducesResponseType(typeof(AccountJwtToken), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RenewToken(int accountId)
        {
            //Check parameters
            if (accountId == 0)
                return BadRequest("AccountId blev ikke fundet");
            try
            {
                //Renew JwtToken
                AccountJwtToken token = await _accountService.RenewLoginToken(accountId);
                if (token != null)
                    return Ok(token);
                return BadRequest("Der skete en fejl under fornyelse af token");
            }
            catch (Exception)
            {
                return BadRequest("Der skete en fejl under fornyelse af token");
            }
        }

        /// <summary>
        /// Verify token for account
        /// </summary>
        /// <param name="playerJwtTokenModel"></param>
        /// <returns>Account</returns>
        [AllowAnonymous]
        [HttpPost("verifyToken")]
        [ProducesResponseType(typeof(PlayerVerificationResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> VerifyToken(PlayerJwtTokenModel playerJwtTokenModel)
        {
            //Check parameters
            if (playerJwtTokenModel == null)
                return BadRequest("Token blev ikke fundet");
            if (playerJwtTokenModel.JwtToken == null)
                return BadRequest("Token blev ikke fundet");

            try
            {
                //Check if account is logged in
                var account = await _accountService.VerifyToken(playerJwtTokenModel.JwtToken);
                if (account != null)
                {
                    return Ok(account);
                }
            }
            catch (Exception)
            {
                return BadRequest("Der skete en fejl under verificering af token");
            }

            return BadRequest("Der skete en fejl under verificering af token");
        }
    }
}
