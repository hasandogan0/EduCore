using EduCore.Business.DTOs;

namespace EduCore.Business.Services.Abstract
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseDto>> GetAllCoursesAsync(); // For Public (Published only)
        Task<IEnumerable<CourseDto>> GetInstructorCoursesAsync(string instructorId); // For Instructor (All their courses)
        Task<CourseDto?> GetCourseByIdAsync(int id);

        Task CreateCourseAsync(CreateCourseDto courseDto, string instructorId);
        Task UpdateCourseAsync(int courseId, CreateCourseDto courseDto, string instructorId); // New
        Task DeleteCourseAsync(int courseId, string instructorId); // New (Instructor specific)
        Task ToggleCourseStatusAsync(int courseId, string instructorId); // New (Instructor specific)
        Task<InstructorStatsDto> GetInstructorStatsAsync(string instructorId);

    }
}
