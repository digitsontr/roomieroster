using System;
namespace RoommateMatcher.Models
{
	public class AppUserFollows
	{
		public int Id { get; set; }
		public string FollowerId { get; set; }
		public string FollowedId { get; set; }
	}
}

