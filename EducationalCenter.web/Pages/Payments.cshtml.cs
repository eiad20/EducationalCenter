using System.ComponentModel.DataAnnotations;
using EducationalCenter.Core.Entities;
using EducationalCenter.Core.Enums;
using EducationalCenter.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EducationalCenter.web.Pages;

public class PaymentsModel : PageModel
{
    private readonly IUnitOfWork _unitOfWork;

    public PaymentsModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public class PaymentRow
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
    }

    public class EnrollmentOption
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
    }

    public List<Student> Students { get; set; } = new();
    public List<PaymentRow> Payments { get; set; } = new();
    public List<EnrollmentOption> EnrollmentOptions { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int? StudentId { get; set; }

    [BindProperty]
    [Required, Range(1, int.MaxValue, ErrorMessage = "Please select an enrollment.")]
    public int NewEnrollmentId { get; set; }

    [BindProperty]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal NewAmount { get; set; }

    [BindProperty]
    [Required]
    public string NewPaymentMethod { get; set; } = "Cash";

    public string? SuccessMessage { get; set; }

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        var payment = new Payment
        {
            EnrollmentId = NewEnrollmentId,
            Amount = NewAmount,
            PaymentMethod = NewPaymentMethod,
            Date = DateTime.UtcNow,
            Status = PaymentStatus.Completed
        };

        await _unitOfWork.Payments.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        SuccessMessage = "Payment recorded.";
        await LoadAsync();
        return Page();
    }

    private async Task LoadAsync()
    {
        Students = (await _unitOfWork.Students.ListAllAsync()).ToList();
        var enrollments = await _unitOfWork.Enrollments.ListAllAsync();
        var classes = (await _unitOfWork.Classes.ListAllAsync()).ToDictionary(c => c.Id);
        var courses = (await _unitOfWork.Courses.ListAllAsync()).ToDictionary(c => c.Id);
        var studentsById = Students.ToDictionary(s => s.Id);

        EnrollmentOptions = enrollments.Select(e =>
        {
            var studentName = studentsById.TryGetValue(e.StudentId, out var st) ? $"{st.FirstName} {st.LastName}" : "Unknown";
            var courseName = classes.TryGetValue(e.ClassId, out var cl) && courses.TryGetValue(cl.CourseId, out var cr) ? cr.Name : "Unknown";
            return new EnrollmentOption { Id = e.Id, Label = $"{studentName} — {courseName}" };
        }).ToList();

        if (StudentId.HasValue)
        {
            var myEnrollmentIds = enrollments.Where(e => e.StudentId == StudentId.Value).Select(e => e.Id).ToHashSet();
            var payments = await _unitOfWork.Payments.FindAsync(p => myEnrollmentIds.Contains(p.EnrollmentId));

            Payments = payments.Select(p =>
            {
                var enrollment = enrollments.First(e => e.Id == p.EnrollmentId);
                var courseName = classes.TryGetValue(enrollment.ClassId, out var cl) && courses.TryGetValue(cl.CourseId, out var cr) ? cr.Name : "Unknown";
                return new PaymentRow
                {
                    Id = p.Id,
                    Date = p.Date,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    Status = p.Status.ToString(),
                    CourseName = courseName
                };
            }).ToList();
        }
    }
}