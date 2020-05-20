using System;
using System.Net;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using Common.Networking.Data.Player;
using Common.Networking.Handlers.Decoder;
using Common.Networking.Handlers.Encoders;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayIt_Api.Models.Entities;
using PlayIt_Api.Services.GameType;
using PlayIt_Api.Services.ServerMediatorMaster.handler;

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
        private readonly IGameTypeService _gameInformationService;

        public GameTypeController([FromServices] IGameTypeService gameInformationService)
        {
            _gameInformationService = gameInformationService;
        }

        /// <summary>
        /// Get gameTypes from database
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(IPagedList<GameType>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var gameTypes = await _gameInformationService.GetGameTypes();
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

        //TODO REMOVE THIS
        [AllowAnonymous]
        [HttpGet("test")]
        public async Task<IActionResult> Test()

        {
            var group = new MultithreadEventLoopGroup();

            //var serverIP = IPAddress.Parse("127.0.0.1");
            var serverIP = IPAddress.Parse("127.0.0.1");
            int serverPort = 8080;

            try
            {
                var bootstrap = new Bootstrap();
                bootstrap
                    .Group(group)
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, true) // Do not buffer and send packages right away
                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;

                        pipeline.AddLast(new PlayerRequestDataEncoder(), new NetworkHandlerDecoder(),
                            new PlayerRequestDataHandler(new UnitOfWork<PlayItContext>(new PlayItContext())));
                    }));

                IChannel bootstrapChannel = await bootstrap.ConnectAsync(new IPEndPoint(serverIP, serverPort));


                var a = new PlayerRequestData(
                    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJBY2NvdW50SWQiOiIyNiIsIlVzZXJuYW1lIjoiTGFsYSIsIkVtYWlsIjoiY3BAaXRvcGVyYXRvcnMuZGsiLCJFeHBpcmVzIjoiMDUvMTkvMjAyMCAxNjozMDowMiIsIm5iZiI6MTU4OTg2OTgwMiwiZXhwIjoxNTg5ODk4NjAyLCJpYXQiOjE1ODk4Njk4MDJ9.EzmqGhBnAQymHrZsA10P7Urk9vRZQsQFWsnYzWGMv7A");

                await bootstrapChannel.WriteAndFlushAsync(a);

                return Ok("her");
            }
            finally
            {
                group.ShutdownGracefullyAsync().Wait(1000);
            }
        }
    }
}
