public class SettingsService
{
    private readonly IStoreRepository _stores;

    public SettingsService(IStoreRepository stores)
    {
        _stores = stores;
    }

    public async Task<StoreSettingsDto> GetAsync()
        => ToDto(await _stores.GetCurrentAsync());

    public async Task<StoreSettingsDto> SaveAsync(StoreSettingsDto dto)
    {
        var saved = await _stores.UpdateCurrentAsync(new Store
        {
            Name = dto.StoreName,
            LowStockAlertLimit = dto.LowStockAlertLimit,
            SmsAlertTemplate = dto.SmsAlertTemplate,
            TimeZoneId = dto.TimeZoneId
        });

        return ToDto(saved);
    }

    private static StoreSettingsDto ToDto(Store s) => new(
        s.Name, s.LowStockAlertLimit, s.SmsAlertTemplate, s.TimeZoneId);
}
