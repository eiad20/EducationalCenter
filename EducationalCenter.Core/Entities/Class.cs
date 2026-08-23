namespace EducationalCenter.Core.Entities;

public class Class
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Schedule { get; set; } = null!;
    public int Capacity { get; set; }
    
    public int CourseId  { get; set; }
    public Course Course { get; set; } = null!;
    public int InstructorId { get; set; }
    public Instructor Instructor { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}