using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TalaanNiAna.Infrastructure.Options;

namespace TalaanNiAna.Infrastructure.Services;

public class TokenService : ITokenService
{
    private const int RefreshTokenBytes = 32;

    private readonly JwtOptions _options;

    public TokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public AccessToken CreateAccessToken(ApplicationUser user)
    {
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)),
            SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim("displayName", user.DisplayName),
                // Every tenant query filter reads this.
                new Claim(StoreClaims.StoreId, user.StoreId.ToString())
            ],
            expires: expires,
            signingCredentials: credentials);

        var seconds = (int)(expires - DateTime.UtcNow).TotalSeconds;
        return new AccessToken(new JwtSecurityTokenHandler().WriteToken(token), seconds);
    }

    public RefreshTokenPair CreateRefreshToken()
    {
        var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(RefreshTokenBytes));
        return new RefreshTokenPair(raw, Hash(raw), DateTime.UtcNow.AddDays(_options.RefreshTokenDays));
    }

    public string Hash(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes);
    }
}
