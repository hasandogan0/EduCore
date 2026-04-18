using EduCore.Business.DTOs;
using FluentValidation;

namespace EduCore.Business.Validators;

public class AdminCourseDtoValidator : AbstractValidator<AdminCourseDto>
{
    public AdminCourseDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Kurs başlığı boş olamaz.")
            .MaximumLength(200).WithMessage("Kurs başlığı 200 karakteri geçemez.");

        RuleFor(x => x.Instructor)
            .NotEmpty().WithMessage("Eğitmen bilgisi gereklidir.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Fiyat negatif olamaz.");

        // Status: Taslak, Onaylı vb. bir değer gelmeli
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Kurs durumu belirtilmelidir.");
    }
}
