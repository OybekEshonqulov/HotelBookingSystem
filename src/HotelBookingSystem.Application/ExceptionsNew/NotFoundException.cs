namespace HotelBookingSystem.Application.ExceptionsNew;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}