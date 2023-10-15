using System;
namespace RoommateMatcher.Models
{
	public class Message
	{
		public int Id { get; set; }
		public string Content { get; set; }
		public DateTime CreatedAt { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }

        public int ChatId { get; set; }
		public Chat Chat { get; set; }
	}
}

