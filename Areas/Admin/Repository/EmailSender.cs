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
                client.Credentials = new NetworkCredential("doanminhvuong071@gmail.com", "app_password");
                //dùng App Password, không dùng mật khẩu Gmail thường
                var mailMessage = new MailMessage(
                    from: "doanminhvuong071@gmail.com",
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
