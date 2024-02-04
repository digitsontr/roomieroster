using System;
using Microsoft.AspNetCore.Identity;

namespace RoommateMatcher.Localization
{
	public class LocalizationIdentityErrorDescriber: IdentityErrorDescriber
	{
        public override IdentityError DuplicateUserName(string userName)
        {
            //return base.DuplicateUserName(userName);

            return new IdentityError() {
                Code = "DuplicateUsername",
                Description = $"{userName} başkası tarafından kullanılıyor" };
        }

        public override IdentityError InvalidEmail(string? email)
        {
            return new IdentityError()
            {
                Code = "DuplicateEmail",
                Description =
                $"Lütfen e mail adresinizi kontrol ediniz"
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError() { Code = "DuplicateEmail",
                Description =
                $"{email} adresi başkası tarafından kullanılıyor" };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError()
            {
                Code = "PasswordTooShort",
                Description = $"Şifre en az 6 karakter içermelidir"
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError()
            {
                Code = "PasswordRequiresLower",
                Description = $"Şifre en az 1 küçük harf içermelidir"
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError()
            {
                Code = "PasswordRequiresUpper",
                Description = $"Şifre en az 1 büyük harf içermelidir"
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError()
            {
                Code = "PasswordRequiresUpper",
                Description = $"Şifre en az 1 rakam içermelidir"
            };
        }

        public override IdentityError InvalidUserName(string? userName)
        {
            return new IdentityError()
            {
                Code = "InvalidUserName",
                Description = $"{userName} uygun değil"
            };
        }
    }
}

