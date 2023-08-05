using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    public class UserController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(string id)
        {
            var hasUser = await _context.Users
                .Where(z => z.Id == id)
                .Include(z=>z.Preferences)
                .ThenInclude(z=>z.Address)
                .SingleOrDefaultAsync();

            if (hasUser == null)
            {
                return CreateActionResult(CustomResponseDto<UserDto>.Fail(404,
                    new List<string>() { "Kullanıcı bulunamadu" }));
            }

            return CreateActionResult(CustomResponseDto<UserDto>.Success(200,
                _mapper.Map<UserDto>(hasUser)));
        }

        [HttpGet("getfollows")]
        public async Task<IActionResult> GetFollows()
        {
            var user = await _context.Users
                .Where(z => z.UserName == HttpContext.User.Identity!.Name)
                .SingleOrDefaultAsync();
            var userFollows = await _context.UserFollows
                .Where(z => z.FollowerId == user!.Id)
                .ToListAsync();
            var userFollowsDto = new List<UserDto>();

            foreach (var userFollow in userFollows)
            {
                var hasUser = await _context.Users
                    .Where(z => z.Id == userFollow.FollowedId)
                    .Include(z => z.Preferences)
                    .ThenInclude(z=>z.Address)
                    .SingleOrDefaultAsync();
               
                if (hasUser != null)
                {
                    userFollowsDto.Add(_mapper.Map<UserDto>(hasUser));
                }
            }

            return CreateActionResult(CustomResponseDto<List<UserDto>>
                .Success(200, userFollowsDto));
        }

        // POST api/values
        [HttpPost("follow")]
        public async Task<IActionResult> PostAsync(string id)
        {
            var user = await _context.Users
                .Where(z => z.UserName == HttpContext.User.Identity!.Name)
                .SingleOrDefaultAsync();
            var hasUser = await _context.Users
                .Where(z => z.Id == id)
                .SingleOrDefaultAsync();
            var hasEntity = await _context.UserFollows
                .Where(z => z.FollowerId == user!.Id && z.FollowedId == id)
                .SingleOrDefaultAsync();

            if (hasEntity != null)
            {
                return CreateActionResult(CustomResponseDto<UserDto>.Fail(404,
                    new List<string>()
                    { "Kullanıcı bu kullanıcı zaten takip ediyor" }));
            }

            if (hasUser == null)
            {
                return CreateActionResult(CustomResponseDto<UserDto>.Fail(404,
                    new List<string>()
                    { "Takip edilmek istenen kullanıcı bulunamadı" }));
            }

            await _context.UserFollows.AddAsync(new AppUserFollows()
            {
                FollowedId = id,
                FollowerId = user!.Id
            });

            await _context.SaveChangesAsync();

            var userFollows = await _context.UserFollows
                .Where(z => z.FollowerId == user.Id)
                .ToListAsync();
            var userFollowsDto = new List<UserDto>();

            foreach (var userFollow in userFollows)
            {
                var hasUserFollow = await _context.Users
                    .Where(z => z.Id == userFollow.FollowedId)
                    .Include(z => z.Preferences)
                    .ThenInclude(z => z.Address)
                    .SingleOrDefaultAsync();

                if (hasUserFollow != null)
                {
                    userFollowsDto.Add(_mapper.Map<UserDto>(hasUserFollow));
                }
            }

            return CreateActionResult(CustomResponseDto<List<UserDto>>
                .Success(201,userFollowsDto));
        }

        // POST api/values
        [HttpPost("unfollow")]
        public async Task<IActionResult> Post(string id)
        {
            var user = await _context.Users
                .Where(z => z.UserName == HttpContext.User.Identity!.Name)
                .SingleOrDefaultAsync();
            var hasUser = await _context.Users
                .Where(z => z.Id == id)
                .SingleOrDefaultAsync();
            var entity = await _context.UserFollows
                .Where(z => z.FollowedId == id && z.FollowerId == user!.Id)
                .SingleOrDefaultAsync();

             _context.UserFollows.Remove(entity!);

            await _context.SaveChangesAsync();

            var userFollows = await _context.UserFollows
                .Where(z => z.FollowerId == user!.Id)
                .ToListAsync();
            var userFollowsDto = new List<UserDto>();

            foreach (var userFollow in userFollows)
            {
                var hasUserFollow = await _context.Users
                    .Where(z => z.Id == userFollow.FollowedId)
                    .Include(z => z.Preferences)
                    .ThenInclude(z => z.Address)
                    .SingleOrDefaultAsync();

                if (hasUserFollow != null)
                {
                    userFollowsDto.Add(_mapper.Map<UserDto>(hasUserFollow));
                }
            }


            return CreateActionResult(CustomResponseDto<List<UserDto>>
                .Success(201,userFollowsDto));
        }

        // PUT api/values/5
        [HttpPut("disableuser")]
        public async Task<IActionResult> PutAsync()
        {
            var user = await _context.Users
                .Where(z => z.UserName == HttpContext.User.Identity!.Name)
                .Include(z=>z.Preferences)
                .ThenInclude(z=>z.Address)
                .SingleOrDefaultAsync();

            user!.Status = false;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();
            
            return CreateActionResult(CustomResponseDto<UserDto>
                .Success(200, _mapper.Map<UserDto>(user)));
        }

        // PUT api/values/5
        [HttpPut("enableuser")]
        public async Task<IActionResult> Put()
        {
            var user = await _context.Users
                .Where(z => z.UserName == HttpContext.User.Identity!.Name)
                .Include(z => z.Preferences)
                .ThenInclude(z => z.Address)
                .SingleOrDefaultAsync();

            user!.Status = true;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return CreateActionResult(CustomResponseDto<UserDto>
                .Success(200, _mapper.Map<UserDto>(user)));
        }

    }
}

