using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Tokens;
using Microsoft.IdentityModel.Tokens;

namespace CashFlow.Infrastructure.Security.Tokens;

internal class JwtTokenGenerator : IAccessTokenGenerator
{
    private readonly uint _expirationTimeMinutes;
    private readonly string _signingKey;

    public JwtTokenGenerator(string signingKey, uint expirationTimeMinutes)
    {
        _signingKey = signingKey;
        _expirationTimeMinutes = expirationTimeMinutes;
    }

    public string Generate(User user)
    {
        // Create the claims (= payload) for the token
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Sid, user.UserIdentifier.ToString()),
        };

        // Token generation config
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddMinutes(_expirationTimeMinutes),
            SigningCredentials = new SigningCredentials(SecurityKey(), SecurityAlgorithms.HmacSha256Signature),
            Subject = new ClaimsIdentity(claims)
        };

        // Create the token
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);

        // Return the token as a string
        return tokenHandler.WriteToken(securityToken);
    }

    private SymmetricSecurityKey SecurityKey()
    {
        var keyBytes = System.Text.Encoding.UTF8.GetBytes(_signingKey);

        return new SymmetricSecurityKey(keyBytes);
    }
}