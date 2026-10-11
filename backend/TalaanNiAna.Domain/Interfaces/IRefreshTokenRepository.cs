public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);
    Task<RefreshToken?> GetByHashAsync(string tokenHash);

    /// <summary>Revokes a token and records what replaced it.</summary>
    Task RevokeAsync(RefreshToken token, string? replacedByHash = null);

    /// <summary>Revokes every live token for a user. Used on reuse detection and logout-all.</summary>
    Task RevokeAllForUserAsync(string userId);
}
