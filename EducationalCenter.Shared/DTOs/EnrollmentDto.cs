namespace EducationalCenter.Shared.DTOs;

public record EnrollmentResponseDto(
    int Id,
    int StudentId,
    int ClassId,
    string Status,
    DateTime EnrollmentDate);

public record CreateEnrollmentRequestDto(
    int StudentId,
    int ClassId);