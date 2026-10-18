using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TalaanNiAna.Infrastructure.Options;

namespace TalaanNiAna.Infrastructure.Data;

/// <summary>
/// Fills the demo store with a fortnight of plausible trading, so a visitor sees
/// a working shop rather than empty screens.
///
/// Runs with no HTTP request behind it, so there is no tenant context: every row
/// sets StoreId itself, and every read ignores the query filters. Both would
/// otherwise silently match nothing.
/// </summary>
public class DemoSeeder
{
    private const int DaysOfHistory = 14;

    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _users;
    private readonly DemoOptions _options;
    private readonly ILogger<DemoSeeder> _logger;

    // Fixed seed: the demo looks the same on every fresh database, which makes
    // a screenshot worth keeping and a bug report worth reproducing.
    private readonly Random _random = new(20261018);

    /// <summary>
    /// Stock on hand as the seeded history plays forward. The ledger has to add
    /// up, so every movement reads and writes this rather than the entity, and
    /// the product's final quantity is whatever is left at the end.
    /// </summary>
    private readonly Dictionary<int, int> _balances = [];

    public DemoSeeder(
        AppDbContext context,
        UserManager<ApplicationUser> users,
        DemoOptions options,
        ILogger<DemoSeeder> logger)
    {
        _context = context;
        _users = users;
        _options = options;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _context.Stores.IgnoreQueryFilters().AnyAsync(s => s.IsDemo))
        {
            _logger.LogInformation("Demo store already present, nothing to seed.");
            return;
        }

        if (string.IsNullOrWhiteSpace(_options.Password))
        {
            _logger.LogWarning("Demo is enabled but Demo:Password is empty. Skipping the seed.");
            return;
        }

        var store = new Store { Name = _options.StoreName, IsDemo = true };
        _context.Stores.Add(store);
        await _context.SaveChangesAsync();

        var user = new ApplicationUser
        {
            UserName = _options.Username,
            Email = $"{_options.Username}@example.com",
            DisplayName = "Aling Nena",
            StoreId = store.Id
        };

        var created = await _users.CreateAsync(user, _options.Password);

        if (!created.Succeeded)
        {
            // Leaving a store with no owner would make the seeder think it had
            // already run, and the demo would never work.
            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();

            _logger.LogError("Could not create the demo user: {Errors}",
                string.Join("; ", created.Errors.Select(e => e.Description)));
            return;
        }

        var products = AddProducts(store.Id);
        var customers = AddCustomers(store.Id);
        await _context.SaveChangesAsync();

