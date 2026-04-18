using EduCore.Business.DTOs;
using FluentValidation;

namespace EduCore.Business.Validators;

public class CreateLessonDtoValidator : AbstractValidator<CreateLessonDto>
{
    public CreateLessonDtoValidator()
    {
        RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Ders başlığı boş bırakılamaz.")
                .MinimumLength(3).WithMessage("Ders başlığı en az 3 karakter olmalıdır.")
                .MaximumLength(200).WithMessage("Ders başlığı 200 karakteri geçemez.");

        RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("Dersin atanacağı kurs geçerli bir ID olmalıdır.");

        RuleFor(x => x.VideoUrl)
                .Must(uri => string.IsNullOrEmpty(uri) || Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute))
                .WithMessage("Geçersiz video bağlantısı formatı.");
    }
}
