namespace SampleFDL;

public class Updater
{
    private static readonly string UpdateVersionUrl =
        "https://raw.githubusercontent.com/drylian/SampleFDL/main/src/version";
    private static readonly string DownloadUrl =
        "https://github.com/drylian/SampleFDL/raw/main/release/SampleFDL";

    private static async Task<string> FetchRemoteVersionAsync(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string version = await response.Content.ReadAsStringAsync();
            return version.Trim();
        }
    }

    private static async Task DownloadFileAsync(string url, string fileName)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync(fileName, fileBytes);

            Logger.Info($"File downloaded successfully to {fileName}");
        }
    }

    public static async Task UpdateAsync()
    {
        try
        {
            string remoteVersion = await FetchRemoteVersionAsync(UpdateVersionUrl);
            string localVersion = Config.Version;

            if (remoteVersion != localVersion)
            {
                Logger.Info(
                    $"A new version is available. Local version: [{localVersion}].blue, Remote version: [{remoteVersion}].green"
                );
                Logger.Info("Downloading new [version].green...");

                await DownloadFileAsync(DownloadUrl, "SampleFDL-update");
                Logger.Info("Downloaded");
            }
            else
            {
                Logger.Info($"The SampleFDL is [up-to-date].green. Version: [{remoteVersion}].green");
            }
        }
        catch
        {
            Logger.Warn($"Error to checking latest version, [ignoring].yellow...");
        }
    }
}
