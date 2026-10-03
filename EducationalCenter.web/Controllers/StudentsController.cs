using System.Linq;
using AutoMapper;
using EducationalCenter.Core.Entities;
using EducationalCenter.Core.Interfaces;
using EducationalCenter.Shared.DTOs;
using EducationalCenter.Shared.Exceptions; // Added Exception using
using Microsoft.AspNetCore.Mvc;

namespace EducationalCenter.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public StudentsController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StudentResponseDto>>> GetAllStudents()
    {
        var students = await _unitOfWork.Students.ListAllAsync();
        var studentDtos = _mapper.Map<IReadOnlyList<StudentResponseDto>>(students);

        return Ok(studentDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StudentResponseDto>> GetStudentById(int id)
    {
        // New Exception Pattern
        var student = await _unitOfWork.Students.GetByIdAsync(id)
            ?? throw new NotFoundException($"Student with ID {id} was not found.");

        var studentDto = _mapper.Map<StudentResponseDto>(student);
        return Ok(studentDto);
    }

    [HttpPost]
    public async Task<ActionResult<StudentResponseDto>> CreateStudent(CreateStudentRequestDto request)
    {
        var newStudent = _mapper.Map<Student>(request);

        await _unitOfWork.Students.AddAsync(newStudent);
        await _unitOfWork.SaveChangesAsync();

        var responseDto = _mapper.Map<StudentResponseDto>(newStudent);

        return CreatedAtAction(nameof(GetStudentById), new { id = newStudent.Id }, responseDto);
    }

    // GET: api/students/{id}/payments
   [HttpGet("{id}/payments")]
public async Task<ActionResult<IReadOnlyList<StudentPaymentHistoryDto>>> GetStudentPaymentHistory(int id, CancellationToken cancellationToken = default)
{
    var student = await _unitOfWork.Students.GetByIdAsync(id, cancellationToken)
        ?? throw new NotFoundException($"Student with ID {id} was not found.");

    // Fetch ONLY this student's enrollments from SQL
    var studentEnrollments = await _unitOfWork.Enrollments.FindAsync(e => e.StudentId == id, cancellationToken);
    if (!studentEnrollments.Any())
        return Ok(Array.Empty<StudentPaymentHistoryDto>());

    var enrollmentIds = studentEnrollments.Select(e => e.Id).ToHashSet();
    var enrollmentsById = studentEnrollments.ToDictionary(e => e.Id);

    // Fetch ONLY payments belonging to this student's enrollments
    var studentPayments = await _unitOfWork.Payments.FindAsync(p => enrollmentIds.Contains(p.EnrollmentId), cancellationToken);

    // Fetch only the relevant classes and courses
    var classIds = studentEnrollments.Select(e => e.ClassId).Distinct().ToHashSet();
    var classes = (await _unitOfWork.Classes.FindAsync(c => classIds.Contains(c.Id), cancellationToken))
        .ToDictionary(c => c.Id);

    var courseIds = classes.Values.Select(c => c.CourseId).Distinct().ToHashSet();
    var courses = (await _unitOfWork.Courses.FindAsync(c => courseIds.Contains(c.Id), cancellationToken))
        .ToDictionary(c => c.Id);

    var history = studentPayments.Select(p =>
    {
        var enrollment = enrollmentsById[p.EnrollmentId];
        var classObj = classes.TryGetValue(enrollment.ClassId, out var cl) ? cl : null;
        var courseName = (classObj != null && courses.TryGetValue(classObj.CourseId, out var cr))
            ? cr.Name
            : "Unknown";

        return new StudentPaymentHistoryDto(
            p.Id,
            p.Amount,
            p.Date,
            p.PaymentMethod,
            p.Status.ToString(),
            enrollment.ClassId,
            courseName
        );
    }).ToList();

    return Ok(history);
}
}