using EducationalCenter.Core.Entities;
using EducationalCenter.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EducationalCenter.web.Pages;

public class StudentsModel : PageModel
{
    private readonly IUnitOfWork _unitOfWork;

    public StudentsModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IReadOnlyList<Student> Students { get; set; } = new List<Student>();

    [BindProperty]
    public string NewFirstName { get; set; } = string.Empty;

    [BindProperty]
    public string NewLastName { get; set; } = string.Empty;

    [BindProperty]
    public string NewEmail { get; set; } = string.Empty;

    [BindProperty]
    public string? NewPhoneNumber { get; set; }

    public string? SuccessMessage { get; set; }

    public async Task OnGetAsync()
    {
        Students = await _unitOfWork.Students.ListAllAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var student = new Student
        {
            FirstName = NewFirstName,
            LastName = NewLastName,
            Email = NewEmail,
            PhoneNumber = NewPhoneNumber
        };

        await _unitOfWork.Students.AddAsync(student);
        await _unitOfWork.SaveChangesAsync();

        SuccessMessage = $"Student '{NewFirstName} {NewLastName}' was added.";
        Students = await _unitOfWork.Students.ListAllAsync();
        return Page();
    }
}
