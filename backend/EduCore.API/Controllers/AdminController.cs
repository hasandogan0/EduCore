using EduCore.Business.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("pending-instructors")]
        public async Task<IActionResult> GetPendingInstructors()
        {
            var instructors = await _adminService.GetPendingInstructorsAsync();
            return Ok(instructors);
        }

        [HttpPost("approve-instructor/{username}")]
        public async Task<IActionResult> ApproveInstructor(string username)
        {
            try
            {
                await _adminService.ApproveInstructorAsync(username);
                return Ok("Instructor approved successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("pending-courses")]
        public async Task<IActionResult> GetPendingCourses()
        {
            var courses = await _adminService.GetPendingCoursesAsync();
            return Ok(courses);
        }

        [HttpPost("approve-course/{courseId}")]
        public async Task<IActionResult> ApproveCourse(int courseId)
        {
            try
            {
                await _adminService.ApproveCourseAsync(courseId);
                return Ok("Course approved successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("all-courses")]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _adminService.GetAllCoursesAdminAsync();
            return Ok(courses);
        }

        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAdminAsync();
            return Ok(users);
        }

        [HttpPost("toggle-course/{courseId}")]
        public async Task<IActionResult> ToggleCourse(int courseId)
        {
            await _adminService.ToggleCourseActiveAsync(courseId);
            return Ok("Course status toggled");
        }

        [HttpPost("toggle-user/{userId}")]
        public async Task<IActionResult> ToggleUser(string userId)
        {
            await _adminService.ToggleUserActiveAsync(userId);
            return Ok("User status toggled");
        }

        [HttpDelete("instructor/{username}")]
        public async Task<IActionResult> DeleteInstructor(string username)
        {
            try
            {
                await _adminService.DeleteInstructorAsync(username);
                return Ok("Instructor deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("course/{courseId}")]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            try
            {
                await _adminService.DeleteCourseAsync(courseId);
                return Ok("Course deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
