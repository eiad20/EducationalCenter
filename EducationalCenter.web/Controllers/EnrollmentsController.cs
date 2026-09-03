using AutoMapper;
using EducationalCenter.Core.Interfaces;
using EducationalCenter.Shared.DTOs;
using EducationalCenter.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EducationalCenter.web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EnrollmentsController(IEnrollmentService enrollmentService, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _enrollmentService = enrollmentService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // GET: api/enrollments
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EnrollmentResponseDto>>> GetAllEnrollments(CancellationToken cancellationToken)
    {
        var enrollments = await _unitOfWork.Enrollments.ListAllAsync(cancellationToken);
        var dtos = _mapper.Map<IReadOnlyList<EnrollmentResponseDto>>(enrollments);
        
        return Ok(dtos);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterStudent([FromBody] CreateEnrollmentRequestDto request, CancellationToken cancellationToken)
    {
        // 1. Throw BadRequest if IDs are invalid
        if (request.StudentId <= 0 || request.ClassId <= 0)
        {
            throw new BadRequestException("Invalid Student ID or Class ID.");
        }

        var success = await _enrollmentService.EnrollStudentAsync(request.StudentId, request.ClassId, cancellationToken);

        // 2. Throw Conflict if business rules (capacity/duplicates) fail
        if (!success)
        {
            throw new ConflictException("Enrollment failed. The student is already registered, or the class is at full capacity.");
        }

        return Ok(new { message = "Student successfully enrolled!" });
    }
}