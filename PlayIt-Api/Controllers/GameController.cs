using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayIt_Api.Models.Dto;
using PlayIt_Api.Services.Game;

namespace PlayIt_Api.Controllers
{
    /// <summary>
    /// Controller responsible for CRUD Account
    /// </summary>
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController([FromServices] IGameService gameService)
        {
            _gameService = gameService;
        }

        /// <summary>
        /// Get game
        /// </summary>
        /// <returns></returns>
        [HttpGet("{gameTypeId}")]
        [ProducesResponseType(typeof(IList<Game>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public IActionResult Get(int gameTypeId)
        {
            try
            {
                var serverRooms = _gameService.GetGameByType(gameTypeId);
                if (serverRooms != null)
                {
                    return Ok(serverRooms);
                }

                return NotFound(new Response("Der findes ingen lobbys"));
            }
            catch (Exception)
            {
                return BadRequest("an error occured whilst trying to get all games");
            }
        }
    }
}
