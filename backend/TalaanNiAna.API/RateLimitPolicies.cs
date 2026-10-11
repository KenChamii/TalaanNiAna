/// <summary>Named rate-limit policies, so controllers and startup cannot drift apart.</summary>
public static class RateLimitPolicies
{
    /// <summary>Login, register and refresh. Tight, because these are brute-force targets.</summary>
    public const string Auth = "auth";
}
