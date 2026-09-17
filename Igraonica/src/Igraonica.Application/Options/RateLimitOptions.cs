namespace Igraonica.Application.Options;

public class RateLimitOptions
{
    public const string SectionName = "RateLimit";

    public int LoginAttemptsPerMinute { get; set; } = 5;
}
