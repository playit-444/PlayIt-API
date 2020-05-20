using System;
using System.Collections.Generic;

namespace PlayIt_Api.Models.Entities
{
    public partial class LoginAttempt
    {
        public long LoginAttemptId { get; set; }
        public DateTime Created { get; set; }
        public string UserName { get; set; }
        public string Ipv4 { get; set; }
    }
}
