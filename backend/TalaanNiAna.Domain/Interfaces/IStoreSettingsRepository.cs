public interface IStoreSettingsRepository
{
    Task<StoreSettings> GetAsync();   // there's only ever one row (Id = 1, seeded in 3.3)
    Task<StoreSettings> SaveAsync(StoreSettings settings);
}