using Microsoft.Extensions.Configuration;
using Serilog;

class Program
{
    static int Main(string[] args)
    {
        IConfiguration config;
        try
        {
            config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config/consoleSettings.json", optional: false, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to load appsettings.json: {ex.Message}");
            return 1;
        }

        var eDocsService = new eDocsService();
        Log.Information("Checking DRR for CSV/TXT files...");

        bool hasFiles = eDocsService.CheckForCsvOrTxtFiles(config);

        if (hasFiles)
        {
            Log.Information("Files found! Returning 1");
            return 1;
        }
        else
        {
            Log.Information("No files found. Returning 0");
            return 0;
        }
    }
}