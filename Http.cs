using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Accelerator
{
    public class Http
    {
        private static readonly string MainHtml =
            @"
                <html lang=""en"">
                    <head>
                        <meta charset=""UTF-8"" />
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                        <title>Accelerator Resources</title>
                        <link rel=""shortcut icon"" href=""https://raw.githubusercontent.com/drylian/tralhas/main/mta.png"" type=""image/png"" style=""border-radius: '50px'; "" />
                        <script src=""https://cdn.tailwindcss.com""></script>
                    </head>
                    <body class=""bg-black"">
                        <div class=""flex justify-center items-center flex-col mx-auto px-4 py-8"">
                            <img class=""w-60 h-36"" src=""https://raw.githubusercontent.com/drylian/tralhas/main/mta.png"" alt=""Logo"" />
                            <h1 class=""text-4xl text-white font-bold text-center mb-4"">Accelerator Running</h1>
                            <p class=""text-lg text-gray-300 text-center"">This port is currently being used to improve your experience on the server!.</p>
                        </div>
                    </body>
                </html>
                ";

        public static WebApplication Start(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddCompression();
            builder.Services.AddResponseCompression(options =>
            {
                // options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });
            builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            builder.Services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.SmallestSize;
            });
            builder.Logging.ClearProviders();
            builder.Services.AddRouting();
            var app = builder.Build();
            app.UseCompression();
            app.UseResponseCompression();
            app.UseRouting();
            Configure(app);
            return app;
        }

        public static void Configure(WebApplication app)
        {
            app.Map(
                "/",
                async (HttpResponse context) =>
                {
                    bool noHtml = Variables.NoHtml();
                    if (noHtml)
                    {
                        await context.WriteAsync(MainHtml);
                    }
                    else
                    {
                        await context.WriteAsync("Access denied");
                    }
                }
            );

            app.Map(
                "/{*path}",
                async (context) =>
                {
                    var originalUrl = context.Request!.Path!.Value!.TrimStart('/');
                    if (originalUrl == "favicon.ico")
                        return;
                    var currentDir = app.Environment.ContentRootPath;
                    var modsDir = Path.Combine(currentDir, "mods");
                    var modsDirectories = Directory.EnumerateDirectories(modsDir);
                    var check = false;

                    foreach (var dir in modsDirectories)
                    {
                        var folder = Path.GetFileName(dir);

                        var paths = new[]
                        {
                            "resource-cache/http-client-files",
                            "resource-cache/http-client-files-no-client-cache"
                        };
                        var fileLocale = originalUrl;

                        foreach (var local in paths)
                        {
                            var noCache = Variables.NoCache();
                            if (noCache)
                            {
                                continue;
                            }

                            var filePath = Path.Combine(modsDir, folder, local, fileLocale);

                            if (File.Exists(filePath))
                            {
                                check = true;
                                Logger.Info($"FileURL: [{originalUrl}].green | [Found].green");
                                var fileInfo = new FileInfo(filePath);
                                context.Response.ContentType = "text/plain;charset=utf-8";
                                await using (var stream = fileInfo.OpenRead())
                                {
                                    await stream.CopyToAsync(context.Response.Body);
                                }
                            }
                        }
                    }

                    if (!check)
                    {
                        context.Response.StatusCode = 404;
                        Logger.Info($"FileURL: [{originalUrl}].green | [Not Found].red");
                        await context.Response.WriteAsync("Resource not found");
                    }
                }
            );
        }
    }
}
