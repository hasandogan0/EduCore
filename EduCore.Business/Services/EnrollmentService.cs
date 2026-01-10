using EduCore.DataAccess.Repositories;
using EduCore.Entity;

namespace EduCore.Business.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IRepository<Enrollment> _enrollmentRepository;
        private readonly IRepository<Course> _courseRepository;

        public EnrollmentService(IRepository<Enrollment> enrollmentRepository, IRepository<Course> courseRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _courseRepository = courseRepository;
        }

        public async Task EnrollStudentAsync(int courseId, string studentId)
        {
            // 1. Check if course exists and is open for enrollment
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw new Exception("Course not found.");

            if (course.Status != CourseStatus.Published || !course.IsActive)
                throw new Exception("Course is not available for enrollment.");

            // 2. Check overlap
            var existing = await _enrollmentRepository.FindAsync(e => e.CourseId == courseId && e.StudentId == studentId);
            if (existing.Any())
                throw new Exception("User is already enrolled in this course.");

            // 3. Check quota
            var currentEnrollments = await _enrollmentRepository.FindAsync(e => e.CourseId == courseId);
            if (currentEnrollments.Count() >= course.Quota)
                throw new Exception("Course qouta is full.");

            // 4. Create enrollment
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

        public async Task<IEnumerable<Course>> GetStudentEnrollmentsAsync(string studentId)
        {
            var enrollments = await _enrollmentRepository.FindAsync(e => e.StudentId == studentId);
            var courseIds = enrollments.Select(e => e.CourseId).ToList();
            
            // Efficiently fetch courses
            // Since FindAsync accepts Expression, we might hit issues with Contains if the Repo doesn't support it fully or if it's not EF directly exposed.
            // But typical Repository.The FindAsync implementation uses dbSet.Where(predicate).
            // So courseIds.Contains(c.Id) should work if EF Core translates it.
            
            // However, to be super safe and since we don't have a specific GetCoursesByIds, let's try FindAsync with Contains.
            // If that fails at runtime, we might need a fallback. Given the constraints, I'll attempt the most standard EF Core way.
            
            var courses = await _courseRepository.FindAsync(c => courseIds.Contains(c.Id));
            return courses;
        }

        public async Task UpdateProgressAsync(int courseId, string studentId, int progress)
        {
            var enrollment = (await _enrollmentRepository.FindAsync(e => e.CourseId == courseId && e.StudentId == studentId)).FirstOrDefault();
            if (enrollment != null)
            {
                enrollment.ProgressPercentage = progress;
                await _enrollmentRepository.UpdateAsync(enrollment);
                await _enrollmentRepository.SaveChangesAsync();
            }
        }

        public async Task<int> GetEnrollmentProgressAsync(int courseId, string studentId)
        {
             var enrollment = (await _enrollmentRepository.FindAsync(e => e.CourseId == courseId && e.StudentId == studentId)).FirstOrDefault();
             return enrollment?.ProgressPercentage ?? 0;
        }
    }
}
