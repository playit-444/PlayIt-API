using System;
using System.Collections.Generic;
using Arch.EntityFrameworkCore.UnitOfWork;
using Common.Networking.Data.Room;
using Common.Networking.Handlers.Decoder;
using Common.Networking.Handlers.Encoders;
using Common.Networking.Mediation.Master;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using PlayIt_Api.Logging;
using PlayIt_Api.Models.Entities;
using PlayIt_Api.Services.ServerMediatorMaster.handler;

namespace PlayIt_Api.Services.ServerMediatorMaster
{
    public class ServerMediatorMaster : IGameServerMediatorMaster
    {
        public Guid ServerID { get; }
        public int Port { get; }
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        public IDictionary<Guid, IChannel> ServerChannels { get; }

        /// <summary>
        /// GameTypes from db saved in Dictionary to save lookup to database
        /// </summary>
        private static IDictionary<int, string> gameTypes;

        // Public GameTypes
        public IDictionary<int, string> GameTypes
        {
            get => gameTypes;
            set => gameTypes = value;
        }

        //GameTypes from db
        private void GetGameTypesFromDb()
        {
            var gameTypesRepo = _unitOfWork.GetRepository<Models.Entities.GameType>();
            var gameTypesList = gameTypesRepo.GetPagedList();
            foreach (var gameType in gameTypesList.Items)
            {
                gameTypes.Add(gameType.GameTypeId, gameType.Name);
            }
        }

        public static IDictionary<int, string> GetGameTypes()
        {
            return gameTypes;
        }

        /// <summary>
        /// Dictionary of lobby
        /// </summary>

        // Static ServerRooms (RoomData/GameRoomData)
        private static IDictionary<Guid, HashSet<IRoomData>> serverRooms;

        // Public ServerRooms (RoomData/GameRoomData)
        public IDictionary<Guid, HashSet<IRoomData>> ServerRooms
        {
            get => serverRooms;
            set => serverRooms = value;
        }

        // Get Server Rooms
        public static IDictionary<Guid, HashSet<IRoomData>> GetServerRooms()
        {
            return serverRooms;
        }

        // Add Server Room
        public static void AddServerRooms(Guid guid, IRoomData roomData)
        {
            if (serverRooms.ContainsKey(guid))
            {
                lock (serverRooms)
                {
                    serverRooms[guid].Add(roomData);
                }
            }
            else
            {
                lock (serverRooms)
                {
                    serverRooms.Add(guid, new HashSet<IRoomData>());
                    serverRooms[guid].Add(roomData);
                }
            }
        }

        /// <summary>
        /// Dictionary of game servers
        /// </summary>

        // Static Connected GameServer
        private static IDictionary<IChannelId, Guid> servers;

        // Public Connected GameServer
        public IDictionary<IChannelId, Guid> ConnectedServers
        {
            get => servers;
            set => servers = value;
        }

        // Get GameServer by channelId
        public static Guid GetGameServerGuid(IChannelId channelId)
        {
            if (servers.ContainsKey(channelId))
            {
                return servers[channelId];
            }

            return Guid.Empty;
        }

        // Add GameServer
        public static void AddGameServer(Guid guid, IChannelId channelId)
        {
            if (!servers.ContainsKey(channelId))
            {
                lock (servers)
                {
                    servers.Add(channelId, guid);
                }
            }
        }

        // Remove GameServer
        public static void RemoveGameServer(Guid guid, IChannelId channelId)
        {
            if (servers.ContainsKey(channelId))
            {
                lock (servers)
                {
                    //servers[channelId].DisconnectAsync();
                    //servers[channelId].CloseAsync();
                    servers.Remove(channelId);
                }
            }
        }

        public ServerMediatorMaster()
        {
            Port = 8282;
            ServerID = Guid.NewGuid();
            GameTypes = new Dictionary<int, string>();
            servers = new Dictionary<IChannelId, Guid>();
            serverRooms = new Dictionary<Guid, HashSet<IRoomData>>();

            _unitOfWork = new UnitOfWork<PlayItContext>(new PlayItContext());
            _logger = new DbExceptionLogger(_unitOfWork);
            StartServer();
            GetGameTypesFromDb();
        }

        private async void StartServer()
        {
            var bossGroup = new MultithreadEventLoopGroup(1); //  accepts an incoming connection
            var workerGroup = new MultithreadEventLoopGroup();
            var bootstrap = new ServerBootstrap();

            try
            {
                bootstrap
                    .Group(bossGroup, workerGroup)
                    .Channel<TcpServerSocketChannel>()
                    .Option(ChannelOption.SoBacklog, 100) // maximum queue length for incoming connection
                    .Option(ChannelOption.SoRcvbuf, 8192)
                    .Option(ChannelOption.SoSndbuf, 8192)
                    .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;
                        //Decoder
                        pipeline.AddLast(new NetworkHandlerDecoder());

                        //New GameServer connect to API
                        pipeline.AddLast(new GameServerDataEncoder(), new GameServerHandler(ServerID));

                        //Game ServerEventData (Stop/restart)
                        //pipeline.AddLast(new GameServerEventDataEncoder(), new GameServerEventHandler());

                        //If gameServer request gameType
                        pipeline.AddLast(new GameTypeRequestDataEncoder(), new GameTypeDataEncoder(),
                            new GameTypeRequestHandler(_unitOfWork));

                        //Player RequestData (Validate if user is logged in, and return user information to GameServer
                        pipeline.AddLast(new PlayerRequestDataEncoder(),
                            new PlayerRequestDataHandler(_unitOfWork));

                        //Game RoomData
                        pipeline.AddLast(new GameRoomDataEncoder(), new GameRoomDataHandler());
                    }));
                await bootstrap.BindAsync(Port);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
