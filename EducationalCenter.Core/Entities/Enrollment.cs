using EducationalCenter.Core.Enums;

namespace EducationalCenter.Core.Entities;

public class Enrollment
{
    public int Id { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public EnrollmentStatus Status { get; set; }    
    public int StudentId { get; set; }
    public int ClassId { get; set; }
    
    public Student Student { get; set; } 
    public Class? Class { get; set; }
}