namespace EducationalCenter.Shared.DTOs;

public record PaymentResponseDto(
    int Id, 
    DateTime Date, 
    decimal Amount, 
    string PaymentMethod, 
    string Status, // Changed from Enum to string
    int EnrollmentId);

public record CreatePaymentRequestDto(
    decimal Amount, 
    string PaymentMethod, 
    string Status, // Changed from Enum to string
    int EnrollmentId);