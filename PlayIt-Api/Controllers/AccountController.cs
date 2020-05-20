using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayIt_Api.Models.Dto;
using PlayIt_Api.Services.Account;
using PlayIt_Api.Services.Security.Account;

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
        public AccountController([FromServices] IPasswordService passwordService,
            [FromServices] IAccountService accountService)
        {
            _accountService = accountService;
            _passwordService = passwordService;
        }

        /// <summary>
        /// Create a Account
        /// </summary>
        /// <param name="accountSignUp"></param>
        /// <returns></returns>
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

            //Check if customer userName exists
            if (await _accountService.AccountExists(accountSignUp.Email))
                return BadRequest("En bruger med denne brugernavn eksistere allerede");

            //Check if customer email exists
            if (await _accountService.EmailExists(accountSignUp.Email))
                return BadRequest("En bruger med denne email eksistere allerede");

            try
            {
                //Create account
                if (await _accountService.CreateAccount(accountSignUp) != null)
                {
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
        /// Login account
        /// </summary>
        /// <param name="accountSignIn"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("signin")]
        [ProducesResponseType(typeof(JwtToken), (int) HttpStatusCode.OK)]
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
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("verify/{tokenId}")]
        [ProducesResponseType(typeof(Response), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Verify(string tokenId)
        {
            if (tokenId == null)
                return BadRequest("Token blev ikke fundet");
            try
            {
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
        /// <returns></returns>
        [HttpGet("renew/{accountId}")]
        [ProducesResponseType(typeof(JwtToken), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RenewToken(int accountId)
        {
            try
            {
                JwtToken token = await _accountService.RenewLoginToken(accountId);
                return Ok(token);
            }
            catch (Exception)
            {
                return BadRequest("Der skete en fejl under fornyelse af token");
            }
        }
    }
}
