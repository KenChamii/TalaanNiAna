public record LoginDto(string Username, string Password);

public record RegisterDto(
    string Username,
    string Password,
    string DisplayName,
    string StoreName
);

/// <summary>Returned by login, register and refresh. The refresh token travels in a cookie.</summary>
public record AuthResultDto(
    string AccessToken,
    int ExpiresInSeconds,
    string DisplayName,
    string StoreName
);
