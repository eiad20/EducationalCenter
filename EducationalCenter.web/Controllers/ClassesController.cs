using AutoMapper;
using EducationalCenter.Core.Entities;
using EducationalCenter.Core.Interfaces;
using EducationalCenter.Shared.DTOs;
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
        var classEntity = await _unitOfWork.Classes.GetByIdAsync(id);

        if (classEntity == null)
        {
            return NotFound();
        }

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
        var existingClass = await _unitOfWork.Classes.GetByIdAsync(id);
        if (existingClass == null)
        {
            return NotFound();
        }

        _mapper.Map(request, existingClass);

        await _unitOfWork.Classes.UpdateAsync(existingClass);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClass(int id)
    {
        var classEntity = await _unitOfWork.Classes.GetByIdAsync(id);
        if (classEntity == null)
        {
            return NotFound();
        }

        await _unitOfWork.Classes.DeleteAsync(classEntity);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}