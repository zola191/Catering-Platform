using Catering.Platform.Applications.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Catering.Platform.Applications.Services;

// классы internal sealed интерфейсы public для класса предотвращает наследовнание
// и не видно за пределами сборки
internal sealed class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    // переделать через IOptions вместо IConfiguration как ДЗ
    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(string email)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expires = Convert.ToInt32(jwtSettings["ExpiryInMinutes"]);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier,"EAAB8A23-9793-4355-96DE-8EBBB9201690"),
            new Claim(ClaimTypes.Name, "Jonh Doe"),
            new Claim(ClaimTypes.Email, email),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(secretKey),
                SecurityAlgorithms.HmacSha256Signature),
            Expires = DateTime.Now.AddMinutes(expires),
            Subject = new ClaimsIdentity(claims)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
