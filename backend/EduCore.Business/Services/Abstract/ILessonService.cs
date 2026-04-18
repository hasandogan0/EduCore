using EduCore.Business.DTOs;

namespace EduCore.Business.Services.Abstract;

public interface ILessonService
{
    Task<IEnumerable<LessonDto>> GetLessonsByCourseIdAsync(int courseId);
    Task<LessonDto> AddLessonAsync(CreateLessonDto lessonDto);
    Task DeleteLessonAsync(int id);
    Task UpdateLessonAsync(int id, CreateLessonDto lessonDto);
    Task<LessonDto?> GetLessonByIdAsync(int id);
}
