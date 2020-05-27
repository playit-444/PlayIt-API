using System;

namespace PlayIt_Api.Models.Entities
{
    public partial class ErrorLog
    {
        public long ErrorLogId { get; set; }
        public string Message { get; set; }
        public DateTime Created { get; set; }
    }
}
