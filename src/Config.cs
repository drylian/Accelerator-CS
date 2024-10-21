namespace SampleFDL;

class Config
{
    // Version of app
    public static readonly string Version = "1.0.0";
    // Show logs for terminal
    public static string LogLevel = "Info";
    // Locale of registred logs
    public static string LogRegisterPath = "./logs";
    // Allow register logs
    public static bool AllowRegisterLogs = true;
    // Port of application
    public static int Port = 3000;
    // Paths of resources
    public static string[] ResourcesPath = new[]
    {
        "public",
    };
    // Allow Html in /
    public static bool AllowHtml = true;
    // Ip used in http
    public static string Ip = "0.0.0.0";
    // Allow domain/ip
    public static string Domain = "";
    // Active Https secure
    public static bool HttpsSecure = false;
    // Html of Application
    public static string Html =
        @"
            <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"" />
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                    <title>Accelerator Resources</title>
                    <link rel=""shortcut icon"" href=""https://github.com/drylian/SampleFDL/blob/main/logo.png?raw=true"" type=""image/png"" style=""border-radius: '50px'; "" />
                    <script src=""https://cdn.tailwindcss.com""></script>
                </head>
                <body class=""bg-black"">
                    <div class=""flex justify-center items-center flex-col mx-auto px-4 py-8"">
                        <img class=""w-60 h-36"" src=""https://github.com/drylian/SampleFDL/blob/main/logo.png?raw=true"" alt=""Logo"" />
                        <h1 class=""text-4xl text-white font-bold text-center mb-4"">Accelerator Running</h1>
                        <p class=""text-lg text-gray-300 text-center"">This port is currently being used to improve your experience on the game server!.</p>
                    </div>
                </body>
            </html>
            ";
    // Load Configurations
    public static async Task PrepareConfig()
    {
        AllowHtml = Args.GetBoolArg("--allow-html");
        HttpsSecure = Args.GetBoolArg("--allow-https");
        ResourcesPath = Args.GetArrayString("--resources", ResourcesPath);
        AllowRegisterLogs = Args.GetBoolArg("--log-register");
        LogRegisterPath = Args.GetStringArg("--log-path", LogRegisterPath);
        LogLevel = Args.GetStringArg("--log-level", "Info");
        Domain = Args.GetStringArg("--domain", await Args.GetIP());
        // check if log level is valid
        if (!new[] { "Error", "Warn", "Info", "Debug" }.Contains(LogLevel))
        {
            LogLevel = "Info";
        }

        Port = Args.GetIntArg("--port", Port);
        Ip = Args.GetStringArg("--ip", Ip);
    }
}
