using EduCore.Business.DTOs;
using FluentValidation;

namespace EduCore.Business.Validators;

public class CourseDtoValidator : AbstractValidator<CourseDto>
{
    public CourseDtoValidator()
    {
        RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Güncellenecek kursun ID bilgisi eksik.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Kurs başlığı zorunludur.")
            .Length(5, 200).WithMessage("Başlık 5-200 karakter arasında olmalıdır.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Fiyat hatalı.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Kurs durumu belirtilmelidir.");
    }
}
