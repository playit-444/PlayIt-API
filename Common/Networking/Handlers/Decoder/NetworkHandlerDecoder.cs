﻿﻿using System;
using System.Collections.Generic;
using Common.Networking.Data.Game;
using Common.Networking.Data.Player;
using Common.Networking.Data.Room;
using Common.Networking.Data.Server;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Common.Networking.Handlers.Decoder
{
    public class NetworkHandlerDecoder : MessageToMessageDecoder<IByteBuffer>
    {
        public override bool IsSharable => true;

        protected override void Decode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
        {
            string[] listings = message.ToString(System.Text.Encoding.UTF8).Split(Environment.NewLine);

            if (listings.Length == 0)
                return;

            int id;
            int.TryParse(listings[0], out id);

            if (id.Equals(0))
                return;

            for (int i = listings.Length > 1 ? 1 : 0; i < listings.Length; i++)
            {
                string[] item = listings[i].Split(':');
                switch (id)
                {
                    case 1:
                        Guid tempGuid1;
                        if (item.Length >= 1 && Guid.TryParse(item[0], out tempGuid1))
                            output.Add(new GameServerData(tempGuid1));
                        break;
                    case 2:
                        Guid tempGuid2;
                        if (item.Length >= 2 && Guid.TryParse(item[0], out tempGuid2))
                            output.Add(new GameServerEventData(item[1], tempGuid2));
                        break;
                    case 3:
                        long playerId;
                        if (item.Length >= 3 && long.TryParse(item[1], out playerId))
                            if (!string.IsNullOrEmpty(item[0]))
                                output.Add(new PlayerData(item[0], playerId, item[2]));
                        break;
                    case 4:
                        if (!string.IsNullOrEmpty(item[0]))
                            output.Add(new PlayerRequestData(item[0]));
                        break;
                    case 5:
                        int current, maxPlayers, gametypeid;
                        bool pri;

                        if (int.TryParse(item[2], out maxPlayers) &&
                            int.TryParse(item[3], out current) &&
                            int.TryParse(item[5], out gametypeid) &&
                            bool.TryParse(item[4], out pri))
                        {
                            output.Add(new GameRoomData(item[0], item[1], maxPlayers, current, pri, gametypeid));
                        }
                        break;
                    case 10:
                        if (item.Length >= 3)
                        {
                            int gameTypeId;
                            byte min, max;
                            if (
                                int.TryParse(item[0], out gameTypeId) &&
                                byte.TryParse(item[1], out min) &&
                                byte.TryParse(item[2], out max))
                            {
                                output.Add(new GameTypeData(gameTypeId, min, max));
                            }
                        }
                        break;
                    case 11:
                        output.Add(new GameTypeRequestData());
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
