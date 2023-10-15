using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoommateMatcher.Dtos;
using RoommateMatcher.Extensions;
using RoommateMatcher.Models;

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
            var followedUsers =  _context.UserFollows.Where(
                z => z.FollowerId == hasUser.Id).ToList();
            var matchedUserDtos = new List<MatchDto>();

            foreach (var user in allUsers)
            {
                matchedUserDtos.Add(new MatchDto()
                {
                    User = _mapper.Map<UserDto>(user),
                    IsFolowing = followedUsers.Where(z => z.FollowedId
                    == user.Id).Count() > 0
                });
            }

            return CreateActionResult(CustomResponseDto<MatchesDto>.Success(200,
                new MatchesDto() { Matches = matchedUserDtos
                .Where(z=>z.User.Id != hasUser.Id)
                .ToList() }));
        }
    }
}

