namespace EducationalCenter.Core.Entities;

public class Course
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public string Name { get; set; }
    public string Grade { get; set; }
    public ICollection<Class> Classes { get; set; } = new List<Class>();
}