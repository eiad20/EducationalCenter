using EducationalCenter.Core.Entities;
using EducationalCenter.Core.Interfaces;
using EducationalCenter.Core.Enums;

namespace EducationalCenter.Infrastructure.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public EnrollmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> EnrollStudentAsync(int studentId, int classId, CancellationToken cancellationToken = default)
    {
        var student = await _unitOfWork.Students.GetStudentWithEnrollmentsAsync(studentId, cancellationToken);
        var targetClass = await _unitOfWork.Classes.GetByIdAsync(classId, cancellationToken);

        if (student == null || targetClass == null)
        {
            return false; 
        }
        if (student.Enrollments.Any(e => e.ClassId == classId))
        {
            return false;
        }


        var allEnrollments = await _unitOfWork.Students.ListAllAsync(cancellationToken); 
       
        
        var newEnrollment = new Enrollment
        {
            StudentId = studentId,
            ClassId = classId,
            EnrollmentDate = DateTime.UtcNow,
            Status = EnrollmentStatus.Active
        };

        student.Enrollments.Add(newEnrollment);

        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}