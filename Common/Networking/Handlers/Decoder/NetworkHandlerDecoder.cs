using System;
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
            int id;

            if (listings.Length == 0)
                return;

            if (listings.Length == 1)
            {
                string[] item = listings[0].Split(':');
                if (item.Length > 0 && int.TryParse(item[0], out id))
                    switch (id)
                    {
                        case 1:
                            Guid tempGuid1;
                            if (item.Length >= 2 && Guid.TryParse(item[1], out tempGuid1))
                                output.Add(new GameServerData(tempGuid1));
                            break;
                        case 2:
                            Guid tempGuid2;
                            if (item.Length >= 3 && Guid.TryParse(item[1], out tempGuid2))
                                output.Add(new GameServerEventData(item[2], tempGuid2));
                            break;
                        case 3:
                            long playerId;
                            if (item.Length >= 3 && long.TryParse(item[1], out playerId))
                                if (!string.IsNullOrEmpty(item[0]))
                                    output.Add(new PlayerData(item[0], playerId, item[2]));
                            break;
                        case 4:
                            if (!string.IsNullOrEmpty(item[1]))
                                output.Add(new PlayerRequestData(item[1]));
                            break;
                        case 10:
                            if (item.Length >= 4)
                            {
                                int gameTypeId;
                                byte min, max;
                                if (
                                    int.TryParse(item[1], out gameTypeId) &&
                                    byte.TryParse(item[2], out min) &&
                                    byte.TryParse(item[3], out max))
                                {
                                    output.Add(new GameTypeData(gameTypeId, min, max));
                                }
                            }
                            break;
                        default:
                            break;
                    }
            }
            else
            {
                int.TryParse(listings[0], out id);
                switch (id)
                {
                    //TODO: single object lists
                    default:
                        break;
                }
            }
        }
    }
}
