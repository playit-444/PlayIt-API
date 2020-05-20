using System;
using System.Collections.Generic;

namespace PlayIt_Api.Models.Entities
{
    public partial class ErrorLog
    {
        public long ErrorLogId { get; set; }
        public string Message { get; set; }
        public DateTime Created { get; set; }
    }
}
