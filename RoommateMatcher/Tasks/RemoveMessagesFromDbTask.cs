using RoommateMatcher.Models;

namespace RoommateMatcher.Tasks
{
	public class RemoveMessagesFromDbTask
	{
        private readonly AppDbContext _dbContext;

        public RemoveMessagesFromDbTask(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void RemoveMessagesOlderThanTenDays()
        {
            var messages = _dbContext.Messages.ToList();

            foreach (var message in messages)
            {
                if ((DateTime.Now - message.CreatedAt).TotalDays > 10)
                {

                    _dbContext.Messages.Remove(message);
                }

            }

            _dbContext.SaveChanges();
        }
    }
}

