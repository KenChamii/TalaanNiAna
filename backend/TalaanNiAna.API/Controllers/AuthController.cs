using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TalaanNiAna.Application.Common.Exceptions;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting(RateLimitPolicies.Auth)]
public class AuthController : ControllerBase
{
    private const string RefreshCookieName = "tna_refresh";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokens,
        IWebHostEnvironment environment,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _refreshTokens = refreshTokens;
        _environment = environment;
        _logger = logger;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResultDto>> Register([FromBody] RegisterDto dto)
    {
        if (await _userManager.FindByNameAsync(dto.Username) is not null)
            throw new ConflictException("Ginamit na ang username na ito. Pumili ng iba.", "username_taken");

        var user = new ApplicationUser
        {
            UserName = dto.Username,
            Email = dto.Username,
            DisplayName = dto.DisplayName,
            StoreName = dto.StoreName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors
                .GroupBy(e => e.Code.Contains("Password") ? nameof(dto.Password) : nameof(dto.Username))
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

            throw new RequestValidationException(errors);
        }

        return await IssueAsync(user);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResultDto>> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.Username)
                   ?? await _userManager.FindByEmailAsync(dto.Username);

        if (user is null)
            return Unauthorized(InvalidCredentials());

        // Goes through SignInManager so Identity's lockout counter actually runs.
        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            _logger.LogWarning("Locked out login attempt for {UserId}", user.Id);
            return StatusCode(StatusCodes.Status423Locked, new ProblemDetails
            {
                Status = StatusCodes.Status423Locked,
                Title = "Naka-lock ang account",
                Detail = "Masyadong maraming maling subok. Subukan ulit mamaya.",
                Extensions = { ["code"] = "locked_out" }
            });
        }

        if (!result.Succeeded)
            return Unauthorized(InvalidCredentials());

        return await IssueAsync(user);
    }

    /// <summary>Exchanges the refresh cookie for a new access token, rotating the refresh token.</summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResultDto>> Refresh()
    {
        if (!Request.Cookies.TryGetValue(RefreshCookieName, out var raw) || string.IsNullOrWhiteSpace(raw))
            return Unauthorized(SessionExpired());

        var stored = await _refreshTokens.GetByHashAsync(_tokenService.Hash(raw));

        if (stored is null)
            return Unauthorized(SessionExpired());

        // A revoked token being presented again means it leaked. Drop the whole family.
        if (!stored.IsActive)
        {
            _logger.LogWarning("Reused refresh token for {UserId}; revoking all sessions", stored.UserId);
            await _refreshTokens.RevokeAllForUserAsync(stored.UserId);
            ClearRefreshCookie();
            return Unauthorized(SessionExpired());
        }

        var rotated = _tokenService.CreateRefreshToken();
        await _refreshTokens.RevokeAsync(stored, rotated.Hash);

        return await IssueAsync(stored.User, rotated);
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout()
    {
        if (Request.Cookies.TryGetValue(RefreshCookieName, out var raw) && !string.IsNullOrWhiteSpace(raw))
        {
            var stored = await _refreshTokens.GetByHashAsync(_tokenService.Hash(raw));
            if (stored is { RevokedAt: null })
                await _refreshTokens.RevokeAsync(stored);
        }

        ClearRefreshCookie();
        return NoContent();
    }

    private async Task<ActionResult<AuthResultDto>> IssueAsync(
        ApplicationUser user,
        RefreshTokenPair? rotated = null)
    {
        var refresh = rotated ?? _tokenService.CreateRefreshToken();

        await _refreshTokens.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refresh.Hash,
            ExpiresAt = refresh.ExpiresAt
        });

        SetRefreshCookie(refresh);

        var access = _tokenService.CreateAccessToken(user);

        return Ok(new AuthResultDto(
            access.Value,
            access.ExpiresInSeconds,
            user.DisplayName,
            user.StoreName));
    }

    private void SetRefreshCookie(RefreshTokenPair refresh)
        => Response.Cookies.Append(RefreshCookieName, refresh.RawValue, CookieOptions(refresh.ExpiresAt));

    private void ClearRefreshCookie()
        => Response.Cookies.Delete(RefreshCookieName, CookieOptions(DateTime.UtcNow.AddDays(-1)));

    private CookieOptions CookieOptions(DateTime expires) => new()
    {
        HttpOnly = true,
        Expires = expires,
        Path = "/api/auth",
        // The deployed frontend is on another origin, which needs None; None needs Secure,
        // and Secure breaks plain-http localhost. So dev relaxes both.
        Secure = !_environment.IsDevelopment(),
        SameSite = _environment.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.None
    };

    private static ProblemDetails InvalidCredentials() => new()
    {
        Status = StatusCodes.Status401Unauthorized,
        Title = "Hindi makapasok",
        Detail = "Mali ang username o password.",
        Extensions = { ["code"] = "invalid_credentials" }
    };

    private static ProblemDetails SessionExpired() => new()
    {
        Status = StatusCodes.Status401Unauthorized,
        Title = "Tapos na ang session",
        Detail = "Mag-login ulit.",
        Extensions = { ["code"] = "session_expired" }
    };
}
