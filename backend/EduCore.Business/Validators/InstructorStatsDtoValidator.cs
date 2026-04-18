using EduCore.Business.DTOs;
using FluentValidation;

namespace EduCore.Business.Validators;

public class InstructorStatsDtoValidator : AbstractValidator<InstructorStatsDto>
{
    public InstructorStatsDtoValidator()
    {
        RuleFor(x => x.TotalEarnings)
                .GreaterThanOrEqualTo(0).WithMessage("Toplam kazanç negatif olamaz.");

        RuleFor(x => x.TotalStudents)
                .GreaterThanOrEqualTo(0).WithMessage("Toplam öğrenci sayısı hatalı.");

        RuleFor(x => x.TotalCourses)
                .GreaterThanOrEqualTo(0).WithMessage("Toplam kurs sayısı hatalı.");
    }
}
