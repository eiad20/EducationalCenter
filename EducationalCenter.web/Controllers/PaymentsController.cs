using AutoMapper;
using EducationalCenter.Core.Entities;
using EducationalCenter.Core.Interfaces;
using EducationalCenter.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EducationalCenter.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PaymentsController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PaymentResponseDto>>> GetAllPayments()
    {
        var payments = await _unitOfWork.Payments.ListAllAsync();
        var paymentDtos = _mapper.Map<IReadOnlyList<PaymentResponseDto>>(payments);

        return Ok(paymentDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentResponseDto>> GetPaymentById(int id)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(id);

        if (payment == null)
        {
            return NotFound();
        }

        var paymentDto = _mapper.Map<PaymentResponseDto>(payment);
        return Ok(paymentDto);
    }

    [HttpPost]
    public async Task<ActionResult<PaymentResponseDto>> CreatePayment(CreatePaymentRequestDto request)
    {
        var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(request.EnrollmentId);
        if (enrollment == null)
        {
            return BadRequest($"Enrollment with ID {request.EnrollmentId} does not exist.");
        }

        var newPayment = _mapper.Map<Payment>(request);
        newPayment.Date = DateTime.UtcNow; // Using your exact property name

        await _unitOfWork.Payments.AddAsync(newPayment);
        await _unitOfWork.SaveChangesAsync();

        var responseDto = _mapper.Map<PaymentResponseDto>(newPayment);

        return CreatedAtAction(nameof(GetPaymentById), new { id = newPayment.Id }, responseDto);
    }
}