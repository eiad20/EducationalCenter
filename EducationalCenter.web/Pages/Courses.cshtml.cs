using System.ComponentModel.DataAnnotations;
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
    [Required(ErrorMessage = "Name is required")]
    public string NewName { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Grade is required")]
    public string NewGrade { get; set; } = string.Empty;

    [BindProperty]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
    public decimal NewPrice { get; set; }

    // Add this property
    public string? SuccessMessage { get; set; }

    public async Task OnGetAsync()
    {
        Courses = await _unitOfWork.Courses.ListAllAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Courses = await _unitOfWork.Courses.ListAllAsync();
            return Page();
        }

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