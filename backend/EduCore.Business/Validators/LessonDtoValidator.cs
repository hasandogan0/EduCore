using EduCore.Business.DTOs;
using FluentValidation;

namespace EduCore.Business.Validators;

public class LessonDtoValidator : AbstractValidator<LessonDto>
{
    public LessonDtoValidator()
    {
        RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Güncellenecek dersin ID bilgisi gereklidir.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Ders başlığı zorunludur.")
            .Length(3, 200).WithMessage("Başlık 3-200 karakter arasında olmalıdır.");

        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("Geçersiz kurs referansı.");

        RuleFor(x => x.VideoUrl)
            .Must(uri => string.IsNullOrEmpty(uri) || Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute))
            .WithMessage("Geçersiz video bağlantısı.");
    }
}
