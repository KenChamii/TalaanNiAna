/// <summary>The store the current request belongs to, read from the access token.</summary>
public interface ICurrentStore
{
    /// <summary>Null on anonymous requests such as login, register and refresh.</summary>
    int? StoreId { get; }

    /// <summary>The store id, or throws. Use where a store is required.</summary>
    int Required { get; }
}
