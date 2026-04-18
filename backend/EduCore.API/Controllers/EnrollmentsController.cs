using EduCore.Business.DTOs;
using EduCore.DataAccess.Repositories;
using EduCore.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EduCore.Business.Services.Abstract;

namespace EduCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Category> _categoryRepository;

        public EnrollmentsController(IEnrollmentService enrollmentService, 
            UserManager<ApplicationUser> userManager,
            IRepository<Category> categoryRepository)
        {
            _enrollmentService = enrollmentService;
            _userManager = userManager;
            _categoryRepository = categoryRepository;
        }

        [HttpPost("{courseId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentId)) return Unauthorized();

            try
            {
                await _enrollmentService.EnrollStudentAsync(courseId, studentId);
                return Ok("Enrolled successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("check/{courseId}")]
        [Authorize]
        public async Task<IActionResult> IsEnrolled(int courseId)
        {
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentId)) return Unauthorized();

            var result = await _enrollmentService.IsStudentEnrolledAsync(courseId, studentId);
            return Ok(result);
        }

        [HttpGet("my-courses")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyCourses()
        {
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentId)) return Unauthorized();

            var courses = await _enrollmentService.GetStudentEnrollmentsAsync(studentId);
            
            // Map to DTOs manually to avoid JSON cycles and populate names
            var dtos = new List<CourseDto>();
            foreach (var course in courses)
            {
                var category = await _categoryRepository.GetByIdAsync(course.CategoryId);
                var instructor = await _userManager.FindByIdAsync(course.InstructorId);

                dtos.Add(new CourseDto
                {
                    Id = course.Id,
                    Title = course.Title,
                    Description = course.Description,
                    Price = course.Price,
                    ImageUrl = course.ImageUrl,
                    Quota = course.Quota,
                    Status = course.Status.ToString(),
                    CategoryName = category?.Name ?? "Unknown",
                    InstructorName = instructor?.FullName ?? "Unknown",
                    InstructorId = course.InstructorId
                });
            }

            return Ok(dtos);
        }

        [HttpPost("progress/{courseId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UpdateProgress(int courseId, [FromBody] int progress)
        {
             var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
             if (string.IsNullOrEmpty(studentId)) return Unauthorized();
             
             await _enrollmentService.UpdateProgressAsync(courseId, studentId, progress);
             return Ok();
        }

        [HttpGet("progress/{courseId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetProgress(int courseId)
        {
             var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
             if (string.IsNullOrEmpty(studentId)) return Unauthorized();
             
             var progress = await _enrollmentService.GetEnrollmentProgressAsync(courseId, studentId);
             return Ok(progress);
        }
    }
}
