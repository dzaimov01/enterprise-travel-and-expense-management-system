namespace enterprise_travel_and_expense_management_system.Features.Common.Behaviors;

/// <summary>
/// Interface for requests that should be cached.
/// Implement this on Query classes to enable caching.
/// </summary>
public interface ICacheable
{
    /// <summary>
    /// Unique cache key for this request.
    /// Used to store and retrieve cached results.
    /// </summary>
    string CacheKey { get; }

    /// <summary>
    /// Duration in seconds for which the cache should remain valid.
    /// </summary>
    int DurationSeconds { get; }
}
