namespace EducationalCenter.Shared.DTOs;

public record InstructorResponseDto(
    int Id, 
    string FirstName, 
    string LastName, 
    string Email, 
    string PhoneNumber);

public record CreateInstructorRequestDto(
    string FirstName, 
    string LastName, 
    string Email, 
    string PhoneNumber);