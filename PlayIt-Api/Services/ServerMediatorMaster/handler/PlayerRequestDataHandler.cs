using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Arch.EntityFrameworkCore.UnitOfWork;
using Common.Networking.Data.Player;
using DotNetty.Transport.Channels;
using Microsoft.IdentityModel.Tokens;

namespace PlayIt_Api.Services.ServerMediatorMaster.handler
{
    public class PlayerRequestDataHandler : SimpleChannelInboundHandler<IPlayerRequestData>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        TokenValidationParameters _validationParameters = new TokenValidationParameters()
        {
            ValidateLifetime = false,
            ValidateAudience = false,
            ValidateIssuer = false,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("YdCnz8X4!dvLvtu8c&q*9JSd$BZD#^P5Wrb^PsvvJm5XfxbHW3X@8YD8D4^pe8nx"))
        };

        public PlayerRequestDataHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }

        protected override async void ChannelRead0(IChannelHandlerContext ctx, IPlayerRequestData msg)
        {
            SecurityToken validatedToken;
            IPrincipal principal;
            try
            {
                //Validate token if not valid it throws a error
                principal =
                    _jwtSecurityTokenHandler.ValidateToken(msg.SessionKey, _validationParameters, out validatedToken);

                //Handler for getting token values
                var handler = new JwtSecurityTokenHandler();
                //Get token's values in claims
                var tokenS = handler.ReadToken(msg.SessionKey) as JwtSecurityToken;

                //Get specific value from claim
                var accountId = tokenS.Claims.First(claim => claim.Type == "AccountId").Value;

                //Account repository
                var accountRepo = _unitOfWork.GetRepository<Models.Entities.Account>();
                //Account information
                var account =
                    accountRepo.GetFirstOrDefault(predicate: e => e.AccountId == Convert.ToInt32(accountId));
                if (account != null)
                {
                    //If account is not null send message to client
                    await ctx.Channel.WriteAndFlushAsync(new PlayerData(msg.SessionKey, account.AccountId, account.UserName));
                    return;
                }
            }
            catch (Exception)
            {
                //Logger
            }

            //If validation failed or account does not exists only send sessionKey back
           await ctx.Channel.WriteAndFlushAsync(new PlayerData(msg.SessionKey, 0, ""));
        }
    }
}
