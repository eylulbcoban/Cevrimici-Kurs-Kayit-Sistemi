namespace Eylul_Webproje.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public int StudentId { get; set; }   // int FK
        public Student Student { get; set; }

        public DateTime EnrollDate { get; set; } = DateTime.Now;
    }
}
