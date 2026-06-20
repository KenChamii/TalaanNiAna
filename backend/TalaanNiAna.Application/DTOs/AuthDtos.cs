public record LoginDto(string Username, string Password);

public record RegisterDto(
    string Username,
    string Password,
    string DisplayName,
    string StoreName
);