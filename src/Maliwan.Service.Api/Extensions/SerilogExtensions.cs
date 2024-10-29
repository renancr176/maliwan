using System.Reflection;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace Maliwan.Service.Api.Extensions;

public static class SerilogExtensions
{
    public static IHostBuilder ConfigureSerilog(this IHostBuilder webHostBuilder)
    {
        try
        {
            return webHostBuilder.UseSerilog((context, loggerConfiguration) =>
            {
                var logLevel = context.Configuration.GetValue<LogEventLevel>("Serilog:MinimumLevel:Default");
                loggerConfiguration
                    .MinimumLevel.Verbose()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .Enrich.WithProperty("version",
                        Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version)
                    .Enrich.WithMachineName()
                    .Enrich.WithMemoryUsage()
                    .ReadFrom.Configuration(context.Configuration);

                if (context.Configuration.GetValue<bool>("Serilog:Console"))
                {
                    loggerConfiguration.WriteTo.Console(logLevel,
                        "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}ElapsedMilliseconds:{ElapsedMilliseconds}, StatusCode:{StatusCode}, RequestPath:{RequestPath}, Machine:{MachineName}, RequestId:{RequestId}, MemoryUsage:{MemoryUsage} bytes{NewLine}{Exception}{NewLine}");
                }
                if (context.Configuration.GetValue<bool>("Serilog:File"))
                {
                    loggerConfiguration.WriteTo.File(Path.Combine("Log", $"LogFile_{DateTime.Now.ToShortDateString().Replace("/", "")}.txt"), logLevel,
                        "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}ElapsedMilliseconds:{ElapsedMilliseconds}, StatusCode:{StatusCode}, RequestPath:{RequestPath}, Machine:{MachineName}, RequestId:{RequestId}, MemoryUsage:{MemoryUsage} bytes{NewLine}{Exception}{NewLine}");
                }
            });
        }
        catch (Exception ex)
        {
            File.WriteAllText("log.txt", ex.ToString());
        }
        return webHostBuilder;

    }

}