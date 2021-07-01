using System;
using CommandLine;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Elastic.CommonSchema.Serilog;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using Serilog.Formatting.Json;

namespace demo_web
{
    
    public enum LoggerMode
    {
        Text,
        Json,
        ECS
    }

    public class CommandLineOptions
    {
        [Option(Default = LoggerMode.Text, HelpText = "Configures the log output (Text, Json, ECS)", Required = true)]
        public LoggerMode LoggerMode { get; set; }
    }
    
    public class Program
    {
        public static int Main(string[] args)
        {
            var results = Parser.Default.ParseArguments<CommandLineOptions>(args);
            return results.MapResult(

                options => Run(options, args),
                _ => 1
            );
        }

        private static int Run(CommandLineOptions options, string[] args)
        {
            // The initial "bootstrap" logger is able to log errors during start-up. It's completely replaced by the
            // logger configured in `UseSerilog()` below, once configuration and dependency-injection have both been
            // set up successfully.
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(options.LoggerMode, args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        
        public static IHostBuilder CreateHostBuilder(LoggerMode mode, string[] args)
        {
            ITextFormatter formatter = mode switch
            {
                LoggerMode.Text => new MessageTemplateTextFormatter("{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"),
                LoggerMode.Json => new JsonFormatter(renderMessage: true),
                LoggerMode.ECS => new EcsTextFormatter(),
                _ => throw new NotSupportedException()
            };

            return Host.CreateDefaultBuilder(args)
                .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(formatter: formatter))
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}
