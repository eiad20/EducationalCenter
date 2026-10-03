using System.ComponentModel.DataAnnotations;
using EducationalCenter.Core.Entities;
using EducationalCenter.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EducationalCenter.web.Pages;

public class ClassesModel : PageModel
{
    private readonly IUnitOfWork _unitOfWork;

    public ClassesModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public class ClassRow
    {
        public int Id { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public string Schedule { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        public int Enrolled { get; set; }
    }

    public List<ClassRow> Classes { get; set; } = new();
    public List<Course> Courses { get; set; } = new();
    public List<Instructor> Instructors { get; set; } = new();

    [BindProperty]
    [Required, Range(1, int.MaxValue, ErrorMessage = "Please select a valid course.")]
    public int NewCourseId { get; set; }

    [BindProperty]
    [Required, Range(1, int.MaxValue, ErrorMessage = "Please select a valid instructor.")]
    public int NewInstructorId { get; set; }

    [BindProperty]
    [Required]
    public DateTime NewStartDate { get; set; } = DateTime.Today;

    [BindProperty]
    [Required]
    public DateTime NewEndDate { get; set; } = DateTime.Today.AddMonths(1);

    [BindProperty]
    [Required(ErrorMessage = "Schedule is required.")]
    public string NewSchedule { get; set; } = string.Empty;

    [BindProperty]
    [Range(1, 500, ErrorMessage = "Capacity must be greater than zero.")]
    public int NewCapacity { get; set; } = 20;

    public string? SuccessMessage { get; set; }

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (NewEndDate <= NewStartDate)
        {
            ModelState.AddModelError(nameof(NewEndDate), "End date must be after start date.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        var newClass = new Class
        {
            CourseId = NewCourseId,
            InstructorId = NewInstructorId,
            StartDate = NewStartDate,
            EndDate = NewEndDate,
            Schedule = NewSchedule,
            Capacity = NewCapacity
        };

        await _unitOfWork.Classes.AddAsync(newClass);
        await _unitOfWork.SaveChangesAsync();

        SuccessMessage = "Class was created.";
        await LoadAsync();
        return Page();
    }

    private async Task LoadAsync()
    {
        Courses = (await _unitOfWork.Courses.ListAllAsync()).ToList();
        Instructors = (await _unitOfWork.Instructors.ListAllAsync()).ToList();
        var classes = await _unitOfWork.Classes.ListAllAsync();
        var enrollments = await _unitOfWork.Enrollments.ListAllAsync();

        var coursesById = Courses.ToDictionary(c => c.Id);
        var instructorsById = Instructors.ToDictionary(i => i.Id);

        Classes = classes.Select(c => new ClassRow
        {
            Id = c.Id,
            CourseName = coursesById.TryGetValue(c.CourseId, out var course) ? course.Name : "Unknown",
            InstructorName = instructorsById.TryGetValue(c.InstructorId, out var instr) ? $"{instr.FirstName} {instr.LastName}" : "Unknown",
            Schedule = c.Schedule,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            Capacity = c.Capacity,
            Enrolled = enrollments.Count(e => e.ClassId == c.Id)
        }).ToList();
    }
}