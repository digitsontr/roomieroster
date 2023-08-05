using System;
namespace RoommateMatcher.Models
{
    public class AppUserRefreshToken
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UserId { get; set; }
        public string? Code { get; set; }
        public DateTime Expiration { get; set; }
    }
}

