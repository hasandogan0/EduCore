using EduCore.Business.DTOs;
using EduCore.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;
        private readonly ICourseService _courseService;

        public LessonController(ILessonService lessonService, ICourseService courseService)
        {
            _lessonService = lessonService;
            _courseService = courseService;
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetLessonsByCourse(int courseId)
        {
            var lessons = await _lessonService.GetLessonsByCourseIdAsync(courseId);
            return Ok(lessons);
        }

        [HttpPost]
        [Authorize(Roles = "Instructor,SuperAdmin")]
        public async Task<IActionResult> AddLesson(CreateLessonDto lessonDto)
        {
            try
            {
                // Security Check: Verify User owns the course
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var course = await _courseService.GetCourseByIdAsync(lessonDto.CourseId);

                if (course == null) return NotFound("Course not found.");
                
                // SuperAdmin can add to any course, Instructor only to their own
                if (!User.IsInRole("SuperAdmin") && course.InstructorId != userId)
                {
                    return Forbid();
                }

                var lesson = await _lessonService.AddLessonAsync(lessonDto);
                return Ok(lesson);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor,SuperAdmin")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            try
            {
                var lesson = await _lessonService.GetLessonByIdAsync(id);
                if (lesson == null) return NotFound("Lesson not found.");

                // Security: Verify ownership of the course the lesson belongs to
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var course = await _courseService.GetCourseByIdAsync(lesson.CourseId);

                if (course == null) return NotFound("Course not found.");
                if (!User.IsInRole("SuperAdmin") && course.InstructorId != userId)
                {
                    return Forbid();
                }

                await _lessonService.DeleteLessonAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor,SuperAdmin")]
        public async Task<IActionResult> UpdateLesson(int id, CreateLessonDto lessonDto)
        {
            try
            {
                var lesson = await _lessonService.GetLessonByIdAsync(id);
                if (lesson == null) return NotFound("Lesson not found.");

                // Security Check
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var course = await _courseService.GetCourseByIdAsync(lesson.CourseId);

                if (course == null) return NotFound("Course not found.");
                if (!User.IsInRole("SuperAdmin") && course.InstructorId != userId)
                {
                    return Forbid();
                }

                await _lessonService.UpdateLessonAsync(id, lessonDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
