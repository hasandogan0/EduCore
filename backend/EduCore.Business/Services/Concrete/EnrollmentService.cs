using AutoMapper;
using EduCore.Business.DTOs;
using EduCore.Business.Services.Abstract;
using EduCore.DataAccess.Repositories;
using EduCore.Entity;

namespace EduCore.Business.Services.Concrete;

public class EnrollmentService : IEnrollmentService
{
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IRepository<Course> _courseRepository;
    private readonly IMapper _mapper; // AutoMapper eklendi

    public EnrollmentService(IRepository<Enrollment> enrollmentRepository,
                             IRepository<Course> courseRepository,
                             IMapper mapper) // Inject edildi
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task EnrollStudentAsync(int courseId, string studentId)
    {
        // 1. Kurs var mı ve kayıt için uygun mu kontrolü
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
            throw new Exception("Kurs bulunamadı.");

        if (course.Status != CourseStatus.Published || !course.IsActive)
            throw new Exception("Bu kurs şu an kayda açık değil.");

        // 2. Mükerrer kayıt kontrolü
        var existing = await _enrollmentRepository.FindAsync(e => e.CourseId == courseId && e.StudentId == studentId);
        if (existing.Any())
            throw new Exception("Bu kursa zaten kayıtlısınız.");

        // 3. Kontenjan kontrolü
        var currentEnrollments = await _enrollmentRepository.FindAsync(e => e.CourseId == courseId);
        if (currentEnrollments.Count() >= course.Quota)
            throw new Exception("Kurs kontenjanı dolu.");

        // 4. Kayıt oluşturma (Manuel eşleme yerine AutoMapper kullanılabilir ancak alanlar çok az olduğu için böyle de kalabilir)
        var enrollment = new Enrollment
        {
            CourseId = courseId,
            StudentId = studentId,
            EnrolledAt = DateTime.UtcNow,
            ProgressPercentage = 0
        };

        await _enrollmentRepository.AddAsync(enrollment);
        await _enrollmentRepository.SaveChangesAsync();
    }

    public async Task<bool> IsStudentEnrolledAsync(int courseId, string studentId)
    {
        var existing = await _enrollmentRepository.FindAsync(e => e.CourseId == courseId && e.StudentId == studentId);
        return existing.Any();
    }

    public async Task<IEnumerable<CourseDto>> GetStudentEnrollmentsAsync(string studentId)
    {
        var enrollments = await _enrollmentRepository.FindAsync(e => e.StudentId == studentId);
        var courseIds = enrollments.Select(e => e.CourseId).ToList();

        var courses = await _courseRepository.FindAsync(c => courseIds.Contains(c.Id));

        return _mapper.Map<IEnumerable<CourseDto>>(courses);
    }

    public async Task UpdateProgressAsync(int courseId, string studentId, int progress)
    {
        var enrollments = await _enrollmentRepository.FindAsync(e => e.CourseId == courseId && e.StudentId == studentId);
        var enrollment = enrollments.FirstOrDefault();

        if (enrollment != null)
        {
            enrollment.ProgressPercentage = progress;
            await _enrollmentRepository.UpdateAsync(enrollment);
            await _enrollmentRepository.SaveChangesAsync();
        }
    }

    public async Task<int> GetEnrollmentProgressAsync(int courseId, string studentId)
    {
        var enrollments = await _enrollmentRepository.FindAsync(e => e.CourseId == courseId && e.StudentId == studentId);
        var enrollment = enrollments.FirstOrDefault();
        return enrollment?.ProgressPercentage ?? 0;
    }
}