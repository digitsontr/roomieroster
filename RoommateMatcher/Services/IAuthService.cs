using System;
using RoommateMatcher.Dtos;

namespace RoommateMatcher.Services
{
	public interface IAuthService
	{
        Task<CustomResponseDto<TokenDto>> CreateTokenAsync(LogInDto loginDTO);
        Task<CustomResponseDto<TokenDto>> CreateTokenByRefreshToken(string refreshToken);
        Task<CustomResponseDto<TokenDto>> RevokeRefreshToken(string refreshToken);
    }
}

