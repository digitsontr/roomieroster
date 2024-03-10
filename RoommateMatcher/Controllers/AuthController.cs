using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoommateMatcher.Dtos;
using RoommateMatcher.Models;
using RoommateMatcher.Services;
using RoommateMatcher.Validations;

namespace RoommateMatcher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuthService _authService;
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public AuthController(UserManager<AppUser> userManager,
            IAuthService authService,
            AppDbContext context,
            IEmailService emailService)
        {
            _userManager = userManager;
            _authService = authService;
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Post(SignUpDto user)
        {

            var validator = new RegisterDtoValidator();
            ValidationResult result = validator.Validate(user);

            if (!result.IsValid)
            {
                return CreateActionResult(CustomResponseDto<SignUpDto>
                    .Fail(400, result.Errors
                    .Select(z => z.ErrorMessage)
                    .ToList()));
            }

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

            try
            {
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

                    var emailConfirmationToken = await _userManager
                        .GenerateEmailConfirmationTokenAsync(identityUser);
                    await _emailService
                        .SendEmailConfirmationLink(identityUser.Id,
                        emailConfirmationToken, identityUser.Email);

                    await _context.SaveChangesAsync();

                    return CreateActionResult(CustomResponseDto<SignUpDto>
                        .Success(201));
                }

                return CreateActionResult(CustomResponseDto<SignUpDto>
                    .Fail(500, identityResult.Errors.Select(z => z.Description)
                    .ToList()));
            }
            catch(Exception ex)
            {
                throw new Exception($"Identity issue: {ex.InnerException.Message}", ex);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Post(LogInDto user)
        {
            return CreateActionResult(await _authService
                .CreateTokenAsync(user));
        }

        [HttpPost("forgotpassword")]
        public async Task<IActionResult> Post(string email)
        {
            return CreateActionResult(await _authService
                .ForgotPassword(email));
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> Post(ResetPasswordDto passwordDto)
        {
            return CreateActionResult(await _authService
                .ResetPassword(passwordDto.UserId,passwordDto.Token,
                passwordDto.Password));
        }

        [HttpPost("createtokenbyrefreshtoken")]
        public async Task<IActionResult> CreateTokenByRefreshToken(
            UserRefreshTokenDto token)
        {
            return CreateActionResult(await _authService
                .CreateTokenByRefreshToken(token.Token));
        }

        [HttpPost("revokerefreshtoken")]
        public async Task<IActionResult> RevokeRefreshToken(
            UserRefreshTokenDto token)
        {
            return CreateActionResult(await _authService
                .RevokeRefreshToken(token.Token));
        }

        [HttpPost("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(
            EmailConfirmationDto emailConfirmation)
        {
            return CreateActionResult(await _authService
                .ConfirmEmail(emailConfirmation.UserId,
                emailConfirmation.Token));
        }
    }
}

