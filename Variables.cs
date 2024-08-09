using System.Reflection;

namespace Accelerator
{
    class Variables
    {
        public static string LogLevel()
        {
            return Environment.GetEnvironmentVariable("LOG_LEVEL") ?? "Info";
        }

        public static bool LogRegister()
        {
            string arg = Environment.GetEnvironmentVariable("LOG_REGISTER") ?? "true";
            return arg == "true" ? true : false;
        }

        public static bool NoHtml()
        {
            string arg = Environment.GetEnvironmentVariable("NO_HTML") ?? "false";
            return arg == "true" ? true : false;
        }

        public static async Task<string> GetIP()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("https://ifconfig.me/ip");
            response.EnsureSuccessStatusCode();
            var ipAddress = await response.Content.ReadAsStringAsync();
            return ipAddress.Trim();
        }

        public static bool NoCache()
        {
            string arg = Environment.GetEnvironmentVariable("NOCLIENTCACHE") ?? "false";
            return arg == "true" ? true : false;
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

        public static bool GetBoolArg(string arg)
        {
            string[] args = Environment.GetCommandLineArgs();
            bool returned = args.Contains(arg);
            return returned;
        }

        public static void Preset()
        {
            var logLevel = Environment.GetEnvironmentVariable("LOG_LEVEL") ?? "";
            if (!string.IsNullOrEmpty(logLevel) && IsValidLogLevel(logLevel))
            {
                Environment.SetEnvironmentVariable("LOG_LEVEL", logLevel);
            }
            else
            {
                Environment.SetEnvironmentVariable("LOG_LEVEL", "Info");
            }

            var logLevelRegister = Environment.GetEnvironmentVariable("LOG_REGISTER") ?? "";
            if (!string.IsNullOrEmpty(logLevel) && IsValidBoolean(logLevelRegister))
            {
                Environment.SetEnvironmentVariable("LOG_REGISTER", logLevelRegister);
            }
            else
            {
                Environment.SetEnvironmentVariable("LOG_REGISTER", "true");
            }

            var noHtml = Environment.GetEnvironmentVariable("NO_HTML") ?? "";
            if (!string.IsNullOrEmpty(noHtml) && IsValidBoolean(noHtml))
            {
                Environment.SetEnvironmentVariable("NO_HTML", noHtml);
            }
            else
            {
                Environment.SetEnvironmentVariable("NO_HTML", "true");
            }

            var noClientCache = Environment.GetEnvironmentVariable("NOCLIENTCACHE") ?? "false";
            if (!string.IsNullOrEmpty(noClientCache) && IsValidBoolean(noClientCache))
            {
                Environment.SetEnvironmentVariable("NOCLIENTCACHE", noClientCache);
            }
            else
            {
                Environment.SetEnvironmentVariable("NOCLIENTCACHE", "false");
            }
        }

        private static bool IsValidLogLevel(string logLevel)
        {
            var validLogLevels = new[] { "Error", "Warn", "Info", "Debug" };
            return validLogLevels.Contains(logLevel);
        }

        private static bool IsValidBoolean(string value)
        {
            var validBooleans = new[] { "true", "false" };
            return validBooleans.Contains(value);
        }
    }
}
