using EduCore.Business.DTOs;
using FluentValidation;

namespace EduCore.Business.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Kullanıcı adı girilmelidir.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre girilmelidir.");
    }
}
