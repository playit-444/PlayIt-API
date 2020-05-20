using System;
using System.Collections.Generic;

namespace PlayIt_Api.Models.Entities
{
    public partial class Account
    {
        public Account()
        {
            Token = new HashSet<Token>();
        }

        public long AccountId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public bool Verified { get; set; }
        public string AvatarFilePath { get; set; }
        public DateTime Created { get; set; }

        public virtual ICollection<Token> Token { get; set; }
    }
}
