using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoommateMatcher.Dtos;
using RoommateMatcher.Models;
using RoommateMatcher.Services;

namespace RoommateMatcher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuthService _authService;
        private readonly AppDbContext _context;

        public AuthController(UserManager<AppUser> userManager,
            IAuthService authService,
            AppDbContext context)
        {
            _userManager = userManager;
            _authService = authService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Post(SignUpDto user)
        {
            var identityUser = new AppUser()
            {
                UserName = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePhoto = user.ProfilePhoto,
                Birthday = user.BirthDay,
                Email = user.Email,
                Gender = user.Gender,
            };
           var identityResult = await _userManager.CreateAsync(identityUser,
               user.Password);

            if (identityResult.Succeeded)
            {
                var address = new AppUserAddress();
                var preferences = new AppUserPreferences();

                address.Country = "Türkiye";
                address.City = "";
                address.District = "";
                address.Neighborhood = "";

                preferences.UserId = identityUser.Id;
                preferences.GenderPref = identityUser.Gender;
                preferences.Address = address;

                await _context.UserPreferences.AddAsync(preferences);

                await _context.SaveChangesAsync();

                return CreateActionResult(
                    await _authService.CreateTokenAsync(new LogInDto() {
                        Email = user.Email,
                        Password = user.Password}));
            }

            return CreateActionResult(CustomResponseDto<SignUpDto>.Fail(500, "Kullanıcı kaydı sırasında bir hata oluştu"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Post(LogInDto user)
        {
            return CreateActionResult(await _authService.CreateTokenAsync(user));
        }


        [HttpPost("createtokenbyrefreshtoken")]
        public async Task<IActionResult> CreateTokenByRefreshToken(UserRefreshTokenDto token)
        {
            return CreateActionResult(await _authService.CreateTokenByRefreshToken(token.Token));
        }

        [HttpPost("revokerefreshtoken")]
        public async Task<IActionResult> RevokeRefreshToken(UserRefreshTokenDto token)
        {
            return CreateActionResult(await _authService.RevokeRefreshToken(token.Token));
        }
    }
}

