using System;
namespace RoommateMatcher.Configuration
{
    public class CustomTokenOption
    {
        public List<string> Audience { get; set; } = new List<string>();
        public string Issuer { get; set; } = "";

        public int AccessTokenExpiration { get; set; }
        public int RefreshTokenExpiration { get; set; }

        public string SecurityKey { get; set; } = "";
    }
}

