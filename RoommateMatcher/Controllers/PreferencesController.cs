using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoommateMatcher.Dtos;
using RoommateMatcher.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RoommateMatcher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PreferencesController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public PreferencesController(AppDbContext context,
            UserManager<AppUser> userManager,
            IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync(UserPreferenecesDto preferences)
        {
            var hasUser = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (hasUser == null)
            {
                return CreateActionResult(
                    CustomResponseDto<UserPreferenecesDto>.Fail(
                        404,
                        new List<string>() {
                            "Kullanıcının bu işleme yetkisi bulunmuyor"
                        }));
            }

            var userPreferences = await _context.UserPreferences.Where(
                z => z.UserId == hasUser.Id)
                .SingleOrDefaultAsync();
            var address = await _context.UserAddresses.Where(z =>
            z.PreferencesId == userPreferences.Id).SingleOrDefaultAsync();

            address.Country = preferences.Address.Country;
            address.City = preferences.Address.City;
            address.District = preferences.Address.District;
            address.Neighborhood = preferences.Address.Neighborhood;

            userPreferences.SmokingAllowed = preferences.SmokingAllowed;
            userPreferences.GuestsAllowed = preferences.GuestsAllowed;
            userPreferences.PetsAllowed = preferences.PetsAllowed;
            userPreferences.GenderPref = preferences.GenderPref;
            userPreferences.ForeignersAllowed = preferences.ForeignersAllowed;
            userPreferences.AlcoholAllowed = preferences.AlcoholAllowed;
            userPreferences.Duration = preferences.Duration;
            userPreferences.AcceptableRoommatesMax = preferences.AcceptableRoommatesMax;
            userPreferences.AcceptableRoommatesMin = preferences.AcceptableRoommatesMin;
            userPreferences.BudgetMin = preferences.BudgetMin;
            userPreferences.BudgetMax = preferences.BudgetMax;
            userPreferences.HasHome = preferences.HasHome;
            userPreferences.Address = address;
           
           var updatedUser = _context.UserPreferences.Update(userPreferences);

            await _context.SaveChangesAsync();

            return CreateActionResult(CustomResponseDto<UserDto>.Success(200,_mapper.Map<UserDto>(hasUser)));
        }
    }
}

