using EducationalCenter.Core.Entities;

namespace EducationalCenter.Core.Interfaces;


public interface IStudentRepository : IRepository<Student>
{
    Task<Student?> GetStudentWithEnrollmentsAsync(int id, CancellationToken cancellationToken = default);
}