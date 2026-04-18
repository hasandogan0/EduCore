using AutoMapper;
using EduCore.Business.DTOs;
using EduCore.Business.Services.Abstract;
using EduCore.DataAccess.Repositories;
using EduCore.Entity;
using Microsoft.AspNetCore.Identity;

namespace EduCore.Business.Services.Concrete;

public class AdminService : IAdminService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IMapper _mapper; // AutoMapper eklendi

    public AdminService(UserManager<ApplicationUser> userManager,
                        IRepository<Course> courseRepository,
                        IRepository<Enrollment> enrollmentRepository,
                        IMapper mapper) // Inject edildi
    {
        _userManager = userManager;
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AuthResponseDto>> GetPendingInstructorsAsync()
    {
        var instructors = await _userManager.GetUsersInRoleAsync("Instructor");
        var pendingOnes = instructors.Where(u => !u.IsApproved);

        // Manuel Select yerine AutoMapper
        return _mapper.Map<IEnumerable<AuthResponseDto>>(pendingOnes);
    }

    public async Task ApproveInstructorAsync(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) throw new Exception("Kullanıcı bulunamadı.");

        user.IsApproved = true;
        await _userManager.UpdateAsync(user);
    }

    public async Task<IEnumerable<CourseDto>> GetPendingCoursesAsync()
    {
        var courses = await _courseRepository.FindAsync(c => c.Status == CourseStatus.Pending);

        // AutoMapper kullanımı
        return _mapper.Map<IEnumerable<CourseDto>>(courses);
    }

    public async Task ApproveCourseAsync(int courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null) throw new Exception("Kurs bulunamadı.");

        course.Status = CourseStatus.Published;
        await _courseRepository.UpdateAsync(course);
        await _courseRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<AdminCourseDto>> GetAllCoursesAdminAsync()
    {
        var courses = await _courseRepository.GetAllAsync();
        var dtos = _mapper.Map<List<AdminCourseDto>>(courses);

        foreach (var dto in dtos)
        {
            // İlgili kursun verisini bulalım
            var course = courses.First(c => c.Id == dto.Id);

            // Eğitmen bilgisini çekelim
            var instructor = await _userManager.FindByIdAsync(course.InstructorId);
            dto.Instructor = instructor?.FullName ?? "Bilinmiyor";

            // Kazanç hesaplama
            var enrollments = await _enrollmentRepository.FindAsync(e => e.CourseId == dto.Id);
            dto.TotalEarnings = enrollments.Count() * dto.Price;
        }

        return dtos;
    }

    public async Task<IEnumerable<AdminUserDto>> GetAllUsersAdminAsync()
    {
        var users = _userManager.Users.ToList();
        var dtos = _mapper.Map<List<AdminUserDto>>(users);

        foreach (var dto in dtos)
        {
            var user = users.First(u => u.Id == dto.Id);
            var roles = await _userManager.GetRolesAsync(user);
            dto.Role = roles.FirstOrDefault() ?? "Üye";

            // Eğitmense toplam kazancını hesapla
            if (dto.Role == "Instructor")
            {
                var instructorCourses = await _courseRepository.FindAsync(c => c.InstructorId == user.Id);
                decimal total = 0;
                foreach (var c in instructorCourses)
                {
                    var ens = await _enrollmentRepository.FindAsync(e => e.CourseId == c.Id);
                    total += ens.Count() * c.Price;
                }
                dto.TotalEarnings = total;
            }
        }
        return dtos;
    }

    public async Task ToggleCourseActiveAsync(int courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null) throw new Exception("Kurs bulunamadı.");

        course.IsActive = !course.IsActive;
        await _courseRepository.UpdateAsync(course);
        await _courseRepository.SaveChangesAsync();
    }

    public async Task ToggleUserActiveAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new Exception("Kullanıcı bulunamadı.");

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains("Admin") || roles.Contains("SuperAdmin"))
        {
            throw new Exception("Yönetici hesapları dondurulamaz.");
        }

        user.IsActive = !user.IsActive;
        await _userManager.UpdateAsync(user);
    }

    public async Task DeleteCourseAsync(int courseId)
    {
        var enrollments = await _enrollmentRepository.FindAsync(e => e.CourseId == courseId);
        foreach (var e in enrollments) await _enrollmentRepository.DeleteAsync(e.Id);
        await _enrollmentRepository.SaveChangesAsync();

        await _courseRepository.DeleteAsync(courseId);
        await _courseRepository.SaveChangesAsync();
    }

    public async Task DeleteInstructorAsync(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) throw new Exception("Kullanıcı bulunamadı.");

        var courses = await _courseRepository.FindAsync(c => c.InstructorId == user.Id);
        foreach (var c in courses) await DeleteCourseAsync(c.Id);

        await _userManager.DeleteAsync(user);
    }
}