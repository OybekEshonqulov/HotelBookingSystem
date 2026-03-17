using HotelBookingSystem.Application.DTOsNew.PaymentNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
using HotelBookingSystem.Infrastructure.ServicesNew;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ICurrentUserService _currentUserService;

    public PaymentsController(IPaymentService paymentService, ICurrentUserService currentUserService)
    {
        _paymentService = paymentService;
        _currentUserService = currentUserService;
    }

    [HttpGet("reservation/{reservationId:guid}")]
    public async Task<IActionResult> GetByReservation(Guid reservationId, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains("payments.view"))
            return Forbid();
        var result = await _paymentService.GetByReservationAsync(reservationId, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _paymentService.CreateAsync(request, cancellationToken);
        return Ok(result);
    }
}