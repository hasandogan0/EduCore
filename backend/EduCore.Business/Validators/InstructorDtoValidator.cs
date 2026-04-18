using EduCore.Business.DTOs;
using FluentValidation;

namespace EduCore.Business.Validators;

public class InstructorDtoValidator : AbstractValidator<InstructorDto>
{
    public InstructorDtoValidator()
    {
        RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Eğitmen kimlik bilgisi eksik.");

        RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Eğitmen adı ve soyadı alanı boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Ad soyad 100 karakterden uzun olamaz.");

        RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Kullanıcı adı gereklidir.");

        RuleFor(x => x.Bio)
                .MaximumLength(1000).WithMessage("Biyografi en fazla 1000 karakter olabilir.")
                .MinimumLength(10).WithMessage("Biyografi girilecekse en az 10 karakter olmalıdır.")
                .When(x => !string.IsNullOrEmpty(x.Bio));

        RuleFor(x => x.HeadshotUrl)
                .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                .WithMessage("Profil fotoğrafı için geçerli bir URL veya dosya yolu giriniz.");
    }
}
