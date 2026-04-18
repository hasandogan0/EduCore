using EduCore.Business.DTOs;
using FluentValidation;

namespace EduCore.Business.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Kullanıcı adı zorunludur.")
            .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresi zorunludur.")
            .EmailAddress().WithMessage("Geçerli bir e-posta formatı giriniz.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre zorunludur.")
            .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Ad soyad alanı boş bırakılamaz.");

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(role => role == "Student" || role == "Instructor")
            .WithMessage("Rol sadece 'Student' veya 'Instructor' olabilir.");

        RuleFor(x => x.ProfileImage)
            .Must(file => file == null || file.Length <= 2 * 1024 * 1024)
            .WithMessage("Profil resmi 2MB'dan büyük olamaz.");
    }
}
