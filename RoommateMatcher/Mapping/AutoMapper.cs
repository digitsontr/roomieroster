using System;
using AutoMapper;
using RoommateMatcher.Dtos;
using RoommateMatcher.Models;

namespace RoommateMatcher.Mapping
{
	public class AutoMapper: Profile
	{
		public AutoMapper()
		{
			CreateMap<AppUser, UserDto>();
            CreateMap<AppUserAddress, AddressDto>();
			CreateMap<AppUserPreferences, UserPreferenecesDto>();
			CreateMap<Message, MessageDto>();
		}
	}
}

