using Microsoft.EntityFrameworkCore;

public class StoreRepository : IStoreRepository
{
    private readonly AppDbContext _context;
    private readonly ICurrentStore _currentStore;

    public StoreRepository(AppDbContext context, ICurrentStore currentStore)
    {
        _context = context;
        _currentStore = currentStore;
    }

    public async Task<Store> GetCurrentAsync()
        => await _context.Stores.FindAsync(_currentStore.Required)
           ?? throw new InvalidOperationException($"Store {_currentStore.Required} no longer exists.");

    public async Task<Store?> GetByIdAsync(int id)
        => await _context.Stores.FindAsync(id);

    public async Task<Store> CreateAsync(Store store)
    {
        _context.Stores.Add(store);
        await _context.SaveChangesAsync();
        return store;
    }

    public async Task<Store> UpdateCurrentAsync(Store changes)
    {
        var store = await GetCurrentAsync();

        store.Name = changes.Name;
        store.TimeZoneId = changes.TimeZoneId;
        store.LowStockAlertLimit = changes.LowStockAlertLimit;
        store.SmsAlertTemplate = changes.SmsAlertTemplate;

        await _context.SaveChangesAsync();
        return store;
    }

    public async Task DeleteAsync(int id)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store is null) return;

        _context.Stores.Remove(store);
        await _context.SaveChangesAsync();
    }
}
