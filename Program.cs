using Serilog;
using WorkerPoc;

var builder = Host.CreateApplicationBuilder(args);

// serilog config
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/worker-poc-.log", rollingInterval: RollingInterval.Day)
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

#if RELEASE
    builder.Logging.ClearProviders(); // otherwise console log wont be written
#endif
builder.Logging.AddSerilog();

// register services to host
builder.Services.AddHostedService<Worker>();

// build and run host
var host = builder.Build();
host.Run();
