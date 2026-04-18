using AutoMapper;
using EduCore.Business.DTOs;
using EduCore.Business.Services.Abstract;
using EduCore.DataAccess.Repositories;
using EduCore.Entity;
using Microsoft.AspNetCore.Identity;

namespace EduCore.Business.Services.Concrete;

public class CourseService : ICourseService
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper; 

    public CourseService(IRepository<Course> courseRepository,
                         IRepository<Category> categoryRepository,
                         IRepository<Enrollment> enrollmentRepository,
                         UserManager<ApplicationUser> userManager,
                         IMapper mapper)
    {
        _courseRepository = courseRepository;
        _categoryRepository = categoryRepository;
        _enrollmentRepository = enrollmentRepository;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
    {
        var courses = await _courseRepository.FindAsync(c => c.Status == CourseStatus.Published && c.IsActive);

        var activeCourses = new List<Course>();
        foreach (var c in courses)
        {
            var instructor = await _userManager.FindByIdAsync(c.InstructorId);
            if (instructor != null && instructor.IsActive)
            {
                activeCourses.Add(c);
            }
        }

        // Manuel döngü yerine tek satırda mapping
        return _mapper.Map<IEnumerable<CourseDto>>(activeCourses);
    }

    public async Task<IEnumerable<CourseDto>> GetInstructorCoursesAsync(string instructorId)
    {
        var courses = await _courseRepository.FindAsync(c => c.InstructorId == instructorId);
        return _mapper.Map<IEnumerable<CourseDto>>(courses);
    }

    public async Task<CourseDto?> GetCourseByIdAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null) return null;

        // Temel alanları otomatik eşle
        var dto = _mapper.Map<CourseDto>(course);

        // Ekstra bilgileri (CategoryName, InstructorName) manuel doldur (Generic Repo sınırlaması nedeniyle)
        var category = await _categoryRepository.GetByIdAsync(course.CategoryId);
        var instructor = await _userManager.FindByIdAsync(course.InstructorId);

        dto.CategoryName = category?.Name ?? "Bilinmiyor";
        dto.InstructorName = instructor?.FullName ?? "Bilinmiyor";

        return dto;
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto courseDto, string instructorId)
    {
        // DTO -> Entity
        var course = _mapper.Map<Course>(courseDto);

        course.InstructorId = instructorId;
        course.Status = CourseStatus.Pending;
        course.IsActive = true;

        await _courseRepository.AddAsync(course);
        await _courseRepository.SaveChangesAsync();

        // Entity -> DTO (Geriye ID'si dolmuş halini dönüyoruz)
        return _mapper.Map<CourseDto>(course);
    }

    public async Task UpdateCourseAsync(int courseId, CreateCourseDto courseDto, string instructorId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null) throw new Exception("Kurs bulunamadı");
        if (course.InstructorId != instructorId) throw new Exception("Bu kursu düzenleme yetkiniz yok");

        // Mevcut nesneyi DTO'daki verilerle güncelle
        _mapper.Map(courseDto, course);

        await _courseRepository.UpdateAsync(course);
        await _courseRepository.SaveChangesAsync();
    }

    public async Task<InstructorStatsDto> GetInstructorStatsAsync(string instructorId)
    {
        var courses = await _courseRepository.FindAsync(c => c.InstructorId == instructorId);
        var courseIds = courses.Select(c => c.Id).ToList();

        var allEnrollments = new List<Enrollment>();
        foreach (var cid in courseIds)
        {
            var courseEnrollments = await _enrollmentRepository.FindAsync(e => e.CourseId == cid);
            allEnrollments.AddRange(courseEnrollments);
        }

        decimal totalEarnings = 0;
        var uniqueStudentIds = new HashSet<string>();

        foreach (var enrollment in allEnrollments)
        {
            var course = courses.FirstOrDefault(c => c.Id == enrollment.CourseId);
            if (course != null) totalEarnings += course.Price;
            uniqueStudentIds.Add(enrollment.StudentId);
        }

        return new InstructorStatsDto
        {
            TotalEarnings = totalEarnings,
            TotalStudents = uniqueStudentIds.Count,
            TotalCourses = courses.Count()
        };
    }

    public async Task DeleteCourseAsync(int courseId, string instructorId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null) throw new Exception("Kurs bulunamadı");
        if (course.InstructorId != instructorId) throw new Exception("Yetkisiz işlem");

        var enrollments = await _enrollmentRepository.FindAsync(e => e.CourseId == courseId);
        foreach (var e in enrollments) await _enrollmentRepository.DeleteAsync(e.Id);

        await _courseRepository.DeleteAsync(courseId);
        await _courseRepository.SaveChangesAsync();
    }

    public async Task ToggleCourseStatusAsync(int courseId, string instructorId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null) throw new Exception("Kurs bulunamadı");
        if (course.InstructorId != instructorId) throw new Exception("Yetkisiz işlem");

        course.IsActive = !course.IsActive;
        await _courseRepository.UpdateAsync(course);
        await _courseRepository.SaveChangesAsync();
    }
}