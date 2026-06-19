public class StoreSettings
{
    public int Id { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public int LowStockAlertLimit { get; set; } = 10;
    public string SmsAlertTemplate { get; set; } = "Hi Suki, paalala lang...";
}