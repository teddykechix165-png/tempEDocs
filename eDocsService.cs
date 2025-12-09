public bool CheckForCsvOrTxtFiles(IConfiguration config)
{
    try
    {
        string localFolder = config["Config:LocalFolder"];
        string[] extensions = config.GetSection("Config:Extension").Get<string[]>();

        Log.Information($"Checking local folder {localFolder}");

        if (!Directory.Exists(localFolder))
        {
            Log.Error($"Folder does not exist: {localFolder}");
            return true; // exception state → return 1
        }

        bool foundInLoop;

        do
        {
            // Get matching files
            var files = Directory.GetFiles(localFolder)
                .Where(f => extensions.Any(ext =>
                    f.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            foundInLoop = files.Any();

            // Delete all found files
            foreach (var file in files)
            {
                try
                {
                    Log.Information($"Deleting file: {file}");
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to delete file {file}: {ex.Message}");
                    return true; // return 1 on ANY delete exception
                }
            }

        } while (foundInLoop); // keep looping until folder has no more matching files

        Log.Information("File check & delete completed successfully. Returning 0.");
        return false; // success (0)
    }
    catch (Exception ex)
    {
        Log.Error($"Unexpected error: {ex.Message}");
        return true; // return 1 on any unhandled exception
    }
}