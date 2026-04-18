namespace EduCore.Business.DTOs
{
    public class AdminCourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Instructor { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public decimal TotalEarnings { get; set; }
    }

    public class AdminUserDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public decimal TotalEarnings { get; set; } // Only relevant for instructors
        public string HeadshotUrl { get; set; }
    }
}
