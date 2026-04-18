namespace EduCore.Business.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int Quota { get; set; }
        public string Status { get; set; } = "Draft";
        public string CategoryName { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public string InstructorId { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class CreateCourseDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int Quota { get; set; }
        public int CategoryId { get; set; }
    }
}
