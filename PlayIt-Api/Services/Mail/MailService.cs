using System.Net.Mail;

namespace PlayIt_Api.Services.Mail
{
    public class MailService : IMailService
    {
        /// <summary>
        /// Send mail from office365 through outbound.itoperators.dk
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="toMail"></param>
        public void SendMail(string subject, string body, string toMail)
        {
            SmtpClient client = new SmtpClient
            {
                Port = 25,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = "outbound.itoperators.dk",
                UseDefaultCredentials = true,
            };

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("info@444.dk", "Info");
            mail.To.Add(new MailAddress(toMail));
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            client.SendAsync(mail, mail);
        }
    }
}
