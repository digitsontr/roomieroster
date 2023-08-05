﻿using System;
using System.Text.Json.Serialization;

namespace RoommateMatcher.Dtos
{
    public class TokenDto
    {
        public string? AccessToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public UserDto User { get; set; }
    }
}

