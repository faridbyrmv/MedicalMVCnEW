using MimeKit;
using MailKit.Net.Smtp;

namespace Medical.Mail
{
    public class MailService : IMailService
    {
        public async Task Send(string from, string to, string message, string subject)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                TextBody = message 
            };
            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.mail.ru", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("hacibalaev.azik@mail.ru", "hAY3Tcz8WgEh5Nryv1rF");
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
