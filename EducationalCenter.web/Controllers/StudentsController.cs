using AutoMapper;
using EducationalCenter.Core.Entities;
using EducationalCenter.Core.Interfaces;
using EducationalCenter.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EducationalCenter.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper; // Add AutoMapper

    public StudentsController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StudentResponseDto>>> GetAllStudents()
    {
        var students = await _unitOfWork.Students.ListAllAsync();
        
        // Let AutoMapper do the work
        var studentDtos = _mapper.Map<IReadOnlyList<StudentResponseDto>>(students);

        return Ok(studentDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StudentResponseDto>> GetStudentById(int id)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(id);

        if (student == null)
        {
            return NotFound();
        }

        var studentDto = _mapper.Map<StudentResponseDto>(student);

        return Ok(studentDto);
    }

    [HttpPost]
    public async Task<ActionResult<StudentResponseDto>> CreateStudent(CreateStudentRequestDto request)
    {
        // Map DTO to Entity
        var newStudent = _mapper.Map<Student>(request);

        await _unitOfWork.Students.AddAsync(newStudent);
        await _unitOfWork.SaveChangesAsync();

        // Map Entity back to DTO
        var responseDto = _mapper.Map<StudentResponseDto>(newStudent);

        return CreatedAtAction(nameof(GetStudentById), new { id = newStudent.Id }, responseDto);
    }
}