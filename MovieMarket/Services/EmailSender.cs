
using System.Net.Mail;
using System.Net;

namespace MovieMarket.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("abdelwahabshandy@gmail.com", "yrlt nmgm yxdy wiye")
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("abdelwahabshandy@gmail.com"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
    }

}