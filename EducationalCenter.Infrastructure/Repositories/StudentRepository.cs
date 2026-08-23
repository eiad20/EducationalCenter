using EducationalCenter.Core.Entities;
using EducationalCenter.Core.Interfaces;
using EducationalCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace EducationalCenter.Infrastructure.Repositories;

public class StudentRepository : Repository<Student>, IStudentRepository
{
    public StudentRepository(AppDbContext context) : base(context)
    {
    }
    
    public async Task<Student?> GetStudentWithEnrollmentsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Students
            .Include(s => s.Enrollments) 
            .ThenInclude(e => e.Class) 
            .ThenInclude(c => c.Course)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
}