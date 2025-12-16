using Eylul_Webproje.Models;

public class Course
{
    public int Id { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }

    
    public String Category { get; set; }
    

    //  EĞİTMEN FK
    public int InstructorId { get; set; }
    public Instructor Instructor { get; set; }

    public List<Module> Modules { get; set; } = new();
}
