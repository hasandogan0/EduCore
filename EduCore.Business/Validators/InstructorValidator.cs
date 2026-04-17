using EduCore.Entity;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Business.Validators;

public class InstructorValidator : AbstractValidator<Instructor>
{
    public InstructorValidator()
    {
        RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Eğitmen adı ve soyadı boş bırakılamaz.")
                .Length(5, 100).WithMessage("Eğitmen adı 5 ile 100 karakter arasında olmalıdır.");

        RuleFor(x => x.Bio)
                .MaximumLength(1000).WithMessage("Biyografi 1000 karakterden uzun olamaz.")
                .When(x => !string.IsNullOrEmpty(x.Bio));

        RuleFor(x => x.HeadshotUrl)
                .Must(BeAValidUrl).WithMessage("Profil fotoğrafı için geçerli bir URL girilmelidir.")
                .When(x => !string.IsNullOrEmpty(x.HeadshotUrl));
    }
    private bool BeAValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
