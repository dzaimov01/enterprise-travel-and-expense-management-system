using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace enterprise_travel_and_expense_management_system.Features.Common.Behaviors;

/// <summary>
/// Pipeline behavior that caches query results if the request implements ICacheable.
/// This is a "Senior" feature demonstrating performance optimization.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the CachingBehavior class.
    /// </summary>
    /// <param name="cache">The memory cache instance.</param>
    /// <param name="logger">The logger instance.</param>
    public CachingBehavior(IMemoryCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Handles the request and checks cache before execution.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="next">The next handler in the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The cached response if available; otherwise, the response from the next handler.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Only cache if the request implements ICacheable
        if (request is not ICacheable cacheableRequest)
        {
            return await next();
        }

        var cacheKey = cacheableRequest.CacheKey;
        var requestName = typeof(TRequest).Name;

        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out TResponse? cachedResult))
        {
            _logger.LogInformation(
                "[CACHE] Cache hit for {RequestName} with key: {CacheKey}",
                requestName,
                cacheKey);
            return cachedResult!;
        }

        _logger.LogInformation(
            "[CACHE] Cache miss for {RequestName} with key: {CacheKey}. Executing handler...",
            requestName,
            cacheKey);

        // Execute the handler
        var response = await next();

        // Store in cache
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheableRequest.DurationSeconds));

        _cache.Set(cacheKey, response, cacheOptions);

        _logger.LogInformation(
            "[CACHE] Cached {RequestName} with key: {CacheKey} for {DurationSeconds}s",
            requestName,
            cacheKey,
            cacheableRequest.DurationSeconds);

        return response;
    }
}
