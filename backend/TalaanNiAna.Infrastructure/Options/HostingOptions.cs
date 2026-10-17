namespace TalaanNiAna.Infrastructure.Options;

/// <summary>How the API is being hosted. Set from the environment at deploy time.</summary>
public class HostingOptions
{
    public const string SectionName = "Hosting";

    /// <summary>
    /// True when something else terminates TLS in front of the API, as Render
    /// does. The app then trusts the forwarded headers and stops issuing
    /// HTTPS redirects of its own, which would otherwise loop.
    /// </summary>
    public bool BehindReverseProxy { get; set; }

    /// <summary>
    /// Applies pending migrations at startup. The free hosting tier offers no
    /// shell to run dotnet-ef from, so the deployed app has to do it itself.
    /// Off by default: locally the migration is run deliberately.
    /// </summary>
    public bool RunMigrationsOnStartup { get; set; }
}
