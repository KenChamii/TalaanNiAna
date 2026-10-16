public class SettingsService
{
    private readonly IStoreSettingsRepository _repo;

    public SettingsService(IStoreSettingsRepository repo)
    {
        _repo = repo;
    }

    public async Task<StoreSettingsDto> GetAsync()
        => ToDto(await _repo.GetAsync());

    public async Task<StoreSettingsDto> SaveAsync(StoreSettingsDto dto)
    {
        var saved = await _repo.SaveAsync(new StoreSettings
        {
            StoreName = dto.StoreName,
            LowStockAlertLimit = dto.LowStockAlertLimit,
            SmsAlertTemplate = dto.SmsAlertTemplate,
            TimeZoneId = dto.TimeZoneId
        });

        return ToDto(saved);
    }

    private static StoreSettingsDto ToDto(StoreSettings s) => new(
        s.StoreName, s.LowStockAlertLimit, s.SmsAlertTemplate, s.TimeZoneId);
}
