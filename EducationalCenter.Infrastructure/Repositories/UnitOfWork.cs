using EducationalCenter.Core.Entities;
using EducationalCenter.Core.Interfaces;
using EducationalCenter.Infrastructure.Data;

namespace EducationalCenter.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private IStudentRepository? _students;
    private IRepository<Course>? _courses;
    private IRepository<Class>? _classes;
    private IRepository<Instructor>? _instructors;
    private IRepository<Enrollment>? _enrollments;
    private IRepository<Payment>? _payments;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IStudentRepository Students => _students ??= new StudentRepository(_context);
    public IRepository<Course> Courses => _courses ??= new Repository<Course>(_context);
    public IRepository<Class> Classes => _classes ??= new Repository<Class>(_context);
    public IRepository<Instructor> Instructors => _instructors ??= new Repository<Instructor>(_context);
    public IRepository<Enrollment> Enrollments => _enrollments ??= new Repository<Enrollment>(_context);
    public IRepository<Payment> Payments => _payments ??= new Repository<Payment>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}