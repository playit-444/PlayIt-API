using System;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PlayIt_Api.Logging;

namespace PlayIt_Api.Services.Token
{
    public class TokenService : ITokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public TokenService([FromServices] IUnitOfWork unitOfWork, [FromServices] ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Create token in database
        /// can example be a verify token for email
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="tokenType"></param>
        /// <returns></returns>
        public async ValueTask<EntityEntry<Models.Entities.Token>> CreateToken(long accountId, short tokenType)
        {
            //Create token
            var accountRepo = _unitOfWork.GetRepository<Models.Entities.Token>();

            EntityEntry<Models.Entities.Token> token = null;

            try
            {
                token = await accountRepo.InsertAsync(new Models.Entities.Token
                {
                    Created = DateTime.Now, TokenTypeId = tokenType, Expiration = DateTime.Now.AddHours(12),
                    AccountId = accountId, TokenId = Guid.NewGuid().ToString()
                });
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await _logger.LogAsync(e.Message);
            }

            return token;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.SaveChangesAsync();
                _unitOfWork.Dispose();
            }
        }
    }
}
