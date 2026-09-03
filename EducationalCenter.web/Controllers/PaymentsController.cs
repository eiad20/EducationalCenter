using AutoMapper;
using EducationalCenter.Core.Entities;
using EducationalCenter.Core.Interfaces;
using EducationalCenter.Shared.DTOs;
using EducationalCenter.Shared.Exceptions;
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
        return Ok(_mapper.Map<IReadOnlyList<PaymentResponseDto>>(payments));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentResponseDto>> GetPaymentById(int id)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(id)
            ?? throw new NotFoundException($"Payment with ID {id} was not found.");

        return Ok(_mapper.Map<PaymentResponseDto>(payment));
    }

    [HttpPost]
    public async Task<ActionResult<PaymentResponseDto>> CreatePayment(CreatePaymentRequestDto request)
    {
        // Data Validation
        if (request.Amount <= 0)
            throw new BadRequestException("Payment amount must be greater than zero.");
            
        if (request.EnrollmentId <= 0)
            throw new BadRequestException("A valid Enrollment ID is required to process a payment.");

        var newPayment = _mapper.Map<Payment>(request);
        await _unitOfWork.Payments.AddAsync(newPayment);
        await _unitOfWork.SaveChangesAsync();

        var responseDto = _mapper.Map<PaymentResponseDto>(newPayment);
        return CreatedAtAction(nameof(GetPaymentById), new { id = newPayment.Id }, responseDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePayment(int id)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(id)
            ?? throw new NotFoundException($"Payment with ID {id} was not found.");

        await _unitOfWork.Payments.DeleteAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}