using Microsoft.Extensions.Configuration;
using SftpFileChecker.Services;

class Program
{
    static int Main(string[] args)
    {
        // Load settings
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var sftpService = new SftpService(config);

        Console.WriteLine("Checking DRR server for CSV/TXT files...");

        bool hasFiles = sftpService.CheckForCsvOrTxtFiles();

        if (hasFiles)
        {
            Console.WriteLine("Files found! Returning 0.");
            return 0;
        }
        else
        {
            Console.WriteLine("No files found. Returning 1.");
            return 1;
        }
    }
}