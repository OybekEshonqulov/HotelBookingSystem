namespace HotelBookingSystem.Application.ExceptionsNew;

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}