using EduCore.Business.DTOs;
using EduCore.Business.Services.Abstract;
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var courses = await _courseService.GetInstructorCoursesAsync(userId);
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null) return NotFound(new { Message = "Kurs bulunamadı." });
            return Ok(course);
        }

        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> Create([FromBody] CreateCourseDto courseDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isApproved = User.FindFirst("IsApproved")?.Value.Equals("True", StringComparison.OrdinalIgnoreCase) ?? false;

            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            if (!isApproved)
            {
                return StatusCode(403, new { Message = "Eğitmen hesabınız henüz onaylanmamış." });
            }

            var result = await _courseService.CreateCourseAsync(courseDto, userId);

            return CreatedAtAction(nameof(GetCourseById), new { id = result.Id }, new { Message = "Kurs başarıyla oluşturuldu (Onay bekleniyor)", Data = result });
        }

        [HttpGet("stats")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> GetInstructorStats()
        {
             var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
             if (string.IsNullOrEmpty(userId)) return Unauthorized();

             var stats = await _courseService.GetInstructorStatsAsync(userId);
             return Ok(stats);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CreateCourseDto courseDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            await _courseService.UpdateCourseAsync(id, courseDto, userId);
            return Ok(new { Message = "Kurs güncellendi" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            await _courseService.DeleteCourseAsync(id, userId);
            return Ok(new { Message = "Kurs silindi" });
        }

        [HttpPost("{id}/toggle")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> ToggleCourseStatus(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            await _courseService.ToggleCourseStatusAsync(id, userId);
            return Ok(new { Message = "Kurs durumu değiştirildi" });
        }
    }
}
