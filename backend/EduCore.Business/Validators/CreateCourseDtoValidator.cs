using EduCore.Business.DTOs;
using FluentValidation;

namespace EduCore.Business.Validators
{
    public class CreateCourseDtoValidator : AbstractValidator<CreateCourseDto>
    {
        public CreateCourseDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Kurs başlığı boş bırakılamaz.")
                .MinimumLength(5).WithMessage("Kurs başlığı en az 5 karakter olmalıdır.")
                .MaximumLength(200).WithMessage("Kurs başlığı 200 karakteri geçemez.");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Açıklama 2000 karakterden fazla olamaz.")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Fiyat negatif bir değer olamaz.");

            RuleFor(x => x.Quota)
                .InclusiveBetween(1, 1000).WithMessage("Kontenjan 1 ile 1000 arasında olmalıdır.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Lütfen geçerli bir kategori seçiniz.");

            RuleFor(x => x.ImageUrl)
                .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                .WithMessage("Geçersiz görsel formatı.");
        }
    }
}
