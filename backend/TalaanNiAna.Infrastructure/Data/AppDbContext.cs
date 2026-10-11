using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<TransactionItem> TransactionItems => Set<TransactionItem>();
    public DbSet<StoreSettings> StoreSettings => Set<StoreSettings>();
    public DbSet<CreditPayment> CreditPayments => Set<CreditPayment>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Product>(entity =>
        {
            entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            entity.Property(p => p.Name).HasMaxLength(120).IsRequired();
            entity.Property(p => p.Category).HasMaxLength(60).IsRequired();

            // Concurrency token. Becomes xmin on PostgreSQL.
            entity.Property(p => p.RowVersion).IsRowVersion();

            entity.HasIndex(p => p.Category);
        });

        builder.Entity<Transaction>(entity =>
        {
            entity.Property(t => t.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(t => t.IdempotencyKey).HasMaxLength(64);

            // Filtered unique index: only one transaction per key, nulls unconstrained.
            entity.HasIndex(t => t.IdempotencyKey)
                .IsUnique()
                .HasFilter("[IdempotencyKey] IS NOT NULL");

            entity.HasIndex(t => t.TransactionDate);

            entity.HasMany(t => t.Items)
                .WithOne(i => i.Transaction)
                .HasForeignKey(i => i.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(t => t.Customer)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<TransactionItem>()
            .Property(i => i.UnitPrice)
            .HasColumnType("decimal(18,2)");

        builder.Entity<Customer>(entity =>
        {
            entity.Property(c => c.FullName).HasMaxLength(120).IsRequired();
            entity.Property(c => c.ContactNumber).HasMaxLength(32);
            entity.Property(c => c.TotalCredit).HasColumnType("decimal(18,2)");
        });

        builder.Entity<CreditPayment>(entity =>
        {
            entity.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            entity.Property(p => p.Note).HasMaxLength(250);

            entity.HasOne(p => p.Customer)
                .WithMany(c => c.CreditPayments)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<RefreshToken>(entity =>
        {
            entity.Property(t => t.TokenHash).HasMaxLength(64).IsRequired();
            entity.Property(t => t.ReplacedByHash).HasMaxLength(64);

            entity.HasIndex(t => t.TokenHash).IsUnique();

            entity.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<StoreSettings>().HasData(new StoreSettings
        {
            Id = 1,
            StoreName = "Nena's Sari-Sari Store",
            LowStockAlertLimit = 10,
            SmsAlertTemplate = "Hi Suki, paalala lang..."
        });
    }
}
