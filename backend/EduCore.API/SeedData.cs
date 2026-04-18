using EduCore.DataAccess.Data;
using EduCore.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduCore.API
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<EduCoreDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();

            // Siding Roles
            string[] roles = { "SuperAdmin", "Instructor", "Student" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed SuperAdmin
            var adminEmail = "admin@educore.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    FullName = "Super Admin",
                    IsApproved = true,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
            }

            // Seed Categories
            if (!context.Categories.Any())
            {
                var categories = new Category[]
                {
                    new Category { Name = "Development", Description = "Software Development Courses" },
                    new Category { Name = "Design", Description = "Graphic and UI/UX Design" },
                    new Category { Name = "Business", Description = "Business and Entrepreneurship" }
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Seed Instructor
            var instructorEmail = "angela@educore.com";
            var instructorUser = await userManager.FindByEmailAsync(instructorEmail);
            if (instructorUser == null)
            {
                instructorUser = new ApplicationUser
                {
                    UserName = "angela",
                    Email = instructorEmail,
                    FullName = "Dr. Angela Yu",
                    IsApproved = true,
                    Bio = "Lead Instructor at App Brewery",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(instructorUser, "Pass123!");
                await userManager.AddToRoleAsync(instructorUser, "Instructor");
            }

            // Seed Courses
            if (!context.Courses.Any() && instructorUser != null)
            {
                var categories = await context.Categories.ToListAsync();
                
                var courses = new Course[]
                {
                    new Course 
                    { 
                        Title = "The Complete Python Bootcamp", 
                        Description = "Learn Python like a Professional! Start from the basics and go all the way to creating your own applications and games.", 
                        Price = 29.99m, 
                        CategoryId = categories.First(c => c.Name == "Development").Id, 
                        InstructorId = instructorUser.Id,
                        ImageUrl = "https://img-c.udemycdn.com/course/750x422/567828_67d0.jpg",
                        Status = CourseStatus.Published,
                        Quota = 100
                    },
                    new Course 
                    { 
                        Title = "Machine Learning Specialization", 
                        Description = "Build ML models with NumPy & scikit-learn, build neural networks with TensorFlow.", 
                        Price = 49.99m, 
                        CategoryId = categories.First(c => c.Name == "Development").Id, 
                        InstructorId = instructorUser.Id,
                        ImageUrl = "https://img-c.udemycdn.com/course/750x422/950358_302f.jpg",
                        Status = CourseStatus.Published,
                        Quota = 50
                    },
                    new Course 
                    { 
                        Title = "Complete Web Design 2024", 
                        Description = "Master Web Design with HTML5, CSS3 and modern UI patterns.", 
                        Price = 19.99m, 
                        CategoryId = categories.First(c => c.Name == "Design").Id, 
                        InstructorId = instructorUser.Id,
                        ImageUrl = "https://img-c.udemycdn.com/course/750x422/1561458_7f3b.jpg",
                        Status = CourseStatus.Published,
                        Quota = 200
                    }
                };
                context.Courses.AddRange(courses);
                await context.SaveChangesAsync();
            }
        }
    }
}
