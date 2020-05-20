using System;
using System.Collections.Generic;

namespace PlayIt_Api.Models.Entities
{
    public partial class Token
    {
        public string TokenId { get; set; }
        public short TokenTypeId { get; set; }
        public long AccountId { get; set; }
        public DateTime Expiration { get; set; }
        public DateTime Created { get; set; }

        public virtual Account Account { get; set; }
        public virtual TokenType TokenType { get; set; }
    }
}
