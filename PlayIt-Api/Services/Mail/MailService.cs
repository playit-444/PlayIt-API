using System.Net.Mail;

namespace PlayIt_Api.Services.Mail
{
    public class MailService : IMailService
    {
        public void SendMail(string subject, string body, string toMail)
        {
            //Create client to send message from
            SmtpClient client = new SmtpClient
            {
                Port = 25,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = "outbound.itoperators.dk",
                UseDefaultCredentials = true,
            };

            MailMessage mail = new MailMessage {From = new MailAddress("info@444.dk", "Info")};
            //Add sender
            mail.To.Add(new MailAddress(toMail));
            mail.Subject = subject;
            mail.Body = body;
            //Set mail to be send as html
            mail.IsBodyHtml = true;
            client.SendAsync(mail, mail);
        }
    }
}
