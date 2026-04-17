using EduCore.Entity;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Business.Validators
{
    public class LessonValidator : AbstractValidator<Lesson>
    {
        public LessonValidator()
        {
            RuleFor(x => x.Title)
                 .NotEmpty().WithMessage("Ders başlığı boş bırakılamaz.")
                 .MinimumLength(3).WithMessage("Ders başlığı en az 3 karakter olmalıdır.")
                 .MaximumLength(200).WithMessage("Ders başlığı 200 karakterden uzun olamaz.");

            RuleFor(x => x.VideoUrl)
                .Must(uri => string.IsNullOrEmpty(uri) || Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out _))
                .WithMessage("Geçerli bir video yolu veya URL giriniz.");

            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("Dersin atanacağı bir kurs seçilmelidir.")
                .GreaterThan(0).WithMessage("Geçersiz Kurs ID.");
        }
    }
}
