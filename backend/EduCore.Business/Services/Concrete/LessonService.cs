using AutoMapper;
using EduCore.Business.DTOs;
using EduCore.Business.Services.Abstract;
using EduCore.DataAccess.Repositories;
using EduCore.Entity;

namespace EduCore.Business.Services.Concrete;

public class LessonService : ILessonService
{
    private readonly IRepository<Lesson> _lessonRepository;
    private readonly IMapper _mapper; // AutoMapper eklendi

    public LessonService(IRepository<Lesson> lessonRepository, IMapper mapper) // Inject edildi
    {
        _lessonRepository = lessonRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LessonDto>> GetLessonsByCourseIdAsync(int courseId)
    {
        var lessons = await _lessonRepository.FindAsync(l => l.CourseId == courseId);

        // Manuel Select(MapToDto) yerine AutoMapper
        return _mapper.Map<IEnumerable<LessonDto>>(lessons);
    }

    public async Task<LessonDto> AddLessonAsync(CreateLessonDto lessonDto)
    {
        // CreateLessonDto -> Lesson (Entity)
        var lesson = _mapper.Map<Lesson>(lessonDto);

        await _lessonRepository.AddAsync(lesson);
        await _lessonRepository.SaveChangesAsync();

        // Lesson (Entity) -> LessonDto
        return _mapper.Map<LessonDto>(lesson);
    }

    public async Task UpdateLessonAsync(int id, CreateLessonDto lessonDto)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        if (lesson == null) throw new Exception("Ders bulunamadı.");

        // Mevcut nesneyi DTO'daki verilerle güncelle
        _mapper.Map(lessonDto, lesson);

        await _lessonRepository.UpdateAsync(lesson);
        await _lessonRepository.SaveChangesAsync();
    }

    public async Task<LessonDto?> GetLessonByIdAsync(int id)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        return _mapper.Map<LessonDto>(lesson);
    }

    public async Task DeleteLessonAsync(int id)
    {
        await _lessonRepository.DeleteAsync(id);
        await _lessonRepository.SaveChangesAsync();
    }
}