using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Json;
using EduCore.Business.DTOs;
using EduCore.DataAccess.Data;
using EduCore.Entity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EduCore.IntegrationTests;

public class CoursesApiTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public CoursesApiTests(ApiFactory factory)
    {
        _client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EduCoreDbContext>();
        if (!context.Courses.Any())
        {
            context.Users.Add(new ApplicationUser
            {
                Id = "integration-instructor",
                UserName = "integration-instructor",
                NormalizedUserName = "INTEGRATION-INSTRUCTOR",
                Email = "integration@educore.test",
                NormalizedEmail = "INTEGRATION@EDUCORE.TEST",
                IsActive = true,
                IsApproved = true
            });
            context.Courses.Add(new Course
            {
                Title = "Integration test course",
                Description = "Course created for API integration tests.",
                InstructorId = "integration-instructor",
                CategoryId = 1,
                Price = 10,
                Quota = 10,
                Status = CourseStatus.Published,
                IsActive = true
            });
            context.SaveChanges();
        }
    }

    [Fact]
    public async Task GetCourses_ReturnsOkAndSeededCourses()
    {
        var response = await _client.GetAsync("/api/courses");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var courses = await response.Content.ReadFromJsonAsync<List<CourseDto>>();
        Assert.NotNull(courses);
    }

    [Fact]
    public async Task GetMissingCourse_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/courses/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
