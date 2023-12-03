using System;
namespace RoommateMatcher.Dtos
{
	public class MessageDto
	{
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SenderUserName { get; set; }
        public string RecieverUserName { get; set; }
    }
}

