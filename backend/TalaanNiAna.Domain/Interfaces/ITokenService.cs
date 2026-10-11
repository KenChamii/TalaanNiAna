public interface ITokenService
{
    /// <summary>Short-lived JWT for the Authorization header.</summary>
    AccessToken CreateAccessToken(ApplicationUser user);

    /// <summary>New refresh token. The raw value goes in the cookie, the hash in the database.</summary>
    RefreshTokenPair CreateRefreshToken();

    /// <summary>Hashes a raw refresh token so it can be looked up.</summary>
    string Hash(string rawToken);
}

public readonly record struct AccessToken(string Value, int ExpiresInSeconds);

public readonly record struct RefreshTokenPair(string RawValue, string Hash, DateTime ExpiresAt);
