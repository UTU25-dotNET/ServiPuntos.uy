using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public string GenerateJwtToken(IEnumerable<Claim> claims)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"];

        // Asegurarse de que la clave tenga el tamaño adecuado
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        Console.WriteLine($"Longitud de la clave en bytes: {keyBytes.Length}, bits: {keyBytes.Length * 8}");

        var key = new SymmetricSecurityKey(keyBytes);

        // Verificar el tamaño de la clave
        Console.WriteLine($"KeySize reportado por SymmetricSecurityKey: {key.KeySize} bits");

        if (key.KeySize < 128)
        {
            throw new InvalidOperationException($"La clave secreta es demasiado corta. Tamaño actual: {key.KeySize} bits. Se requieren al menos 128 bits.");
        }

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["JwtSettings:ExpirationHours"])),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}