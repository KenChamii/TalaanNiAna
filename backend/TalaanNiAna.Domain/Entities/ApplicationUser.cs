using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>The store this account belongs to. Set at registration.</summary>
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;
}
