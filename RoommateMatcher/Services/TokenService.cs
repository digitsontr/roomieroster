using System;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RoommateMatcher.Configuration;
using RoommateMatcher.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using RoommateMatcher.Models;
using AutoMapper;

namespace RoommateMatcher.Services
{
    public class TokenService : ITokenService
    {
        private readonly CustomTokenOption _tokenOption;
        private readonly IMapper _mapper;

        public TokenService(IOptions<CustomTokenOption> options, IMapper mapper)
        {
            _tokenOption = options.Value;
            _mapper= mapper;
        }

        private string CreateRefreshToken()
        {
            var numberByte = new Byte[32];

            using var rnd = RandomNumberGenerator.Create();

            rnd.GetBytes(numberByte);

            return Convert.ToBase64String(numberByte);
        }

        private IEnumerable<Claim> GetClaims(AppUser user,
            List<string> audiences)
        {
            var userList = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(Microsoft.IdentityModel.JsonWebTokens.
                JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Surname,user.LastName),
                new Claim(ClaimTypes.UserData,user.FirstName),
                new Claim(Microsoft.IdentityModel.JsonWebTokens.
                JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            userList.AddRange(audiences.Select(z => new Claim(Microsoft.
                IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Aud, z)));

            return userList;
        }

        public TokenDto CreateToken(AppUser user)
        {
            var accessTokenExpiration = DateTime.Now
                .AddMinutes(_tokenOption.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.Now
                .AddMinutes(_tokenOption.RefreshTokenExpiration);
            var securityKey = SignService.GetSymmetricSecurityKey(
                _tokenOption.SecurityKey);

            SigningCredentials signingCredentials =
                new SigningCredentials(securityKey,
                SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken jwtSecurityToken =
                new JwtSecurityToken(issuer: _tokenOption.Issuer,
                expires: accessTokenExpiration, notBefore: DateTime.Now,
                claims: GetClaims(user, _tokenOption.Audience),
                signingCredentials: signingCredentials);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.WriteToken(jwtSecurityToken);
            var tokenDto = new TokenDto
            {
                AccessToken = token,
                RefreshToken = CreateRefreshToken(),
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration,
                User = _mapper.Map<UserDto>(user)
            };

            return tokenDto;
        }
    }
}

