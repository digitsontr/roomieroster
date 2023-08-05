using System;
using RoommateMatcher.Dtos;
using RoommateMatcher.Models;

namespace RoommateMatcher.Services
{
    public interface ITokenService
    {
        TokenDto CreateToken(AppUser user);
    }
}

