namespace EducationalCenter.Shared.DTOs;

public record StudentPaymentHistoryDto(
    int PaymentId,
    decimal Amount,
    DateTime Date,
    string PaymentMethod,
    string Status,
    int ClassId,
    string CourseName);