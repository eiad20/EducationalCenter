namespace EducationalCenter.Core.Interfaces;

public interface IEnrollmentService
{
    Task<bool> EnrollStudentAsync(int studentId, int classId, CancellationToken cancellationToken = default);
}