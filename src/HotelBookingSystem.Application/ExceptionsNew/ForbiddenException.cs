namespace HotelBookingSystem.Application.ExceptionsNew;

public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message)
    {
    }
}