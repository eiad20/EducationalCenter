using EducationalCenter.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EducationalCenter.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [HttpPost("enroll")]
    public async Task<IActionResult> EnrollStudent(int studentId, int classId)
    {
        var result = await _enrollmentService.EnrollStudentAsync(studentId, classId);

        if (!result)
        {
            return BadRequest("Cannot enroll student. Please check if the student or class exists, if the student is already enrolled, or if the class is at full capacity.");
        }

        return Ok("Student enrolled successfully.");
    }
}