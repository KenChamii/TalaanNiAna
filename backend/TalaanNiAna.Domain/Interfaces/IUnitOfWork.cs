public interface IUnitOfWork
{
    IProductRepository Products { get; }
    ICustomerRepository Customers { get; }
    ITransactionRepository Transactions { get; }

    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    Task SaveChangesAsync();
}