﻿namespace PlayIt_Api.Services.Mail
{
    public interface IMailService
    {
        /// <summary>
        /// Send mail
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="to"></param>
        void SendMail(string subject, string body, string to);
    }
}
