﻿using System.Net;
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


            mailMessage.Subject = "Roomie Email Adresi Doğrulama";
            mailMessage.Body = @$"<h4>Email adresinizi doğrulamak için
                            aşağıdaki bağlantıya tıklayınız.</h4>
                            <p><a href={confirmEmailLink}>Email doğrulama
                            bağlantısı</a></p>";
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

            mailMessage.From = new MailAddress(_options.Username);
            mailMessage.To.Add(to);
            mailMessage.Subject = "Roomie Şifre Sıfırlama Bağlantısı";
            mailMessage.Body = @$"<h4>Şifrenizi yenilemek için aşağıdaki
                            bağlantıya tıklayınız.</h4>
                            <p><a href={resetPasswordLink}>Şifre sıfırlama
                            bağlantısı</a></p>";
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

