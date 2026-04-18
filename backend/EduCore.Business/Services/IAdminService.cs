using EduCore.Business.DTOs;

namespace EduCore.Business.Services
{
    public interface IAdminService
    {
        Task<IEnumerable<AuthResponseDto>> GetPendingInstructorsAsync();
        Task ApproveInstructorAsync(string username);
        Task<IEnumerable<CourseDto>> GetPendingCoursesAsync();
        Task ApproveCourseAsync(int courseId);
        
        // New methods
        Task<IEnumerable<AdminCourseDto>> GetAllCoursesAdminAsync();
        Task<IEnumerable<AdminUserDto>> GetAllUsersAdminAsync(); // Students and Instructors
        Task ToggleCourseActiveAsync(int courseId);
        Task ToggleUserActiveAsync(string userId);
        Task DeleteInstructorAsync(string username);
        Task DeleteCourseAsync(int courseId);
    }
}
