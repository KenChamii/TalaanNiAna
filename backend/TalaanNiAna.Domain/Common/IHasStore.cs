/// <summary>
/// Marks a row as belonging to one store. The DbContext filters every query
/// on it and stamps it on insert, so isolation cannot be forgotten.
/// </summary>
public interface IHasStore
{
    int StoreId { get; set; }
}
