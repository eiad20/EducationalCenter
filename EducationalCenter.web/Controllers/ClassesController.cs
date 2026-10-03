using AutoMapper;
using EducationalCenter.Core.Entities;
using EducationalCenter.Core.Interfaces;
using EducationalCenter.Shared.DTOs;
using EducationalCenter.Shared.Exceptions;
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
        var classEntity = await _unitOfWork.Classes.GetByIdAsync(id) 
            ?? throw new NotFoundException($"Class with ID {id} was not found.");

        var classDto = _mapper.Map<ClassResponseDto>(classEntity);
        return Ok(classDto);
    }

    [HttpPost]
    public async Task<ActionResult<ClassResponseDto>> CreateClass(CreateClassRequestDto request)
    {
        if (request.Capacity <= 0)
            throw new BadRequestException("Capacity must be greater than zero.");
    
        if (request.EndDate <= request.StartDate)
            throw new BadRequestException("End date must be after start date.");

        _ = await _unitOfWork.Courses.GetByIdAsync(request.CourseId) 
            ?? throw new NotFoundException($"Course with ID {request.CourseId} not found.");
        
        _ = await _unitOfWork.Instructors.GetByIdAsync(request.InstructorId) 
            ?? throw new NotFoundException($"Instructor with ID {request.InstructorId} not found.");

        var newClass = _mapper.Map<Class>(request);
        await _unitOfWork.Classes.AddAsync(newClass);
        await _unitOfWork.SaveChangesAsync();

        var responseDto = _mapper.Map<ClassResponseDto>(newClass);
        return CreatedAtAction(nameof(GetClassById), new { id = newClass.Id }, responseDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClass(int id, CreateClassRequestDto request)
    {
        if (request.Capacity <= 0)
            throw new BadRequestException("Capacity must be greater than zero.");

        if (request.EndDate <= request.StartDate)
            throw new BadRequestException("End date must be after start date.");

        var existingClass = await _unitOfWork.Classes.GetByIdAsync(id)
            ?? throw new NotFoundException($"Class with ID {id} was not found.");

        _ = await _unitOfWork.Courses.GetByIdAsync(request.CourseId)
            ?? throw new NotFoundException($"Course with ID {request.CourseId} not found.");

        _ = await _unitOfWork.Instructors.GetByIdAsync(request.InstructorId)
            ?? throw new NotFoundException($"Instructor with ID {request.InstructorId} not found.");

        _mapper.Map(request, existingClass);

        await _unitOfWork.Classes.UpdateAsync(existingClass);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClass(int id)
    {
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
        _ = await _unitOfWork.Classes.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Class with ID {id} was not found.");

        // Query only enrollments belonging to this class
        var enrollments = await _unitOfWork.Enrollments.FindAsync(e => e.ClassId == id, cancellationToken);
        var enrolledStudentIds = enrollments.Select(e => e.StudentId).Distinct().ToHashSet();

        if (enrolledStudentIds.Count == 0)
            return Ok(Array.Empty<StudentResponseDto>());

        // Query only the matching students from the database
        var studentsInClass = await _unitOfWork.Students.FindAsync(s => enrolledStudentIds.Contains(s.Id), cancellationToken);

        var result = _mapper.Map<IReadOnlyList<StudentResponseDto>>(studentsInClass);
        return Ok(result);
    }
}