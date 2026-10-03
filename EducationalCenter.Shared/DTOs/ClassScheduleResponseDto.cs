namespace EducationalCenter.Shared.DTOs;

public record ClassScheduleResponseDto(
    int ClassId,
    string CourseName,
    string InstructorName,
    string Schedule,
    DateTime StartDate,
    DateTime EndDate,
    int Capacity,
    int EnrolledCount,
    int AvailableSpots);