using EduCore.Business.DTOs;
using FluentValidation;

namespace EduCore.Business.Validators;

public class AdminUserDtoValidator : AbstractValidator<AdminUserDto>
{
    public AdminUserDtoValidator()
    {
        RuleFor(x => x.Id)
                .NotEmpty();

        RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Kullanıcı adı boş olamaz.");

        RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Ad soyad alanı zorunludur.")
                .MaximumLength(100);

        RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta adresi gereklidir.")
                .EmailAddress().WithMessage("Geçerli bir e-posta formatı giriniz.");

        RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Kullanıcı rolü belirtilmelidir.");

        RuleFor(x => x.TotalEarnings)
                .GreaterThanOrEqualTo(0).WithMessage("Kazanç bilgisi negatif olamaz.");
    }
}
