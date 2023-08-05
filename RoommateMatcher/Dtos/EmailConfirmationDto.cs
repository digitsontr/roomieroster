using System;
namespace RoommateMatcher.Dtos
{
	public class EmailConfirmationDto
	{
		public string UserId { get; set; }
		public string Token { get; set; }
	}
}

