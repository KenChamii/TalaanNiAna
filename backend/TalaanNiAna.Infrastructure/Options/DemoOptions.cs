namespace TalaanNiAna.Infrastructure.Options;

/// <summary>
/// The public demo. Off everywhere by default, because a store that anyone can
/// sign into has no business existing in a deployment holding real data.
/// </summary>
public class DemoOptions
{
    public const string SectionName = "Demo";

    /// <summary>Seeds the demo store on startup, and opens the demo sign-in endpoint.</summary>
    public bool Enabled { get; set; }

    public string Username { get; set; } = "demo";

    /// <summary>
    /// Never sent to the browser. The demo endpoint signs the user in on the
    /// server, so the bundle carries no credential at all.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    public string StoreName { get; set; } = "Tindahan ni Aling Nena";
}
