﻿using System;
namespace RoommateMatcher.Dtos
{
	public class ResetPasswordDto
	{
		public string UserId { get; set; }
		public string Token { get; set; }
		public string Password { get; set; }
	}
}

