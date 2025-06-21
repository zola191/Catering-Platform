namespace Catering.Platform.Applications.Abstractions;

public interface IHashService
{
    string HashPassword(string password);
    bool Verify(string password, string passwordHash);
}
