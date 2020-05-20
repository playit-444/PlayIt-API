﻿namespace PlayIt_Api.Models.Dto
{
    public class Response
    {
        public string Message { get; set; }

        public Response(string message)
        {
            Message = message;
        }
    }
}
