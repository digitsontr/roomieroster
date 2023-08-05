using System;
namespace RoommateMatcher.Models
{
	public class AppUserAddress
	{
		public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
		public string District { get; set; }
		public string Neighborhood { get; set; }

		public int PreferencesId { get; set; }
        public virtual AppUserPreferences Preferences { get; set; }
    }
}

