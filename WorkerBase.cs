namespace WorkerPoc;

public abstract class WorkerBase : BackgroundService
{
    private readonly ILogger _logger;

    protected WorkerBase(ILogger logger)
        => _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{Service} started", GetType().Name);
        try
        {
            await RunAsync(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("{Service} cancelled", GetType().Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Service} failed", GetType().Name);
            throw; // Let the host decide if it should restart
        }
        finally
        {
            _logger.LogInformation("{Service} stopped", GetType().Name);
        }
    }

    protected abstract Task RunAsync(CancellationToken stoppingToken);

}