namespace RoommateMatcher.Models
{
	public class UnreadedChat
	{
		public int Id { get; set; }
		public int ChatId { get; set; }
		public string RecieverId { get; set; }
		public DateTime RecievedAt { get; set; }
		
	}
}

