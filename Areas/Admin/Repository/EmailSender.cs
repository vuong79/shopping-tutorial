using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Shopping_Tutorial.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;

                // Gmail + App Password (16 ký tự)
                client.Credentials = new NetworkCredential("ny5489656@gmail.com", "bbxkgizhshsvucdq");

                var mailMessage = new MailMessage(
                    from: "ny5489656@gmail.com",
                    to: email,
                    subject,
                    htmlMessage
                )
                {
                    IsBodyHtml = true
                };

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}

