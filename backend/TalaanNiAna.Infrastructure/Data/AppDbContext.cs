using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly ICurrentStore _currentStore;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentStore currentStore)
        : base(options)
    {
        _currentStore = currentStore;
    }

    /// <summary>
    /// Read by every tenant query filter. Zero on anonymous requests, which
    /// matches no rows rather than leaking another store's data.
    /// </summary>
    public int CurrentStoreId => _currentStore.StoreId ?? 0;

    public DbSet<Store> Stores => Set<Store>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<TransactionItem> TransactionItems => Set<TransactionItem>();
    public DbSet<CreditPayment> CreditPayments => Set<CreditPayment>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public override int SaveChanges()
    {
        StampStoreId();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampStoreId();
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Store>(entity =>
        {
            entity.Property(s => s.Name).HasMaxLength(120).IsRequired();
            entity.Property(s => s.TimeZoneId).HasMaxLength(64).IsRequired();
            entity.Property(s => s.SmsAlertTemplate).HasMaxLength(500);
        });

        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Store)
            .WithMany()
            .HasForeignKey(u => u.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Product>(entity =>
        {
            entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            entity.Property(p => p.CostPrice).HasColumnType("decimal(18,2)");
            entity.Property(p => p.Name).HasMaxLength(120).IsRequired();
            entity.Property(p => p.Category).HasMaxLength(60).IsRequired();
            entity.Property(p => p.Barcode).HasMaxLength(32);
            entity.Property(p => p.RowVersion).IsRowVersion();

            entity.HasIndex(p => new { p.StoreId, p.Category });

            // One active product per code per store. Deleted products keep their
            // barcode but stop reserving it, so the code can be reused.
            entity.HasIndex(p => new { p.StoreId, p.Barcode })
                .IsUnique()
                .HasFilter("[Barcode] IS NOT NULL AND [IsActive] = 1");
            entity.HasQueryFilter(p => p.StoreId == CurrentStoreId);
        });

        builder.Entity<Customer>(entity =>
        {
            entity.Property(c => c.FullName).HasMaxLength(120).IsRequired();
            entity.Property(c => c.ContactNumber).HasMaxLength(32);
            entity.Property(c => c.TotalCredit).HasColumnType("decimal(18,2)");

            entity.HasIndex(c => c.StoreId);
            entity.HasQueryFilter(c => c.StoreId == CurrentStoreId);
        });

        builder.Entity<Transaction>(entity =>
        {
            entity.Property(t => t.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(t => t.IdempotencyKey).HasMaxLength(64);

            // Unique per store: two stores may legitimately generate the same key.
            entity.HasIndex(t => new { t.StoreId, t.IdempotencyKey })
                .IsUnique()
                .HasFilter("[IdempotencyKey] IS NOT NULL");

            entity.HasIndex(t => new { t.StoreId, t.TransactionDate });

            entity.HasMany(t => t.Items)
                .WithOne(i => i.Transaction)
                .HasForeignKey(i => i.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(t => t.Customer)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasQueryFilter(t => t.StoreId == CurrentStoreId);
        });

        builder.Entity<TransactionItem>(entity =>
        {
            entity.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(i => i.UnitCost).HasColumnType("decimal(18,2)");
            entity.HasIndex(i => i.StoreId);
            entity.HasQueryFilter(i => i.StoreId == CurrentStoreId);
        });

        builder.Entity<CreditPayment>(entity =>
        {
            entity.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            entity.Property(p => p.Note).HasMaxLength(250);

            entity.HasOne(p => p.Customer)
                .WithMany(c => c.CreditPayments)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(p => new { p.StoreId, p.PaidAt });
            entity.HasQueryFilter(p => p.StoreId == CurrentStoreId);
        });

        builder.Entity<RefreshToken>(entity =>
        {
            entity.Property(t => t.TokenHash).HasMaxLength(64).IsRequired();
            entity.Property(t => t.ReplacedByHash).HasMaxLength(64);

            entity.HasIndex(t => t.TokenHash).IsUnique();

            // Not store-scoped: refresh happens before a store context exists.
            entity.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    /// <summary>Sets StoreId on new rows so no repository has to remember to.</summary>
    private void StampStoreId()
    {
        if (_currentStore.StoreId is not { } storeId) return;

        foreach (var entry in ChangeTracker.Entries<IHasStore>())
        {
            if (entry.State == EntityState.Added && entry.Entity.StoreId == 0)
                entry.Entity.StoreId = storeId;
        }
    }
}
