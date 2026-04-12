using HotelBookingSystem.Application.DTOsNew.PaymentNew;
using HotelBookingSystem.Application.InterfacesNew;
using HotelBookingSystem.Application.InterfacesNew.ServicesNew;
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

    [HttpPost("search")]
    public async Task<IActionResult> GetPaged([FromBody] PaymentFilterRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.PaymentsView))
            return Forbid();

        var result = await _paymentService.GetPagedAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentRequestDto request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.Permissions.Contains(PermissionCodes.PaymentsCreate))
            return Forbid();

        var result = await _paymentService.CreateAsync(request, cancellationToken);
        return Ok(result);
    }
}