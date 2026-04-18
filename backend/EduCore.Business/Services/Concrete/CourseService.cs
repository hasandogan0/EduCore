using EduCore.Business.DTOs;
using EduCore.Business.Services.Abstract;
using EduCore.DataAccess.Repositories;
using EduCore.Entity;
using Microsoft.AspNetCore.Identity;

namespace EduCore.Business.Services.Concrete
{
    public class CourseService : ICourseService
    {

        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Enrollment> _enrollmentRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CourseService(IRepository<Course> courseRepository,
                             IRepository<Category> categoryRepository,
                             IRepository<Enrollment> enrollmentRepository,
                             UserManager<ApplicationUser> userManager)
        {
            _courseRepository = courseRepository;
            _categoryRepository = categoryRepository;
            _enrollmentRepository = enrollmentRepository;
            _userManager = userManager;
        }

        public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
        {
            // Only Published AND Active courses
            // And Instructor must be Active
            var courses = await _courseRepository.FindAsync(c => c.Status == CourseStatus.Published && c.IsActive);
            
            // Further filter by Instructor Active status (since repo find might not include instructor)
            var activeCourses = new List<Course>();
            foreach(var c in courses)
            {
                var instructor = await _userManager.FindByIdAsync(c.InstructorId);
                if(instructor != null && instructor.IsActive)
                {
                    activeCourses.Add(c);
                }
            }
            
            return await MapToDtos(activeCourses);
        }

        public async Task<IEnumerable<CourseDto>> GetInstructorCoursesAsync(string instructorId)
        {
            // Instructor Dashboard: All their courses
            var courses = await _courseRepository.FindAsync(c => c.InstructorId == instructorId);
            return await MapToDtos(courses);
        }

        public async Task<CourseDto?> GetCourseByIdAsync(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null) return null;

            // In a real app, map manually or use Include. 
            // Here we fetch related data manually for simplicity with Generic Repo.
            var category = await _categoryRepository.GetByIdAsync(course.CategoryId);
            var instructor = await _userManager.FindByIdAsync(course.InstructorId);

            return new CourseDto
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
                InstructorId = course.InstructorId,
                IsActive = course.IsActive
            };
        }

        public async Task CreateCourseAsync(CreateCourseDto courseDto, string instructorId)
        {
            var course = new Course
            {
                Title = courseDto.Title,
                Description = courseDto.Description,
                Price = courseDto.Price,
                ImageUrl = courseDto.ImageUrl,
                Quota = courseDto.Quota,
                CategoryId = courseDto.CategoryId,
                InstructorId = instructorId,
                Status = CourseStatus.Pending // Default to Pending per requirements
            };
            
            await _courseRepository.AddAsync(course);
            await _courseRepository.SaveChangesAsync();
        }

        private async Task<IEnumerable<CourseDto>> MapToDtos(IEnumerable<Course> courses)
        {
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
                    InstructorId = course.InstructorId,
                    IsActive = course.IsActive
                });
            }
            return dtos;
        }

        public async Task<InstructorStatsDto> GetInstructorStatsAsync(string instructorId)
        {
            var courses = await _courseRepository.FindAsync(c => c.InstructorId == instructorId);
            var courseIds = courses.Select(c => c.Id).ToList();
            
            var allEnrollments = new List<Enrollment>();
            foreach(var cid in courseIds)
            {
               var courseEnrollments = await _enrollmentRepository.FindAsync(e => e.CourseId == cid);
               allEnrollments.AddRange(courseEnrollments);
            }

            decimal totalEarnings = 0;
            var uniqueStudentIds = new HashSet<string>();

            foreach(var enrollment in allEnrollments)
            {
                var course = courses.FirstOrDefault(c => c.Id == enrollment.CourseId);
                if (course != null)
                {
                    totalEarnings += course.Price;
                }
                uniqueStudentIds.Add(enrollment.StudentId);
            }

            return new InstructorStatsDto
            {
                TotalEarnings = totalEarnings,
                TotalStudents = uniqueStudentIds.Count,
                TotalCourses = courses.Count()
            };
        }

        public async Task UpdateCourseAsync(int courseId, CreateCourseDto courseDto, string instructorId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null) throw new Exception("Course not found");
            if (course.InstructorId != instructorId) throw new Exception("You are not authorized to edit this course");

            course.Title = courseDto.Title;
            course.Description = courseDto.Description;
            course.Price = courseDto.Price;
            course.Quota = courseDto.Quota;
            course.CategoryId = courseDto.CategoryId;
            
            // Only update image if provided
            if (!string.IsNullOrEmpty(courseDto.ImageUrl))
            {
                course.ImageUrl = courseDto.ImageUrl;
            }

            await _courseRepository.UpdateAsync(course);
            await _courseRepository.SaveChangesAsync();
        }

        public async Task DeleteCourseAsync(int courseId, string instructorId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null) throw new Exception("Course not found");
            if (course.InstructorId != instructorId) throw new Exception("You are not authorized to delete this course");

            // Delete Enrollments
            var enrollments = await _enrollmentRepository.FindAsync(e => e.CourseId == courseId);
            foreach(var e in enrollments)
            {
                await _enrollmentRepository.DeleteAsync(e.Id);
            }
            await _enrollmentRepository.SaveChangesAsync();

            // Delete Course
            await _courseRepository.DeleteAsync(courseId);
            await _courseRepository.SaveChangesAsync();
        }

        public async Task ToggleCourseStatusAsync(int courseId, string instructorId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null) throw new Exception("Course not found");
            if (course.InstructorId != instructorId) throw new Exception("You are not authorized to modify this course");

            course.IsActive = !course.IsActive; // Toggle Active/Passive
            await _courseRepository.UpdateAsync(course);
            await _courseRepository.SaveChangesAsync();
        }
    }
}
