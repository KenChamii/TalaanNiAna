using Microsoft.EntityFrameworkCore;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RefreshToken token)
    {
        _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByHashAsync(string tokenHash)
        => await _context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);

    public async Task RevokeAsync(RefreshToken token, string? replacedByHash = null)
    {
        token.RevokedAt = DateTime.UtcNow;
        token.ReplacedByHash = replacedByHash;
        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllForUserAsync(string userId)
    {
        await _context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ExecuteUpdateAsync(set => set.SetProperty(t => t.RevokedAt, DateTime.UtcNow));
    }
}
