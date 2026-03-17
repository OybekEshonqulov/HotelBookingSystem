namespace HotelBookingSystem.Application.InterfacesNew.ServicesNew;

public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}