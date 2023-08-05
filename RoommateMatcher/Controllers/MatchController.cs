using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoommateMatcher.Dtos;
using RoommateMatcher.Extensions;
using RoommateMatcher.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RoommateMatcher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MatchController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public MatchController(AppDbContext context,
            UserManager<AppUser> userManager, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/values
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var hasUser = await _context.Users
                .Where(z => z.UserName == HttpContext.User.Identity!.Name)
                .Include(z => z.Preferences)
                .ThenInclude(z=>z.Address)
                .SingleOrDefaultAsync();
            var allUsers = _context.Users
                .Include(z => z.Preferences)
                .ThenInclude(z=>z.Address)
                .FilterByPreferences(hasUser!.Preferences, hasUser)
                .ToList();
            var matchedUserDtos = new List<UserDto>();

            foreach (var user in allUsers)
            {
                matchedUserDtos.Add(_mapper.Map<UserDto>(user));
            }

            return CreateActionResult(CustomResponseDto<MatchesDto>.Success(200,
                new MatchesDto() { Matches = matchedUserDtos
                .Where(z=>z.Id != hasUser.Id)
                .ToList() }));
        }
    }
}

