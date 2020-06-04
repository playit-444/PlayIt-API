using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlayIt_Api.Models.Dto;
using PlayIt_Api.Models.GameServer;
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
        private readonly IHttpContextAccessor _accessor;


        public GameController([FromServices] IGameService gameService, IHttpContextAccessor accessor)
        {
            _gameService = gameService;
            _accessor = accessor;
        }

        /// <summary>
        /// GameServer created a new GameRoom
        /// Saved in static list
        /// </summary>
        /// <param name="roomData"></param>
        /// <returns></returns>
        // TODO Enable security
        [HttpPost]
        [ProducesResponseType(typeof(RoomData), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public IActionResult CreateGameLobby(RoomData roomData)
        {
            //Check parameters
            if (roomData == null)
                return BadRequest("RoomData blev ikke fundet");
            if (string.IsNullOrEmpty(roomData.Name))
                return BadRequest("Navn blev ikke fundet");
            if (string.IsNullOrEmpty(roomData.RoomID))
                return BadRequest("RoomId blev ikke fundet");
            if (roomData.MaxUsers == 0)
                return BadRequest("Maks brugere kan ikke være 0");
            if (roomData.GameType == 0)
                return BadRequest("Gametype kan ikke være 0");

            try
            {
                //Get authentication token from server
                var serverId = _accessor.HttpContext.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(serverId))
                {
                    return BadRequest("ServerId blev ikke fundet");
                }

                _gameService.AddRoomData(serverId, roomData);
                return Ok(roomData);
            }
            catch (Exception)
            {
                return BadRequest("An error occured while trying to add roomData");
            }
        }

        /// <summary>
        /// Update GameLobby data with data from gameServer
        /// </summary>
        /// <param name="roomData"></param>
        /// <returns></returns>
        // TODO Enable security
        [HttpPut]
        [ProducesResponseType(typeof(IRoomData), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public IActionResult UpdateGameLobby(RoomData roomData)
        {
            //Check parameters
            if (roomData == null)
                return BadRequest("RoomData blev ikke fundet");
            if (string.IsNullOrEmpty(roomData.Name))
                return BadRequest("Navn blev ikke fundet");
            if (string.IsNullOrEmpty(roomData.RoomID))
                return BadRequest("RoomId blev ikke fundet");
            if (roomData.MaxUsers == 0)
                return BadRequest("Maks brugere kan ikke være 0");
            if (roomData.GameType == 0)
                return BadRequest("Gametype kan ikke være 0");

            try
            {
                var serverId = _accessor.HttpContext.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(serverId))
                {
                    return BadRequest("ServerId blev ikke fundet");
                }

                var response = _gameService.UpdateRoomData(serverId, roomData);
                if (response)
                    return Ok(roomData);
                return BadRequest("An error occured while trying to update roomData");
            }
            catch (Exception)
            {
                return BadRequest("An error occured while trying to update roomData");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="roomData"></param>
        /// <returns></returns>
        // TODO Enable security
        [HttpDelete("{roomId}")]
        [ProducesResponseType(typeof(RoomData), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public IActionResult DeleteGameLobby(string roomId)
        {
            if (string.IsNullOrEmpty(roomId))
                return BadRequest("RoomId blev ikke fundet");

            try
            {
                var serverId = _accessor.HttpContext.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(serverId))
                {
                    return BadRequest("ServerId blev ikke fundet");
                }

                var response = _gameService.RemoveRoomData(serverId, roomId);
                if (response)
                    return Ok("Success");
                return BadRequest("An error occured while trying to remove roomData");
            }
            catch (Exception)
            {
                return BadRequest("An error occured while trying to remove roomData");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="gameTypeId"></param>
        /// <returns></returns>
        [HttpGet("{gameTypeId}")]
        [ProducesResponseType(typeof(IList<RoomData>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public IActionResult GetGameType(int gameTypeId)
        {
            if (gameTypeId == 0)
                return BadRequest("GameType blev ikke fundet");

            try
            {
                var serverRooms = _gameService.GetGameByType(gameTypeId);
                if (serverRooms != null && serverRooms.Count != 0)
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


        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [HttpGet("CountPlayers")]
        [ProducesResponseType(typeof(IList<GamePlayerCount>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public IActionResult GetCountGameType()
        {
            try
            {
                return Ok(_gameService.GetGamePlayerCountByGameType());
            }
            catch (Exception)
            {
                return BadRequest("an error occured whilst trying to get all games");
            }
        }

        [HttpDelete("serverClose")]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public IActionResult DeleteAllLobbyForSpecificGameServer()
        {
            try
            {
                var serverId = _accessor.HttpContext.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(serverId))
                {
                    return BadRequest("ServerId blev ikke fundet");
                }

                var response = _gameService.CloseServer(serverId);
                if (response)
                    return Ok("Success");
                return BadRequest("An error occured while trying to remove roomData");
            }
            catch (Exception)
            {
                return BadRequest("An error occured while trying to remove roomData");
            }
        }
    }
}
