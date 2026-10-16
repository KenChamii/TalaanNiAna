public record StoreSettingsDto(
    string StoreName,
    int LowStockAlertLimit,
    string SmsAlertTemplate,
    string TimeZoneId = "Asia/Manila");
