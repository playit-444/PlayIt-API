using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayIt_Api.Models.Entities;
using PlayIt_Api.Models.GameServer;
using PlayIt_Api.Services.GameType;

namespace PlayIt_Api.Controllers
{
    /// <summary>
    /// Controller responsible for CRUD Account
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GameTypeController : ControllerBase
    {
        private readonly IGameTypeService _gameTypeService;

        public GameTypeController([FromServices] IGameTypeService gameTypeService)
        {
            _gameTypeService = gameTypeService;
        }

        /// <summary>
        /// Get gameTypes from database
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(IPagedList<GameType>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetGameTypes()
        {
            try
            {
                var gameTypes = await _gameTypeService.GetGameTypes();
                if (gameTypes != null)
                {
                    return Ok(gameTypes);
                }
            }
            catch (Exception)
            {
                return BadRequest("an error occured whilst trying to get all gameTypes");
            }

            return BadRequest("an error occured whilst getting gameTypes");
        }

        /// <summary>
        /// Get gameTypes id and name from database used for gameServer
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("Simple")]
        [ProducesResponseType(typeof(ICollection<GameTypeSimple>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetGameTypeSimple()
        {
            try
            {
                var gameTypeSimples = await _gameTypeService.GetGameTypesSimple();
                if (gameTypeSimples != null)
                {
                    return Ok(gameTypeSimples);
                }
            }
            catch (Exception)
            {
                return BadRequest("an error occured whilst trying to get all gameTypes");
            }

            return BadRequest("an error occured whilst getting gameTypes");
        }

        /// <summary>
        /// Get specific gameType
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{gameTypeId}")]
        [ProducesResponseType(typeof(GameType), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetGameType(int gameTypeId)
        {
            if (gameTypeId == 0)
                return BadRequest("GameTypeId blev ikke fundet");

            try
            {
                var gameTypes = await _gameTypeService.GetGameType(gameTypeId);
                if (gameTypes != null)
                {
                    return Ok(gameTypes);
                }
            }
            catch (Exception)
            {
                return BadRequest("an error occured whilst trying to get all gameTypes");
            }

            return BadRequest("an error occured whilst getting gameTypes");
        }
    }
}
