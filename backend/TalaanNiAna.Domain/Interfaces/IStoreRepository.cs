public interface IStoreRepository
{
    /// <summary>The store on the current request's token.</summary>
    Task<Store> GetCurrentAsync();

    /// <summary>Unfiltered lookup. Used during login, before a store context exists.</summary>
    Task<Store?> GetByIdAsync(int id);

    Task<Store> CreateAsync(Store store);

    /// <summary>Applies editable settings to the current store.</summary>
    Task<Store> UpdateCurrentAsync(Store changes);

    /// <summary>Removes a store. Used to undo a failed registration.</summary>
    Task DeleteAsync(int id);
}
