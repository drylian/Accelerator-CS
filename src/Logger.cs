namespace SampleFDL;

public class Logger
{
    public const string Format = "[{type}] [{time}].lgray-b {message}";

    public static int Level(string level = "")
    {

        switch (level)
        {
            case "Debug":
                return 4;
            case "Info":
                return 3;
            case "Warn":
                return 2;
            case "Error":
                return 1;
            default:
                return 0;
        }
    }

    private static void Log(string message, string type)
    {
        string msg = Format;
        DateTime now = DateTime.Now;
        string time = now.ToString("HH:mm:ss");
        msg = msg.Replace("{time}", time);

        switch (type)
        {
            case "Info":
                msg = msg.Replace("{type}", $"[{Color.Colors(ANSIColors.Blue, type)}].bold");
                break;
            case "Debug":
                msg = msg.Replace("{type}", $"[{Color.Colors(ANSIColors.Magenta, type)}].bold");
                break;
            case "Warn":
                msg = msg.Replace("{type}", $"[{Color.Colors(ANSIColors.Yellow, type)}].bold");
                break;
            case "Error":
                msg = msg.Replace("{type}", $"[{Color.Colors(ANSIColors.Red, type)}].bold");
                break;
        }

        var result = Color.Formatter(msg.Replace("{message}", message));
        if (Level(Config.LogLevel) >= Level(type))
        {
            Console.WriteLine(result.csl);
        }
        if (Level(Config.LogLevel) >= Level(type) && Config.AllowRegisterLogs)
        {
            Register(result.rgt);
        }
    }

    private static void Syslog(string msg)
    {
        var result = Color.Formatter($"[Trash].red-b {msg}");
        Console.WriteLine(result.csl);
    }

    private static void Register(string msg)
    {
        DateTime data = DateTime.Now;
        string time = data.ToString("yyyy-MM-dd");
        string logDir = Config.LogRegisterPath;
        string filePath = Path.Combine(logDir, $"{time}.log");

        try
        {
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }

            if (Directory.Exists(logDir))
            {
                if (Directory.GetFiles(logDir).Length > 10)
                {
                    string[] logFiles = Directory
                        .GetFiles(logDir)
                        .Where(file =>
                            file.EndsWith(".log", StringComparison.OrdinalIgnoreCase)
                        )
                        .OrderBy(file => File.GetLastWriteTimeUtc(file))
                        .ToArray();

                    int filesToDeleteCount = logFiles.Length - 10;
                    string[] filesToDelete = logFiles.Take(filesToDeleteCount).ToArray();

                    foreach (string file in filesToDelete)
                    {
                        string filePathToDelete = Path.Combine(file);
                        Syslog($"Deleting [old logs].red {file}");
                        File.Delete(filePathToDelete);
                    }
                }
            }

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(msg);
            }
        }
        catch (Exception ex)
        {
            Syslog($"Error in register log: {ex.Message}");
        }
    }

    public static void Info(string msg)
    {
        Log(msg, "Info");
    }

    public static void Warn(string msg)
    {
        Log(msg, "Warn");
    }

    public static void Error(string msg)
    {
        Log(msg, "Error");
    }

    public static void Debug(string msg)
    {
        Log(msg, "Debug");
    }
}
