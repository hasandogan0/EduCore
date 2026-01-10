using EduCore.Business.DTOs;
using EduCore.DataAccess.Repositories;
using EduCore.Entity;

namespace EduCore.Business.Services
{
    public class LessonService : ILessonService
    {
        private readonly IRepository<Lesson> _lessonRepository;

        public LessonService(IRepository<Lesson> lessonRepository)
        {
            _lessonRepository = lessonRepository;
        }

        public async Task<IEnumerable<LessonDto>> GetLessonsByCourseIdAsync(int courseId)
        {
            var lessons = await _lessonRepository.FindAsync(l => l.CourseId == courseId);
            return lessons.Select(MapToDto);
        }

        public async Task<LessonDto> AddLessonAsync(CreateLessonDto lessonDto)
        {
            var lesson = new Lesson
            {
                Title = lessonDto.Title,
                VideoUrl = lessonDto.VideoUrl,
                CourseId = lessonDto.CourseId
            };

            await _lessonRepository.AddAsync(lesson);
            await _lessonRepository.SaveChangesAsync();

            return MapToDto(lesson);
        }

        public async Task DeleteLessonAsync(int id)
        {
            await _lessonRepository.DeleteAsync(id);
            await _lessonRepository.SaveChangesAsync();
        }

        public async Task UpdateLessonAsync(int id, CreateLessonDto lessonDto)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            if (lesson != null)
            {
                lesson.Title = lessonDto.Title;
                lesson.VideoUrl = lessonDto.VideoUrl;
                
                await _lessonRepository.UpdateAsync(lesson);
                await _lessonRepository.SaveChangesAsync();
            }
        }

        public async Task<LessonDto?> GetLessonByIdAsync(int id)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            return lesson == null ? null : MapToDto(lesson);
        }

        private LessonDto MapToDto(Lesson lesson)
        {
            return new LessonDto
            {
                Id = lesson.Id,
                Title = lesson.Title,
                VideoUrl = lesson.VideoUrl,
                CourseId = lesson.CourseId
            };
        }
    }
}
