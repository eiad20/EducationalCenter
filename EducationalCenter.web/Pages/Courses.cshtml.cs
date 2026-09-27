using EducationalCenter.Core.Entities;
using EducationalCenter.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EducationalCenter.web.Pages;

public class CoursesModel : PageModel
{
    private readonly IUnitOfWork _unitOfWork;

    public CoursesModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IReadOnlyList<Course> Courses { get; set; } = new List<Course>();

    [BindProperty]
    public string NewName { get; set; } = string.Empty;

    [BindProperty]
    public string NewGrade { get; set; } = string.Empty;

    [BindProperty]
    public decimal NewPrice { get; set; }

    public string? SuccessMessage { get; set; }

    public async Task OnGetAsync()
    {
        Courses = await _unitOfWork.Courses.ListAllAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var course = new Course
        {
            Name = NewName,
            Grade = NewGrade,
            Price = NewPrice
        };

        await _unitOfWork.Courses.AddAsync(course);
        await _unitOfWork.SaveChangesAsync();

        SuccessMessage = $"Course '{NewName}' was added.";
        Courses = await _unitOfWork.Courses.ListAllAsync();
        return Page();
    }
}
