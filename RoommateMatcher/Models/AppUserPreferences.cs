using System;
namespace RoommateMatcher.Models
{
	public class AppUserPreferences
	{
        public int Id { get; set; }
        public byte SmokingAllowed { get; set; } = 2;
        public byte GuestsAllowed { get; set; } = 2;
        public byte PetsAllowed { get; set; } = 2;
        public byte GenderPref { get; set; }
        public byte ForeignersAllowed { get; set; } = 2;
        public byte AlcoholAllowed { get; set; } = 2;
        public byte Duration { get; set; } = 2;
        public int AcceptableRoommatesMin { get; set; } = 0;
        public int AcceptableRoommatesMax { get; set; } = 999;
        public float BudgetMin { get; set; } = 0;
        public float BudgetMax { get; set; } = int.MaxValue;
        public bool HasHome { get; set; } = false;

        public string UserId { get; set; }
        public virtual AppUser User { get; set; }
        public virtual AppUserAddress Address { get; set; }

    }
}

