using System.ComponentModel.DataAnnotations;
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
    [Required(ErrorMessage = "First name is required.")]
    public string NewFirstName { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Last name is required.")]
    public string NewLastName { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Email is required."), EmailAddress(ErrorMessage = "Invalid email format.")]
    public string NewEmail { get; set; } = string.Empty;

    [BindProperty]
    public string? NewPhoneNumber { get; set; }

    public string? SuccessMessage { get; set; }

    public async Task OnGetAsync()
    {
        Instructors = await _unitOfWork.Instructors.ListAllAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Instructors = await _unitOfWork.Instructors.ListAllAsync();
            return Page();
        }

        var instructor = new Instructor
        {
            FirstName = NewFirstName,
            LastName = NewLastName,
            Email = NewEmail,
            PhoneNumber = NewPhoneNumber ?? string.Empty
        };

        await _unitOfWork.Instructors.AddAsync(instructor);
        await _unitOfWork.SaveChangesAsync();

        SuccessMessage = $"Instructor '{NewFirstName} {NewLastName}' was added.";
        Instructors = await _unitOfWork.Instructors.ListAllAsync();

        return Page();
    }
}