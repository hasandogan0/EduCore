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
}
