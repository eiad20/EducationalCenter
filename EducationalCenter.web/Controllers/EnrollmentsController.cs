using AutoMapper;
using EducationalCenter.Core.Interfaces;
using EducationalCenter.Shared.DTOs;
using EducationalCenter.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EducationalCenter.Web.Controllers;

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
        if (request.StudentId <= 0 || request.ClassId <= 0)
            throw new BadRequestException("Invalid Student ID or Class ID.");

        await _enrollmentService.EnrollStudentAsync(request.StudentId, request.ClassId, cancellationToken);
    
        return Ok(new { message = "Student successfully enrolled!" });
    }
}