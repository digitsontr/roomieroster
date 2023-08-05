using Microsoft.AspNetCore.Identity;

namespace RoommateMatcher.Models
{
	public class AppUser:IdentityUser
	{
		public string FirstName { get; set; } = "";
		public string LastName { get; set; } = "";
		public string ProfilePhoto { get; set; } = "defaultimage.png";
		public DateTime CreatedAt { get; set; }
		public DateTime UpdaredAt { get; set; }
		public DateTime Birthday { get; set; }
		public byte Gender { get; set; }
		public bool Status { get; set; } = true;

        public AppUserPreferences Preferences { get; set; }
    }
}

