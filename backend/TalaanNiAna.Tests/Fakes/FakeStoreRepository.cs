namespace TalaanNiAna.Tests.Fakes;

public sealed class FakeStoreRepository : IStoreRepository
{
    public Store Current { get; set; } = new()
    {
        Id = 1,
        Name = "Tindahan ni Ana",
        TimeZoneId = "Asia/Manila"
    };

    public Task<Store> GetCurrentAsync() => Task.FromResult(Current);

    // Not exercised by these tests.
    public Task<Store?> GetByIdAsync(int id) => throw new NotSupportedException();
    public Task<Store> CreateAsync(Store store) => throw new NotSupportedException();
    public Task<Store> UpdateCurrentAsync(Store changes) => throw new NotSupportedException();
    public Task DeleteAsync(int id) => throw new NotSupportedException();
}
