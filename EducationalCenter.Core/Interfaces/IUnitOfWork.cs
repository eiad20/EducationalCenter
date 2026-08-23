using EducationalCenter.Core.Entities;

namespace EducationalCenter.Core.Interfaces;

public interface IUnitOfWork 
{
    IStudentRepository Students { get; }
    IRepository<Course> Courses { get; }
    IRepository<Class> Classes { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}