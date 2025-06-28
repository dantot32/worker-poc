using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using System.Runtime.InteropServices;
using WorkerPoc;

var builder = Host.CreateApplicationBuilder(args);

// since will be run as windows service
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    builder.Services.Configure<HostOptions>(opt => opt.ShutdownTimeout = TimeSpan.FromSeconds(30));
    builder.Services.AddWindowsService(opt => opt.ServiceName = "WorkerPocService");

// serilog config
var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WorkerPocService", "logs");

var logConfig = new LoggerConfiguration()
            .WriteTo.File(Path.Combine(logPath, "worker-poc-.log"), rollingInterval: RollingInterval.Day)
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext();

#if RELEASE
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    logConfig.WriteTo.EventLog(source: "WorkerPocService Source",
                                logName: "Application",
                                manageEventSource: true,
                                restrictedToMinimumLevel: LogEventLevel.Information);
#endif

Log.Logger = logConfig.CreateLogger();

#if RELEASE
    // this is needed since we wonna get rid of default logging providers such as EventLog configured in 'AddWindowsService'
    builder.Logging.ClearProviders(); // otherwise console log wont be written
#endif
builder.Logging.AddSerilog();

// register services to host
builder.Services.AddHostedService<Worker>();

// build and run host
var host = builder.Build();
host.Run();
