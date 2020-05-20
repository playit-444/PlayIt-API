using System;
using System.Collections.Generic;

namespace PlayIt_Api.Models.Entities
{
    public partial class GameType
    {
        public byte GameTypeId { get; set; }
        public string Name { get; set; }
        public string LogoPath { get; set; }
        public string CoverPath { get; set; }
        public string BackgroundPath { get; set; }
        public byte MaxPlayers { get; set; }
        public byte MinimumPlayers { get; set; }
        public string Description { get; set; }
    }
}
