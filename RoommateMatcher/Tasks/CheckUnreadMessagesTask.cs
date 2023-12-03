using System;
using RoommateMatcher.Models;
using RoommateMatcher.Services;

namespace RoommateMatcher.Tasks
{
	public class CheckUnreadMessagesTask
	{
        private readonly AppDbContext _dbContext;
        private readonly IEmailService _emailService;

        public CheckUnreadMessagesTask(AppDbContext dbContext, IEmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
        }

        public void CheckUnreadMessages()
        {
            var unreadedChats = _dbContext.UnreadedChats.ToList();

            foreach (var unreadedChat in unreadedChats)
            {
                if ((DateTime.Now - unreadedChat.RecievedAt).TotalHours > 1)
                {
                    var user = _dbContext.Users.Where(z => z.Id ==
                  unreadedChat.RecieverId).SingleOrDefault();

                    _emailService.SendNotification(user.Email);

                    _dbContext.UnreadedChats.Remove(unreadedChat);
                }
              
            }

            _dbContext.SaveChanges();
        }
    }
}

