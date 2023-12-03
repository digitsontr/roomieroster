using System;
namespace RoommateMatcher.Services
{
	public interface IEmailService
	{
		Task SendResetPasswordEmail(string userId, string token, string to);
		Task SendEmailConfirmationLink(string userId, string token, string to);
        Task SendNotification(string email);
    }
}

