using System.Text.Json.Serialization;

namespace EducationalCenter.Core.Entities;

public class Class
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Schedule { get; set; } = null!;
    public int Capacity { get; set; }
    
    public int CourseId { get; set; }
    
    [JsonIgnore]
    public Course? Course { get; set; }
    
    public int InstructorId { get; set; }
    
    [JsonIgnore]
    public Instructor? Instructor { get; set; }
    
    [JsonIgnore]
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}