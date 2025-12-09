using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaynetReportCsvToTxt;
using Serilog;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        try
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("Config/consolesettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile($"Config/consolesettings.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                                       optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddScoped<CsvToTxtService>();
                    services.Configure<ConfigSettings>(context.Configuration.GetSection("Config"));
                })
                .UseSerilog((context, services, configuration) =>
                {
                    configuration
                        .ReadFrom.Configuration(context.Configuration)
                        .WriteTo.Console()
                        .WriteTo.EventLog("PaynetConsole", manageEventSource: false);
                });

            using var host = builder.Build();

            using (var scope = host.Services.CreateScope())
            {
                var csvToTxtService = scope.ServiceProvider.GetRequiredService<CsvToTxtService>();

                try
                {
                    Log.Information("Converting CSV start");

                    // Call your service methods
                    string sol = await csvToTxtService.SaveRppSqlIntoTxt();
                    string myd = await csvToTxtService.SaveNydCsvIntoTxt();
                    string rpp = await csvToTxtService.SaveRppI01AsTxt();

                    Log.Information("Txt generated successfully.");

                    return 0; // SUCCESS
                }
                catch (Exception ex)
                {
                    Log.Error($"An error occurred: {ex.Message}");
                    return 1; // FAIL
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Startup error: {ex.Message}");
            return 1; // FAIL (startup issue)
        }
    }
}