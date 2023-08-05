using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RoommateMatcher.Dtos;
using RoommateMatcher.Models;

namespace RoommateMatcher.Services
{
	public class AuthService:IAuthService
	{
        private readonly ITokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;

        public AuthService(ITokenService tokenService,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            AppDbContext context)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }


        public async Task<CustomResponseDto<TokenDto>> CreateTokenAsync(LogInDto loginDTO)
        {
            var hasUser = await _context.Users.Where(z=>z.Email == loginDTO.Email).Include(z=>z.Preferences).ThenInclude(z=>z.Address).SingleOrDefaultAsync();

            if (hasUser == null)
            {
                return CustomResponseDto<TokenDto>.Fail(404, "E posta adresi ya da şifre hatalı");
            }


            var signInResult = await _signInManager.PasswordSignInAsync(hasUser, loginDTO.Password, false, false);

            if (signInResult.Succeeded)
            {
                var token = _tokenService.CreateToken(hasUser);
                var userRefreshToken = await _context.UserRefreshTokens.Where(z => z.UserId == hasUser.Id).SingleOrDefaultAsync();

                if (userRefreshToken == null)
                {
                    await _context.UserRefreshTokens.AddAsync(new AppUserRefreshToken()
                    {
                        Code = token.RefreshToken,
                        Expiration = token.RefreshTokenExpiration,
                        UserId = hasUser.Id
                    });


                   await _context.SaveChangesAsync();
                }
                else
                {
                    userRefreshToken.Code = token.RefreshToken;
                    userRefreshToken.Expiration = token.RefreshTokenExpiration;

                    _context.UserRefreshTokens.Update(userRefreshToken);

                    await _context.SaveChangesAsync();
                }

                return CustomResponseDto<TokenDto>.Success(200, token);
            }

            return CustomResponseDto<TokenDto>.Fail(404, new List<string>() { "E posta adresi ya da şifre hatalı" });
        }

        public async Task<CustomResponseDto<TokenDto>> CreateTokenByRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _context.UserRefreshTokens.Where(z => z.Code == refreshToken).SingleOrDefaultAsync();

            if (existRefreshToken == null)
            {
                return CustomResponseDto<TokenDto>.Fail(500, new List<string>() { "Kullancıya ait refresh token bulunamadı" });
            }


            var user = await _userManager.FindByIdAsync(existRefreshToken.UserId);

            if (user == null)
            {
                return CustomResponseDto<TokenDto>.Fail(500, new List<string>() { "Kullanıcı bulunamadı" });
            }

            var token = _tokenService.CreateToken(user);

            existRefreshToken.Code = token.RefreshToken;
            existRefreshToken.Expiration = token.RefreshTokenExpiration;
            existRefreshToken.UpdatedAt = DateTime.Now;

            _context.UserRefreshTokens.Update(existRefreshToken);

            await _context.SaveChangesAsync();

            return CustomResponseDto<TokenDto>.Success(200, token);
        }

        public async Task<CustomResponseDto<TokenDto>> RevokeRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _context.UserRefreshTokens.Where(z => z.Code == refreshToken).SingleOrDefaultAsync();

            if (existRefreshToken == null)
            {
                return CustomResponseDto<TokenDto>.Fail(500, new List<string>() { "Kullancıya ait refresh token bulunamadı" });
            }

            _context.UserRefreshTokens.Remove(existRefreshToken);

            await _context.SaveChangesAsync();

            return CustomResponseDto<TokenDto>.Success(200);
        }
    }
}

