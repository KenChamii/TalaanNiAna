namespace TalaanNiAna.Tests.Fakes;

public sealed class FakeUnitOfWork : IUnitOfWork
{
    public FakeProductRepository ProductsFake { get; } = new();
    public FakeCustomerRepository CustomersFake { get; } = new();
    public FakeTransactionRepository TransactionsFake { get; } = new();

    public IProductRepository Products => ProductsFake;
    public ICustomerRepository Customers => CustomersFake;
    public ITransactionRepository Transactions => TransactionsFake;

    public bool Committed { get; private set; }
    public bool RolledBack { get; private set; }

    public Task BeginTransactionAsync() => Task.CompletedTask;

    public Task CommitAsync()
    {
        Committed = true;
        return Task.CompletedTask;
    }

    public Task RollbackAsync()
    {
        RolledBack = true;
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => Task.CompletedTask;
}
