using System.Collections.Generic;

namespace PlayIt_Api.Models.Entities
{
    public partial class TokenType
    {
        public TokenType()
        {
            Token = new HashSet<Token>();
        }

        public short TokenTypeId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Token> Token { get; set; }
    }
}
