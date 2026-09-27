using EducationalCenter.Core.Entities;
using EducationalCenter.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EducationalCenter.web.Pages;

public class EnrollModel : PageModel
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEnrollmentService _enrollmentService;

    public EnrollModel(IUnitOfWork unitOfWork, IEnrollmentService enrollmentService)
    {
        _unitOfWork = unitOfWork;
        _enrollmentService = enrollmentService;
    }

    public class ClassOption
    {
        public int Id { get; set; }
        public string Label { get; set; } = "";
    }

    public List<Student> Students { get; set; } = new();
    public List<ClassOption> ClassOptions { get; set; } = new();

    [BindProperty]
    public int SelectedStudentId { get; set; }

    [BindProperty]
    public int SelectedClassId { get; set; }

    public string? Message { get; set; }
    public bool IsError { get; set; }

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var success = await _enrollmentService.EnrollStudentAsync(SelectedStudentId, SelectedClassId);

        if (success)
        {
            Message = "Student successfully enrolled!";
            IsError = false;
        }
        else
        {
            Message = "Enrollment failed \u2014 the student is already registered for this class, or the class is at full capacity.";
            IsError = true;
        }

        await LoadAsync();
        return Page();
    }

    private async Task LoadAsync()
    {
        Students = (await _unitOfWork.Students.ListAllAsync()).ToList();

        var classes = await _unitOfWork.Classes.ListAllAsync();
        var courses = (await _unitOfWork.Courses.ListAllAsync()).ToDictionary(c => c.Id);
        var enrollments = await _unitOfWork.Enrollments.ListAllAsync();

        ClassOptions = classes.Select(c =>
        {
            var courseName = courses.TryGetValue(c.CourseId, out var course) ? course.Name : "Unknown";
            var enrolled = enrollments.Count(e => e.ClassId == c.Id);
            return new ClassOption
            {
                Id = c.Id,
                Label = $"{courseName} \u2014 {c.Schedule} ({enrolled}/{c.Capacity} seats filled)"
            };
        }).ToList();
    }
}
