public class SettingsService
{
    private readonly IStoreSettingsRepository _repo;

    public SettingsService(IStoreSettingsRepository repo)
    {
        _repo = repo;
    }

    public async Task<StoreSettingsDto> GetAsync()
    {
        var settings = await _repo.GetAsync();
        return new StoreSettingsDto(settings.StoreName, settings.LowStockAlertLimit, settings.SmsAlertTemplate);
    }

    public async Task<StoreSettingsDto> SaveAsync(StoreSettingsDto dto)
    {
        var saved = await _repo.SaveAsync(new StoreSettings
        {
            StoreName = dto.StoreName,
            LowStockAlertLimit = dto.LowStockAlertLimit,
            SmsAlertTemplate = dto.SmsAlertTemplate
        });

        return new StoreSettingsDto(saved.StoreName, saved.LowStockAlertLimit, saved.SmsAlertTemplate);
    }
}