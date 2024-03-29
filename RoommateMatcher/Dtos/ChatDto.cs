﻿namespace RoommateMatcher.Dtos
{
	public class ChatDto
	{
		public int Id { get; set; }
		public string RecieverFullName { get; set; }
		public string LastMessage { get; set; }
		public string RecieverUserName { get; set; }
		public string RecieverProfilePhoto { get; set; }
		public DateTime LastMessageDate { get; set; }
		public bool IsReaded { get; set; }
	}
}

