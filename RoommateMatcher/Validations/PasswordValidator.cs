using System;
using Microsoft.AspNetCore.Identity;
using RoommateMatcher.Models;

namespace RoommateMatcher.Validations
{
	public class PasswordValidator:IPasswordValidator<AppUser>
	{
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string? password)
        {
            var errors = new List<IdentityError>();

            if (password.ToLower().Contains(user.UserName.ToLower()))
            {
                errors.Add(new()
                {
                    Code = "PasswordContainsUserName",
                    Description = "Şifreniz kullanıcı adınızı içermemeli"
                });
            }

            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}

