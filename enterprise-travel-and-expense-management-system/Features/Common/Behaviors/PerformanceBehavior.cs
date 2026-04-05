using System.Diagnostics;
using MediatR;

namespace enterprise_travel_and_expense_management_system.Features.Common.Behaviors;

/// <summary>
/// Pipeline behavior that measures and logs the execution time of requests.
/// Logs a warning if execution time exceeds 500ms.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private const long WarningThresholdMs = 500;

    /// <summary>
    /// Initializes a new instance of the PerformanceBehavior class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the request and measures its execution time.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="next">The next handler in the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the next handler.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("[PERF] Starting execution of {RequestName}", requestName);
            var response = await next();
            stopwatch.Stop();

            var executionTimeMs = stopwatch.ElapsedMilliseconds;

            if (executionTimeMs > WarningThresholdMs)
            {
                _logger.LogWarning(
                    "[PERF] {RequestName} took {ExecutionTimeMs}ms (threshold: {ThresholdMs}ms)",
                    requestName,
                    executionTimeMs,
                    WarningThresholdMs);
            }
            else
            {
                _logger.LogInformation(
                    "[PERF] {RequestName} completed in {ExecutionTimeMs}ms",
                    requestName,
                    executionTimeMs);
            }

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            var executionTimeMs = stopwatch.ElapsedMilliseconds;
            _logger.LogError(
                ex,
                "[PERF] {RequestName} failed after {ExecutionTimeMs}ms with exception: {ExceptionMessage}",
                requestName,
                executionTimeMs,
                ex.Message);
            throw;
        }
    }
}
