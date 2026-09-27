using EducationalCenter.Core.Entities;
using EducationalCenter.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EducationalCenter.web.Pages;

public class InstructorsModel : PageModel
{
    private readonly IUnitOfWork _unitOfWork;

    public InstructorsModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IReadOnlyList<Instructor> Instructors { get; set; } = new List<Instructor>();

    [BindProperty]
    public string NewFirstName { get; set; } = string.Empty;

    [BindProperty]
    public string NewLastName { get; set; } = string.Empty;

    [BindProperty]
    public string NewEmail { get; set; } = string.Empty;

    [BindProperty]
    public string NewPhoneNumber { get; set; } = string.Empty;

    public string? SuccessMessage { get; set; }

    public async Task OnGetAsync()
    {
        Instructors = await _unitOfWork.Instructors.ListAllAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var instructor = new Instructor
        {
            FirstName = NewFirstName,
            LastName = NewLastName,
            Email = NewEmail,
            PhoneNumber = NewPhoneNumber
        };

        await _unitOfWork.Instructors.AddAsync(instructor);
        await _unitOfWork.SaveChangesAsync();

        SuccessMessage = $"Instructor '{NewFirstName} {NewLastName}' was added.";
        Instructors = await _unitOfWork.Instructors.ListAllAsync();
        return Page();
    }
}
