namespace EducationalCenter.Shared.DTOs;

public record ClassResponseDto(
    int Id, 
    DateTime StartDate, 
    DateTime EndDate, 
    string Schedule, 
    int Capacity, 
    int CourseId, 
    int InstructorId);

public record CreateClassRequestDto(
    DateTime StartDate, 
    DateTime EndDate, 
    string Schedule, 
    int Capacity, 
    int CourseId, 
    int InstructorId);