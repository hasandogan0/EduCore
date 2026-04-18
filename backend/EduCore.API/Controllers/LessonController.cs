using EduCore.Business.DTOs;
using EduCore.Business.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduCore.API.Controllers;

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
        // Servis katmanında zaten AutoMapper ile LessonDto'ya dönüştürüldü
        var lessons = await _lessonService.GetLessonsByCourseIdAsync(courseId);
        return Ok(lessons);
    }

    [HttpPost]
    [Authorize(Roles = "Instructor,SuperAdmin")]
    public async Task<IActionResult> AddLesson(CreateLessonDto lessonDto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = await _courseService.GetCourseByIdAsync(lessonDto.CourseId);

            if (course == null) return NotFound(new { Message = "Kurs bulunamadı." });

            // Güvenlik: Eğitmen sadece kendi kursuna ders ekleyebilir
            if (!User.IsInRole("SuperAdmin") && course.InstructorId != userId)
            {
                return Forbid();
            }

            var lesson = await _lessonService.AddLessonAsync(lessonDto);
            return Ok(new { Message = "Ders başarıyla eklendi.", Data = lesson });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Instructor,SuperAdmin")]
    public async Task<IActionResult> DeleteLesson(int id)
    {
        try
        {
            var lesson = await _lessonService.GetLessonByIdAsync(id);
            if (lesson == null) return NotFound(new { Message = "Ders bulunamadı." });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = await _courseService.GetCourseByIdAsync(lesson.CourseId);

            if (course == null) return NotFound(new { Message = "İlgili kurs bulunamadı." });
            if (!User.IsInRole("SuperAdmin") && course.InstructorId != userId)
            {
                return Forbid();
            }

            await _lessonService.DeleteLessonAsync(id);
            return Ok(new { Message = "Ders başarıyla silindi." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Instructor,SuperAdmin")]
    public async Task<IActionResult> UpdateLesson(int id, CreateLessonDto lessonDto)
    {
        try
        {
            var lesson = await _lessonService.GetLessonByIdAsync(id);
            if (lesson == null) return NotFound(new { Message = "Ders bulunamadı." });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = await _courseService.GetCourseByIdAsync(lesson.CourseId);

            if (course == null) return NotFound(new { Message = "İlgili kurs bulunamadı." });
            if (!User.IsInRole("SuperAdmin") && course.InstructorId != userId)
            {
                return Forbid();
            }

            await _lessonService.UpdateLessonAsync(id, lessonDto);
            return Ok(new { Message = "Ders başarıyla güncellendi." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}