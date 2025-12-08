namespace Eylul_Webproje.Models
{
    public class Module
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
