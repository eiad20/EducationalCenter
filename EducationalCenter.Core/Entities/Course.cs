namespace EducationalCenter.Core.Entities;

public class Course
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;

    public ICollection<Class> Classes { get; set; } = new List<Class>();
}