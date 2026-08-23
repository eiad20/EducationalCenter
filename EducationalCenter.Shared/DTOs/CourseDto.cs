namespace EducationalCenter.Shared.DTOs;

public record CourseResponseDto(
    int Id, 
    string Name, 
    decimal Price, 
    string Grade);

public record CreateCourseRequestDto(
    string Name, 
    decimal Price, 
    string Grade);