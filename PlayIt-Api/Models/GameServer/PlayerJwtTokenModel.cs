﻿namespace PlayIt_Api.Models.GameServer
{
    public class PlayerJwtTokenModel
    {
        public PlayerJwtTokenModel(string jwtToken)
        {
            JwtToken = jwtToken;
        }


        public string JwtToken { get; set; }
    }
}
