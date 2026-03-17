using HotelBookingSystem.Domain.EnumsNew;

namespace HotelBookingSystem.Application.DTOsNew.CheckInOutNew;

public class ReservationStatusResultDto
{
    public Guid ReservationId { get; set; }
    public string ReservationNumber { get; set; } = default!;
    public ReservationStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
}