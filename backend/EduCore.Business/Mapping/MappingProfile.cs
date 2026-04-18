using AutoMapper;
using EduCore.Business.DTOs;
using EduCore.Entity;

namespace EduCore.Business.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //Lesson Mappings
        CreateMap<Lesson, LessonDto>().ReverseMap();
        CreateMap<Lesson, CreateLessonDto>().ReverseMap();


        // Course Mappings
        CreateMap<Course, CourseDto>().ReverseMap();
        CreateMap<Course,CreateCourseDto>().ReverseMap();
        CreateMap<Course, AdminCourseDto>().ReverseMap();

        //Instructor Mappings
        CreateMap<Instructor, InstructorDto>().ReverseMap();
        CreateMap<Instructor, InstructorStatsDto>().ReverseMap();

        //User - Auth Mappings
        CreateMap<ApplicationUser, AdminUserDto>().ReverseMap();
        CreateMap<ApplicationUser, AuthResponseDto>().ReverseMap();
        CreateMap<ApplicationUser, LoginDto>().ReverseMap();
        CreateMap<ApplicationUser, RegisterDto>().ReverseMap();
    }
}
