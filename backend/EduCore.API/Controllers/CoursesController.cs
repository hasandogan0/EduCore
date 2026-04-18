using EduCore.Business.DTOs;
using EduCore.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        [HttpGet("my-courses")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> GetInstructorCourses()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var courses = await _courseService.GetInstructorCoursesAsync(userId);
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null) return NotFound();
            return Ok(course);
        }

        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> Create([FromBody] CreateCourseDto courseDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isApproved = User.FindFirst("IsApproved")?.Value == "True";

            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Additional security check: Is Instructor Approved?
            // Ideally this should be a Policy, but manual check works for now.
            if (!isApproved)
            {
                // Re-check from DB to be safe (token might be old)
                // But for now, trust the token claims or refresh logic
                return StatusCode(403, "Instructor account is not approved yet.");
            }

            await _courseService.CreateCourseAsync(courseDto, userId);
            
            return Ok("Course created successfully (Pending Admin Approval)");
        }

        [HttpGet("stats")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> GetInstructorStats()
        {
             var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
             if (string.IsNullOrEmpty(userId)) return Unauthorized();

             var stats = await _courseService.GetInstructorStatsAsync(userId);
             return Ok(stats);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CreateCourseDto courseDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                await _courseService.UpdateCourseAsync(id, courseDto, userId);
                return Ok("Course updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                await _courseService.DeleteCourseAsync(id, userId);
                return Ok("Course deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/toggle")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> ToggleCourseStatus(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                await _courseService.ToggleCourseStatusAsync(id, userId);
                return Ok("Course status toggled");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
