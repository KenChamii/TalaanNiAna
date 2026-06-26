using Microsoft.EntityFrameworkCore.Storage;

public class UnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    public IProductRepository Products { get; }
    public ICustomerRepository Customers { get; }
    public ITransactionRepository Transactions { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        // Reuses the same AppDbContext instance the repositories already take in their
        // constructors (see 4.5–4.7), so reads/writes here share one tracked change-set
        // and one transaction — this is what makes the commit/rollback atomic.
        Products = new ProductRepository(context);
        Customers = new CustomerRepository(context);
        Transactions = new TransactionRepository(context);
    }

    public async Task BeginTransactionAsync()
        => _transaction = await _context.Database.BeginTransactionAsync();

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
        if (_transaction is not null)
            await _transaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        if (_transaction is not null)
            await _transaction.RollbackAsync();
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
            await _transaction.DisposeAsync();
    }
}