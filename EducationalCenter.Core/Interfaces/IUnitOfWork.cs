using EducationalCenter.Core.Entities;

namespace EducationalCenter.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IStudentRepository Students { get; }
    IRepository<Course> Courses { get; }
    IRepository<Class> Classes { get; }
    IRepository<Instructor> Instructors { get; } // Add this line
    IRepository<Enrollment> Enrollments { get; }
    IRepository<Payment> Payments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}