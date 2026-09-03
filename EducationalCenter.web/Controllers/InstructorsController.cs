using AutoMapper;
using EducationalCenter.Core.Entities;
using EducationalCenter.Core.Interfaces;
using EducationalCenter.Shared.DTOs;
using EducationalCenter.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EducationalCenter.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InstructorsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public InstructorsController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<InstructorResponseDto>>> GetAllInstructors()
    {
        var instructors = await _unitOfWork.Instructors.ListAllAsync();
        return Ok(_mapper.Map<IReadOnlyList<InstructorResponseDto>>(instructors));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InstructorResponseDto>> GetInstructorById(int id)
    {
        var instructor = await _unitOfWork.Instructors.GetByIdAsync(id)
            ?? throw new NotFoundException($"Instructor with ID {id} was not found.");

        return Ok(_mapper.Map<InstructorResponseDto>(instructor));
    }

    [HttpPost]
    public async Task<ActionResult<InstructorResponseDto>> CreateInstructor(CreateInstructorRequestDto request)
    {
        // Data Validation
        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
            throw new BadRequestException("Instructor first and last names cannot be empty.");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new BadRequestException("Instructor email address is required.");

        var newInstructor = _mapper.Map<Instructor>(request);
        await _unitOfWork.Instructors.AddAsync(newInstructor);
        await _unitOfWork.SaveChangesAsync();

        var responseDto = _mapper.Map<InstructorResponseDto>(newInstructor);
        return CreatedAtAction(nameof(GetInstructorById), new { id = newInstructor.Id }, responseDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInstructor(int id, CreateInstructorRequestDto request)
    {
        // Data Validation
        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
            throw new BadRequestException("Instructor first and last names cannot be empty.");

        // Not Found Validation
        var existingInstructor = await _unitOfWork.Instructors.GetByIdAsync(id)
            ?? throw new NotFoundException($"Instructor with ID {id} was not found.");

        _mapper.Map(request, existingInstructor);
        await _unitOfWork.Instructors.UpdateAsync(existingInstructor);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInstructor(int id)
    {
        var instructor = await _unitOfWork.Instructors.GetByIdAsync(id)
            ?? throw new NotFoundException($"Instructor with ID {id} was not found.");

        await _unitOfWork.Instructors.DeleteAsync(instructor);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}