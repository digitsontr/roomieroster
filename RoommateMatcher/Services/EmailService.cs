using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RoommateMatcher.Models;

namespace RoommateMatcher.Services
{
	public class EmailService: IEmailService
	{
        private readonly EmailOptions _options;
        private readonly UserOptions _userOptions;
		public EmailService(IOptions<EmailOptions> options, IOptions<UserOptions> userOptions)
		{
            _options = options.Value;
            _userOptions = userOptions.Value;
		}

        public async Task SendEmailConfirmationLink(string userId,
            string token, string to)
        {
            var smptClient = new SmtpClient();
            var confirmEmailLink = _userOptions.ConfirmEmailAddress
                + "?user=" + userId + "&token=" + token;

            smptClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smptClient.Host = _options.Host;
            smptClient.Port = Int32.Parse(_options.Port);
            smptClient.Credentials = new NetworkCredential(_options.Username,
                _options.Password);
            smptClient.EnableSsl = true;

            var mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(_options.Username);

            mailMessage.To.Add(to);
            string htmlContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "MailTemplates", "EmailConfirmation.html"));

            htmlContent = htmlContent.Replace("emailconfirmationlinkhere", confirmEmailLink);
            htmlContent = htmlContent.Replace("logolinkhere", _options.WhiteDigitsonLogo);

            mailMessage.Subject = "RoomieRoster Email Adresi Doğrulama";
            mailMessage.Body = htmlContent;
            mailMessage.IsBodyHtml = true;

            await smptClient.SendMailAsync(mailMessage);
        }

        public async Task SendResetPasswordEmail(string userId,string token,
            string to)
        {
            var smptClient = new SmtpClient();
            var resetPasswordLink = _userOptions.PasswordResetAddress +
                "?user=" + userId + "&token=" + token;

            smptClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smptClient.Host = _options.Host;
            smptClient.Port = Int32.Parse(_options.Port);
            smptClient.Credentials = new NetworkCredential(_options.Username,
                _options.Password);
            smptClient.EnableSsl = true;

            var mailMessage = new MailMessage();
            string htmlContent = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "MailTemplates", "ResetPasswordTemplate.html"));

            htmlContent = htmlContent.Replace("resetpasswordlinkhere", resetPasswordLink);
            htmlContent = htmlContent.Replace("logolinkhere", _options.WhiteDigitsonLogo);

            mailMessage.From = new MailAddress(_options.Username);
            mailMessage.To.Add(to);
            mailMessage.Subject = "Roomie Şifre Sıfırlama Bağlantısı";
            mailMessage.Body = htmlContent;
            mailMessage.IsBodyHtml = true;

            await smptClient.SendMailAsync(mailMessage);
        }

        public async Task SendNotification(string email)
        {
            var smptClient = new SmtpClient();

            smptClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smptClient.Host = _options.Host;
            smptClient.Port = Int32.Parse(_options.Port);
            smptClient.Credentials = new NetworkCredential(_options.Username,
                _options.Password);
            smptClient.EnableSsl = true;

            var mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(_options.Username);
            mailMessage.To.Add(email);
            mailMessage.Subject = "Roomie Notification";
            mailMessage.Body = @$"<h4>Okunmamış mesajlarınız bulunuyor.</h4>
                            <p><a href={_userOptions.FrontdEndChatPath}>Mesajları Görüntüle</a></p>";
            mailMessage.IsBodyHtml = true;

            await smptClient.SendMailAsync(mailMessage);

        }
    }
}

