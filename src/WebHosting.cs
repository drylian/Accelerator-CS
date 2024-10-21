using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SampleFDL;

public class WebHosting
{
    public static WebApplication Start(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddCompression();
        builder.Services.AddResponseCompression(options =>
        {
            if (Config.HttpsSecure) options.EnableForHttps = true;
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
                if (Config.AllowHtml)
                {
                    await context.WriteAsync(Config.Html);
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
                var check = false;

                var paths = Config.ResourcesPath;

                foreach (var local in paths)
                {
                    var resourcePath = Path.Combine(currentDir, local);
                    var filePath = Path.Combine(resourcePath, originalUrl);

                    if (Directory.Exists(resourcePath))
                    {
                        if (File.Exists(filePath))
                        {
                            var directoryName = Path.GetDirectoryName(filePath);
                            if (directoryName != null && File.Exists(Path.Combine(directoryName, ".fastignore")))
                            {
                                // fastignore
                                continue;
                            }
                            check = true;
                            Logger.Info($"URL: [/{originalUrl}].green");
                            Logger.Debug($"FILE: [{filePath}].magenta");

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
                    Logger.Info($"URL: [/{originalUrl}].red | [Not found].red");
                    await context.Response.WriteAsync("Resource not found");
                }
            });
    }
}
