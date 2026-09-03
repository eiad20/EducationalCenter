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
        // New Exception Pattern
        var student = await _unitOfWork.Students.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Student with ID {id} was not found.");

        var enrollments = (await _unitOfWork.Enrollments.ListAllAsync(cancellationToken))
            .Where(e => e.StudentId == id)
            .ToDictionary(e => e.Id);

        var payments = await _unitOfWork.Payments.ListAllAsync(cancellationToken);
        var studentPayments = payments.Where(p => enrollments.ContainsKey(p.EnrollmentId)).ToList();

        var classes = (await _unitOfWork.Classes.ListAllAsync(cancellationToken)).ToDictionary(c => c.Id);
        var courses = (await _unitOfWork.Courses.ListAllAsync(cancellationToken)).ToDictionary(c => c.Id);

        var history = studentPayments.Select(p =>
        {
            var enrollment = enrollments[p.EnrollmentId];
            var classObj = classes.TryGetValue(enrollment.ClassId, out var cl) ? cl : null;
            var courseName = (classObj != null && courses.TryGetValue(classObj.CourseId, out var cr)) 
                ? cr.Name 
                : "Unknown";

            return new StudentPaymentHistoryDto(
                p.Id,
                p.Amount,
                p.Date,
                p.PaymentMethod.ToString(),
                p.Status.ToString(),
                enrollment.ClassId,
                courseName
            );
        }).ToList();

        return Ok(history);
    }
}