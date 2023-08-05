using System;
using FluentValidation;
using RoommateMatcher.Dtos;

namespace RoommateMatcher.Validations
{
	public class RegisterDtoValidator: AbstractValidator<SignUpDto>
	{
		public RegisterDtoValidator()
		{
			RuleFor(z => z.FirstName)
				.NotEmpty()
				.WithMessage("İsim alanı boş olamaz");
            RuleFor(z => z.LastName)
                .NotEmpty()
                .WithMessage("Soyisim alanı boş olamaz")
                .MinimumLength(1)
                .WithMessage("Soyisim alanı boş olamaz");
			RuleFor(z => z.Username)
				.NotEmpty()
				.WithMessage("Username alanı boş olamaz")
				.MinimumLength(5)
				.WithMessage("Username en az 5 karakterli olmalıdır");
            RuleFor(z => z.BirthDay)
                .NotEmpty()
                .WithMessage("Kullanıcı doğum tarihi alanı boş olamaz");
        }
    }
}

