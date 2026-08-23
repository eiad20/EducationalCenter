using EducationalCenter.Core.Entities;
using EducationalCenter.Core.Interfaces;
using EducationalCenter.Infrastructure.Data;

namespace EducationalCenter.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _context;

    public IStudentRepository Students { get; private set; }
    public IRepository<Course> Courses { get; private set; }
    public IRepository<Class> Classes { get; private set; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        
        Students = new StudentRepository(_context);
        Courses = new Repository<Course>(_context);
        Classes = new Repository<Class>(_context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
      
        return await _context.SaveChangesAsync(cancellationToken);
    }

  
    public void Dispose()
    {
        _context.Dispose();
    }
}