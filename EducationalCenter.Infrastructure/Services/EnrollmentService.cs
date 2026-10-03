using EducationalCenter.Core.Entities;
using EducationalCenter.Core.Interfaces;
using EducationalCenter.Core.Enums;
using EducationalCenter.Shared.Exceptions;

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
        var student = await _unitOfWork.Students.GetByIdAsync(studentId, cancellationToken);
        if (student == null)
            throw new NotFoundException($"Student with ID {studentId} not found.");

        var targetClass = await _unitOfWork.Classes.GetByIdAsync(classId, cancellationToken);
        if (targetClass == null)
            throw new NotFoundException($"Class with ID {classId} not found.");

        var allEnrollments = await _unitOfWork.Enrollments.ListAllAsync(cancellationToken);

        bool isAlreadyEnrolled = allEnrollments.Any(e => e.StudentId == studentId && e.ClassId == classId);
        if (isAlreadyEnrolled)
            throw new ConflictException("Student is already enrolled in this class.");

        int currentEnrollmentsCount = allEnrollments.Count(e => e.ClassId == classId && e.Status == EnrollmentStatus.Active);
        if (currentEnrollmentsCount >= targetClass.Capacity)
            throw new BadRequestException("The class has reached its maximum capacity.");

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            ClassId = classId,
            Status = EnrollmentStatus.Active,
            EnrollmentDate = DateTime.UtcNow
        };

        await _unitOfWork.Enrollments.AddAsync(enrollment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}