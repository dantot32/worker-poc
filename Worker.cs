namespace WorkerPoc;
public class Worker : WorkerBase
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger) : base(logger)
        => _logger = logger;

    protected override async Task RunAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }

}
