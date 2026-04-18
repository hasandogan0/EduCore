using EduCore.Business.DTOs;

namespace EduCore.Business.Services.Abstract;

public interface IEnrollmentService
{
    Task EnrollStudentAsync(int courseId, string studentId);
    Task<bool> IsStudentEnrolledAsync(int courseId, string studentId);
    Task<IEnumerable<CourseDto>> GetStudentEnrollmentsAsync(string studentId);
    Task UpdateProgressAsync(int courseId, string studentId, int progress);
    Task<int> GetEnrollmentProgressAsync(int courseId, string studentId);
}