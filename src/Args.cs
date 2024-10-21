using System.Reflection;

namespace SampleFDL;

class Args
{
    public static async Task<string> GetIP()
    {
        var client = new HttpClient();
        var response = await client.GetAsync("https://ifconfig.me/ip");
        response.EnsureSuccessStatusCode();
        var ipAddress = await response.Content.ReadAsStringAsync();
        return ipAddress.Trim();
    }

    public static string GetEmbeddedResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        string resourceContent;
        Stream? stream = assembly.GetManifestResourceStream(resourceName);
        if (stream != null)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                resourceContent = reader.ReadToEnd();
            }
        }
        else
        {
            throw new InvalidOperationException($"Resource '{resourceName}' not found.");
        }
        return resourceContent;
    }

    public static string[] GetArrayString(string arg, string[] defaultValues)
    {
        string[] args = Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == arg && i + 1 < args.Length)
            {
                return args[i + 1].Split(',');
            }
        }

        return defaultValues;
    }



    public static string GetStringArg(string arg, string def = "")
    {
        string[] args = Environment.GetCommandLineArgs();
        string returned = def;
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == $"{arg}" && i + 1 < args.Length)
            {
                returned = args[i + 1];
            }
        }
        return returned;
    }

    public static int GetIntArg(string arg, int def = 0)
    {
        string[] args = Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == arg && i + 1 < args.Length)
            {
                if (int.TryParse(args[i + 1], out int result))
                {
                    return result;
                }
            }
        }

        return def;
    }

    public static bool GetBoolArg(string arg)
    {
        string[] args = Environment.GetCommandLineArgs();
        bool returned = args.Contains(arg);
        return returned;
    }
}
