namespace HotelBookingSystem.Application.ExceptionsNew;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }
}