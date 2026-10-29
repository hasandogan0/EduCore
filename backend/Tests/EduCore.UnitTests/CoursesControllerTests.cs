using System.Threading.Tasks;
using EduCore.API.Controllers;
using EduCore.Business.DTOs;
using EduCore.Business.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EduCore.UnitTests;

public class CoursesControllerTests
{
    [Fact]
    public async Task GetAllCourses_ReturnsOkWithCourses()
    {
        var courses = new[]
        {
            new CourseDto
            {
                Id = 1,
                Title = "Test course",
                Status = "Published",
                IsActive = true
            }
        };
        var service = new Mock<ICourseService>();
        service.Setup(x => x.GetAllCoursesAsync()).ReturnsAsync(courses);
        var controller = new CoursesController(service.Object);

        var result = await controller.GetAllCourses();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(courses, ok.Value);
        service.Verify(x => x.GetAllCoursesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetCourseById_WhenCourseDoesNotExist_ReturnsNotFound()
    {
        var service = new Mock<ICourseService>();
        service.Setup(x => x.GetCourseByIdAsync(42)).ReturnsAsync((CourseDto?)null);
        var controller = new CoursesController(service.Object);

        var result = await controller.GetCourseById(42);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFound.StatusCode);
        service.Verify(x => x.GetCourseByIdAsync(42), Times.Once);
    }
}
