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
public class ClassesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ClassesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClassResponseDto>>> GetAllClasses()
    {
        var classes = await _unitOfWork.Classes.ListAllAsync();
        var classDtos = _mapper.Map<IReadOnlyList<ClassResponseDto>>(classes);
        
        return Ok(classDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClassResponseDto>> GetClassById(int id)
    {
        // New Exception Pattern
        var classEntity = await _unitOfWork.Classes.GetByIdAsync(id) 
            ?? throw new NotFoundException($"Class with ID {id} was not found.");

        var classDto = _mapper.Map<ClassResponseDto>(classEntity);
        return Ok(classDto);
    }

    [HttpPost]
    public async Task<ActionResult<ClassResponseDto>> CreateClass(CreateClassRequestDto request)
    {
        var newClass = _mapper.Map<Class>(request);

        await _unitOfWork.Classes.AddAsync(newClass);
        await _unitOfWork.SaveChangesAsync();

        var responseDto = _mapper.Map<ClassResponseDto>(newClass);

        return CreatedAtAction(nameof(GetClassById), new { id = newClass.Id }, responseDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClass(int id, CreateClassRequestDto request)
    {
        // New Exception Pattern
        var existingClass = await _unitOfWork.Classes.GetByIdAsync(id)
            ?? throw new NotFoundException($"Class with ID {id} was not found.");

        _mapper.Map(request, existingClass);

        await _unitOfWork.Classes.UpdateAsync(existingClass);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClass(int id)
    {
        // New Exception Pattern
        var classEntity = await _unitOfWork.Classes.GetByIdAsync(id)
            ?? throw new NotFoundException($"Class with ID {id} was not found.");

        await _unitOfWork.Classes.DeleteAsync(classEntity);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/classes/schedule
    [HttpGet("schedule")]
    public async Task<ActionResult<IReadOnlyList<ClassScheduleResponseDto>>> GetClassSchedule(CancellationToken cancellationToken = default)
    {
        var classes = await _unitOfWork.Classes.ListAllAsync(cancellationToken);
        var courses = (await _unitOfWork.Courses.ListAllAsync(cancellationToken)).ToDictionary(c => c.Id);
        var instructors = (await _unitOfWork.Instructors.ListAllAsync(cancellationToken)).ToDictionary(i => i.Id);
        var enrollments = await _unitOfWork.Enrollments.ListAllAsync(cancellationToken);

        var schedule = classes.Select(c =>
        {
            var courseName = courses.TryGetValue(c.CourseId, out var crs) ? crs.Name : "Unknown";
            var instructorName = instructors.TryGetValue(c.InstructorId, out var inst) 
                ? $"{inst.FirstName} {inst.LastName}" 
                : "Unknown";
            var enrolledCount = enrollments.Count(e => e.ClassId == c.Id);

            return new ClassScheduleResponseDto(
                c.Id,
                courseName,
                instructorName,
                c.Schedule,
                c.StartDate,
                c.EndDate,
                c.Capacity,
                enrolledCount,
                Math.Max(0, c.Capacity - enrolledCount)
            );
        }).ToList();

        return Ok(schedule);
    }

    // GET: api/classes/{id}/students
    [HttpGet("{id}/students")]
    public async Task<ActionResult<IReadOnlyList<StudentResponseDto>>> GetStudentsInClass(int id, CancellationToken cancellationToken = default)
    {
        // New Exception Pattern
        var classExists = await _unitOfWork.Classes.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Class with ID {id} was not found.");

        var enrollments = await _unitOfWork.Enrollments.ListAllAsync(cancellationToken);
        var enrolledStudentIds = enrollments
            .Where(e => e.ClassId == id)
            .Select(e => e.StudentId)
            .ToHashSet();

        var allStudents = await _unitOfWork.Students.ListAllAsync(cancellationToken);
        var studentsInClass = allStudents.Where(s => enrolledStudentIds.Contains(s.Id)).ToList();

        var result = _mapper.Map<IReadOnlyList<StudentResponseDto>>(studentsInClass);
        return Ok(result);
    }
}