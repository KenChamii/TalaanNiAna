namespace TalaanNiAna.API.Auth;

/// <summary>Reads the store id from the access token on the current request.</summary>
public sealed class CurrentStore : ICurrentStore
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentStore(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public int? StoreId
    {
        get
        {
            var claim = _accessor.HttpContext?.User?.FindFirst(StoreClaims.StoreId)?.Value;
            return int.TryParse(claim, out var storeId) && storeId > 0 ? storeId : null;
        }
    }

    public int Required => StoreId
        ?? throw new InvalidOperationException("The request has no store context.");
}
