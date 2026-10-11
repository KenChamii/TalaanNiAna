using System.ComponentModel.DataAnnotations;

namespace TalaanNiAna.Infrastructure.Options;

/// <summary>Bound from configuration and validated at startup, so a missing key fails fast.</summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    /// <summary>HMAC signing key. Supply it through user-secrets locally and env vars in production.</summary>
    [Required(ErrorMessage = "Jwt:Key is required. Set it with dotnet user-secrets or an environment variable.")]
    [MinLength(32, ErrorMessage = "Jwt:Key must be at least 32 characters.")]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Range(5, 1440)]
    public int AccessTokenMinutes { get; set; } = 30;

    [Range(1, 90)]
    public int RefreshTokenDays { get; set; } = 14;
}
