using Microsoft.Extensions.Configuration;
using Serilog;

namespace SftpFileChecking
{
    public class eDocsService
    {
        public bool CheckForCsvOrTxtFiles(IConfiguration config)
        {
            string localFolder = config["Config:LocalFolder"];
            string[] extension = config.GetSection("Config:Extension").Get<string[]>();

            Log.Information($"Checking local folder {localFolder}");

            if (!Directory.Exists(localFolder))
            {
                Log.Error($"Folder does not exist: {localFolder}");
                return false;
            }

            var files = Directory.GetFiles(localFolder);

            bool found = files.Any(f => extension.Any(ext =>
                f.EndsWith(ext, StringComparison.OrdinalIgnoreCase)));

            if (found)
            {
                Log.Information($"Found CSV/TXT file(s) in {localFolder}");
            }
            else
            {
                Log.Information($"No CSV/TXT file(s) in {localFolder}");
            }

            return found;
        }
    }
}