        AddOpeningCounts(store.Id, products);
        AddTrading(store.Id, products, customers);
        AddSpoilage(store.Id, products);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded the demo store with {Products} products and {Days} days of sales.",
            products.Count, DaysOfHistory);
    }

    /// <summary>
    /// Real sari-sari stock: cheap fast movers, a few bigger-ticket items, one
    /// loss leader, and two things with no cost recorded yet.
    /// </summary>
    private List<Product> AddProducts(int storeId)
    {
        var products = new List<Product>
        {
            Product(storeId, "Lucky Me Pancit Canton", "Noodles", 12m, 9.50m, 48, "4807770270066"),
            Product(storeId, "Coke Mismo 290ml", "Inumin", 30m, 24m, 36, "4800016641107"),
            Product(storeId, "Royal Mismo 290ml", "Inumin", 30m, 24m, 24, "4800016641206"),
            Product(storeId, "Kopiko Brown 3-in-1", "Kape", 8m, 6.20m, 120, "4800361410014"),
            Product(storeId, "Bear Brand Sterilized 300ml", "Gatas", 42m, 36m, 18, "4800361361019"),
            Product(storeId, "Palmolive Shampoo Sachet", "Panlinis", 7m, 5.40m, 90, "4800888134028"),
            Product(storeId, "Tide Bar 130g", "Panlinis", 23m, 19m, 30, "4902430538268"),
            Product(storeId, "Silver Swan Soy Sauce 200ml", "Pampalasa", 18m, 14m, 22, "4800194110129"),
            Product(storeId, "Datu Puti Suka 350ml", "Pampalasa", 21m, 17m, 16, "4800194120128"),
            Product(storeId, "Argentina Corned Beef 150g", "De lata", 48m, 41m, 14, "4806502540016"),
            Product(storeId, "555 Sardinas 155g", "De lata", 25m, 20.50m, 28, "4800016011047"),
            Product(storeId, "Skyflakes Crackers", "Meryenda", 9m, 7m, 60, "4800016441011"),
            Product(storeId, "Piattos Cheese 40g", "Meryenda", 20m, 16.50m, 34, "4800016161032"),
            Product(storeId, "Bato ng Yelo", "Inumin", 10m, 0m, 40, null),
            Product(storeId, "Asin 1kg", "Pampalasa", 18m, 20m, 12, "ASIN-1KG"),
            Product(storeId, "Itlog (bawat isa)", "Sariwa", 9m, 0m, 90, null),
            Product(storeId, "Load Card 50", "Serbisyo", 55m, 48m, 25, null),
            Product(storeId, "Tubig Mineral 500ml", "Inumin", 15m, 11m, 8, "4800016881015")
        };

        _context.Products.AddRange(products);
        return products;
    }

    private static Product Product(
        int storeId, string name, string category,
        decimal price, decimal cost, int stock, string? barcode) => new()
        {
            StoreId = storeId,
            Name = name,
            Category = category,
            Price = price,
            CostPrice = cost,
            Barcode = barcode,
            StockQuantity = stock,
            LowStockThreshold = 10,
            IsActive = true
        };

    private List<Customer> AddCustomers(int storeId)
    {
        var customers = new List<Customer>
        {
            new() { StoreId = storeId, FullName = "Mang Tonyo", ContactNumber = "0917 555 0101" },
            new() { StoreId = storeId, FullName = "Aling Baby", ContactNumber = "0918 555 0144" },
            new() { StoreId = storeId, FullName = "Kuya Jun", ContactNumber = "0920 555 0177" },
            new() { StoreId = storeId, FullName = "Ate Marites", ContactNumber = "0995 555 0133" },
            new() { StoreId = storeId, FullName = "Lola Pacing", ContactNumber = null }
        };

        _context.Customers.AddRange(customers);
        return customers;
    }

    /// <summary>
    /// Two weeks of sales, busier at the start and end of the day and heavier on
    /// paydays, so the dashboard chart has a shape rather than a flat line.
    /// </summary>
    private void AddTrading(int storeId, List<Product> products, List<Customer> customers)
    {
        var manila = TimeSpan.FromHours(8);
        var todayLocal = DateTime.UtcNow.Add(manila).Date;

        foreach (var dayOffset in Enumerable.Range(0, DaysOfHistory).Reverse())
        {
            var localDay = todayLocal.AddDays(-dayOffset);
            var payday = localDay.Day is 15 or 30;
            var sales = _random.Next(payday ? 14 : 6, payday ? 22 : 13);

            for (var i = 0; i < sales; i++)
            {
                // Trading hours, 6am to 9pm local, converted back to UTC for storage.
                var localTime = localDay.AddHours(6 + _random.NextDouble() * 15);
                var whenUtc = DateTime.SpecifyKind(localTime - manila, DateTimeKind.Utc);

                // Roughly one sale in five goes on the utang list.
                var onCredit = _random.Next(5) == 0;
                var customer = onCredit ? customers[_random.Next(customers.Count)] : null;

                var transaction = new Transaction
                {
                    StoreId = storeId,
                    TransactionDate = whenUtc,
                    PaymentType = onCredit ? PaymentType.Credit : PaymentType.Cash,
                    Customer = customer,
                    IdempotencyKey = $"demo-{localDay:yyyyMMdd}-{i}"
                };

                var lineCount = _random.Next(1, 5);
                var chosen = products.OrderBy(_ => _random.Next()).Take(lineCount);

                foreach (var product in chosen)
                {
                    var quantity = _random.Next(1, 4);

                    transaction.Items.Add(new TransactionItem
                    {
                        StoreId = storeId,
                        Product = product,
                        Quantity = quantity,
                        UnitPrice = product.Price,
                        UnitCost = product.CostPrice
                    });
                }

                transaction.TotalAmount = transaction.Items.Sum(line => line.Quantity * line.UnitPrice);

                // The ledger explains every sale, the same as a real checkout would.
                foreach (var line in transaction.Items)
                {
                    _balances[line.Product.Id] -= line.Quantity;

                    _context.StockMovements.Add(new StockMovement
                    {
                        StoreId = storeId,
                        Product = line.Product,
                        Quantity = -line.Quantity,
                        Reason = StockReason.Sale,
                        BalanceAfter = _balances[line.Product.Id],
                        Transaction = transaction,
                        OccurredAt = whenUtc
                    });
                }

                if (customer is not null)
                {
                    customer.TotalCredit += transaction.TotalAmount;
                    customer.LastActiveAt = whenUtc;
                }

                _context.Transactions.Add(transaction);
            }

            AddPayments(storeId, customers, localDay, manila);
        }
    }

    /// <summary>
    /// What was on the shelf on day one. Without these the history screen opens
    /// on a stock figure that nothing in the ledger accounts for.
    /// </summary>
    private void AddOpeningCounts(int storeId, List<Product> products)
    {
        var openedAt = DateTime.UtcNow.AddDays(-DaysOfHistory).Date;

        foreach (var product in products)
        {
            // Opened with enough to trade through the fortnight and still have
            // the shelf stock the product is declared with.
            var opening = product.StockQuantity * 3;
            _balances[product.Id] = opening;

            _context.StockMovements.Add(new StockMovement
            {
                StoreId = storeId,
                Product = product,
                Quantity = opening,
                Reason = StockReason.Opening,
                BalanceAfter = opening,
                OccurredAt = DateTime.SpecifyKind(openedAt, DateTimeKind.Utc)
            });
        }
    }

    /// <summary>
    /// Eggs break and bread goes stale. A shrinkage report with nothing in it
    /// would not show what the screen is for.
    /// </summary>
    private void AddSpoilage(int storeId, List<Product> products)
    {
        var spoilable = products
            .Where(p => p.Category is "Sariwa" or "Meryenda" or "Gatas")
            .ToList();

        foreach (var product in spoilable)
        {
            var lost = _random.Next(1, 5);
            var daysAgo = _random.Next(1, DaysOfHistory);

            _balances[product.Id] -= lost;

            _context.StockMovements.Add(new StockMovement
            {
                StoreId = storeId,
                Product = product,
                Quantity = -lost,
                Reason = StockReason.Spoilage,
                BalanceAfter = _balances[product.Id],
                Note = product.Category == "Sariwa" ? "Nabasag" : "Expired na",
                OccurredAt = DateTime.UtcNow.AddDays(-daysAgo)
            });
        }

        // The shelf now holds what the ledger says it holds, not what the
        // product was declared with.
        foreach (var product in products)
            product.StockQuantity = Math.Max(0, _balances[product.Id]);
    }

    /// <summary>Someone settles part of their utang most days, so collections are not zero.</summary>
    private void AddPayments(int storeId, List<Customer> customers, DateTime localDay, TimeSpan offset)
    {
        var payers = customers.Where(c => c.TotalCredit > 50m).ToList();
        if (payers.Count == 0 || _random.Next(3) == 0) return;

        var customer = payers[_random.Next(payers.Count)];

        // Part payments, which is how utang is actually settled.
        var amount = Math.Round(customer.TotalCredit * (decimal)(0.3 + _random.NextDouble() * 0.5), 2);
        var whenUtc = DateTime.SpecifyKind(localDay.AddHours(17) - offset, DateTimeKind.Utc);

        _context.CreditPayments.Add(new CreditPayment
        {
            StoreId = storeId,
            Customer = customer,
            Amount = amount,
            PaidAt = whenUtc,
            Note = "Bayad sa utang"
        });

        customer.TotalCredit -= amount;
    }
}
