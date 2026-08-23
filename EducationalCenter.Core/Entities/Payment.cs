using EducationalCenter.Core.Enums;

namespace EducationalCenter.Core.Entities;

public class Payment
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; }
    public int EnrollmentId { get; set; }
    public Enrollment Enrollment { get; set; }
}