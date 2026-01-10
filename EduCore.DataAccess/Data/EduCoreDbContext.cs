using EduCore.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EduCore.DataAccess.Data
{
    public class EduCoreDbContext : IdentityDbContext<ApplicationUser>
    {
        public EduCoreDbContext(DbContextOptions<EduCoreDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        // public DbSet<Instructor> Instructors { get; set; } // Removing old Instructor entity

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Course>()
                .Property(c => c.Price)
                .HasColumnType("decimal(18,2)");
                
            // New Foreign Key for Instructor (ApplicationUser)
            builder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany() // ApplicationUser doesn't need a collection of courses explicitly unless we want it
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Course>()
                .HasOne(c => c.Category)
                .WithMany(c => c.Courses)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
