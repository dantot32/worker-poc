using Serilog;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Hosting;
using WorkerPoc;

var builder = Host.CreateApplicationBuilder(args);

// since will be run as windows service
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    builder.Services.Configure<HostOptions>(opt => opt.ShutdownTimeout = TimeSpan.FromSeconds(30));

// serilog config
var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WorkerPocService", "logs");
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(Path.Combine(logPath, "worker-poc-.log"), rollingInterval: RollingInterval.Day)
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

//#if RELEASE
//    builder.Logging.ClearProviders(); // otherwise console log wont be written
//#endif
builder.Logging.AddSerilog();

// register services to host
builder.Services.AddHostedService<Worker>();

// build and run host
var host = builder.Build();
host.Run();
