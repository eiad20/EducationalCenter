using AutoMapper;
using EducationalCenter.Core.Entities;
using EducationalCenter.Core.Interfaces;
using EducationalCenter.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EducationalCenter.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CoursesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CoursesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CourseResponseDto>>> GetAllCourses()
    {
        var courses = await _unitOfWork.Courses.ListAllAsync();
        var courseDtos = _mapper.Map<IReadOnlyList<CourseResponseDto>>(courses);
        
        return Ok(courseDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseResponseDto>> GetCourseById(int id)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(id);

        if (course == null)
        {
            return NotFound();
        }

        var courseDto = _mapper.Map<CourseResponseDto>(course);
        return Ok(courseDto);
    }

    [HttpPost]
    public async Task<ActionResult<CourseResponseDto>> CreateCourse(CreateCourseRequestDto request)
    {
        var newCourse = _mapper.Map<Course>(request);

        await _unitOfWork.Courses.AddAsync(newCourse);
        await _unitOfWork.SaveChangesAsync();

        var responseDto = _mapper.Map<CourseResponseDto>(newCourse);

        return CreatedAtAction(nameof(GetCourseById), new { id = newCourse.Id }, responseDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(int id, CreateCourseRequestDto request)
    {
        var existingCourse = await _unitOfWork.Courses.GetByIdAsync(id);
        if (existingCourse == null)
        {
            return NotFound();
        }

        // Map the updated fields from the DTO to the existing entity
        _mapper.Map(request, existingCourse);

        await _unitOfWork.Courses.UpdateAsync(existingCourse);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(id);
        if (course == null)
        {
            return NotFound();
        }

        await _unitOfWork.Courses.DeleteAsync(course);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}