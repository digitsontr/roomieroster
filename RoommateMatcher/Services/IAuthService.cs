using System;
using RoommateMatcher.Dtos;

namespace RoommateMatcher.Services
{
	public interface IAuthService
	{
        Task<CustomResponseDto<TokenDto>> CreateTokenAsync(LogInDto loginDTO);
        Task<CustomResponseDto<TokenDto>> CreateTokenByRefreshToken(string refreshToken);
        Task<CustomResponseDto<TokenDto>> RevokeRefreshToken(string refreshToken);
        Task<CustomResponseDto<TokenDto>> ForgotPassword(string email);
        Task<CustomResponseDto<TokenDto>> ResetPassword(string userId, string token, string password);
        Task<CustomResponseDto<TokenDto>> ConfirmEmail(string userId, string token);
    }
}

