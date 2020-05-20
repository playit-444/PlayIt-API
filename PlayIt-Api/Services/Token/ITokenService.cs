using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace PlayIt_Api.Services.Token
{
    public interface ITokenService : IDisposable
    {
        /// <summary>
        /// Create token
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="tokenType"></param>
        /// <returns></returns>
        ValueTask<EntityEntry<Models.Entities.Token>> CreateToken(long accountId, short tokenType);
    }
}
