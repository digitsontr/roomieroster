namespace RoommateMatcher.Models
{
	public class Message
	{
		public int Id { get; set; }
		public string Content { get; set; }
		public DateTime CreatedAt { get; set; }
        public string SenderUserName { get; set; }
        public string RecieverUserName { get; set; }

        public int ChatId { get; set; }
		public Chat Chat { get; set; }
	}
}

