namespace Catering.Platform.Applications.Abstractions;

public interface IJwtService
{
    string GenerateToken(string email);
}
