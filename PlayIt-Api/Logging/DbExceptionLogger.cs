using System;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using PlayIt_Api.Models.Entities;

namespace PlayIt_Api.Logging
{
    /// <summary>
    /// Exception logger to database
    /// </summary>
    public class DbExceptionLogger : ILogger
    {
        private readonly IUnitOfWork _unitOfWork;

        public DbExceptionLogger([FromServices] IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        ~DbExceptionLogger()
        {
            Dispose(false);
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

        public void Log(string msg)
        {
            var errorLogRepo = _unitOfWork.GetRepository<ErrorLog>();
            errorLogRepo.Insert(new ErrorLog {Created = DateTime.Now, Message = msg});
        }

        public async Task LogAsync(string msg)
        {
            var errorLogRepo = _unitOfWork.GetRepository<ErrorLog>();
            await errorLogRepo.InsertAsync(new ErrorLog {Created = DateTime.Now, Message = msg});
        }
    }
}
