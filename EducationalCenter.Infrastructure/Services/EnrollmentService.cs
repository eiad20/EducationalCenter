using EducationalCenter.Core.Entities;
using EducationalCenter.Core.Interfaces;
using EducationalCenter.Core.Enums;
using System.Linq;

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
        // 1. Access the repository via the specific property defined in IUnitOfWork
        var targetClass = await _unitOfWork.Classes.GetByIdAsync(classId, cancellationToken);
        if (targetClass == null)
            return false;

        // 2. Fetch enrollments using the available ListAllAsync method
        var allEnrollments = await _unitOfWork.Enrollments.ListAllAsync(cancellationToken);

        // Rule 1: Student not already enrolled
        bool isAlreadyEnrolled = allEnrollments.Any(e => e.StudentId == studentId && e.ClassId == classId);
        if (isAlreadyEnrolled)
            return false;

        // Rule 2: Class capacity check
        int currentEnrollmentsCount = allEnrollments.Count(e => e.ClassId == classId);
        if (currentEnrollmentsCount >= targetClass.Capacity)
            return false;

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            ClassId = classId,
            Status = EnrollmentStatus.Active
        };

        // 3. Add and save using the specific property
        await _unitOfWork.Enrollments.AddAsync(enrollment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}