namespace TalaanNiAna.Infrastructure.Repositories;

/// <summary>
/// Builds a safe ILIKE pattern. Postgres compares case-sensitively, so searches
/// go through ILIKE rather than Contains, which means the wildcards a customer
/// might type have to be escaped first.
/// </summary>
internal static class SearchPattern
{
    public const string Escape = "\\";

    public static string Contains(string term)
    {
        var escaped = term
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");

        return $"%{escaped}%";
    }
}
