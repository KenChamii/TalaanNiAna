public class StoreSettingsRepository : IStoreSettingsRepository
{
    private readonly AppDbContext _context;

    public StoreSettingsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StoreSettings> GetAsync()
        => await _context.StoreSettings.FindAsync(1)
           ?? throw new InvalidOperationException("Store settings row is missing — check the seed data in AppDbContext.");

    public async Task<StoreSettings> SaveAsync(StoreSettings settings)
    {
        var existing = await GetAsync();
        existing.StoreName = settings.StoreName;
        existing.LowStockAlertLimit = settings.LowStockAlertLimit;
        existing.SmsAlertTemplate = settings.SmsAlertTemplate;

        await _context.SaveChangesAsync();
        return existing;
    }
}