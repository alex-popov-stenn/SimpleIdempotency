using SimpleIdempotency.Services;

namespace SimpleIdempotency.Internal;

internal sealed class CleanupJob : BackgroundService
{
    private readonly int _maxCleanupSize;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CleanupJob> _logger;
    private readonly TimeSpan _cleanupInterval;

    public CleanupJob(
        IConfiguration configuration,
        IServiceProvider serviceProvider,
        ILogger<CleanupJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _maxCleanupSize = int.Parse(configuration["MaxCleanupSize"] ?? throw new ArgumentException("There is no such key in the configuration"));
        _cleanupInterval = TimeSpan.FromSeconds(int.Parse(configuration["CleanupIntervalInSeconds"] ?? throw new ArgumentException("There is no such key in the configuration")));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await BackgroundProcessing(stoppingToken);
    }

    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_cleanupInterval, stoppingToken);

                await using var scope = _serviceProvider.CreateAsyncScope();
                var cleaner = scope.ServiceProvider.GetRequiredService<IIdempotencyCacheCleaner>();
                await cleaner.CleanExpiredAsync(_maxCleanupSize, stoppingToken);
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while cleaning expired idempotency keys");
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        await base.StopAsync(stoppingToken);
    }
}