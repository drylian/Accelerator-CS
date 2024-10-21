using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Runtime.InteropServices;

namespace SampleFDL;

internal class Program
{
    static async Task Main(string[] args)
    {
        await Config.PrepareConfig();
        Logger.Info("Starting SampleDFL");
        Logger.Debug("");
        Logger.Debug("---[ Configurations ]---");
        Architecture architecture = RuntimeInformation.ProcessArchitecture;

        switch (architecture)
        {
            case Architecture.X64:
                Logger.Debug("Architecture:[ x64 (64 bits)].cyan");
                break;
            case Architecture.X86:
                Logger.Debug("Architecture: [x86 (32 bits)].cyan");
                break;
            case Architecture.Arm:
                Logger.Debug("Architecture: [ARM].cyan");
                break;
            case Architecture.Arm64:
                Logger.Debug("Architecture: [ARM64 (64 bits)]].cyan");
                break;
            case Architecture.Wasm:
                Logger.Debug("Architecture: [WebAssembly].cyan");
                break;
            default:
                Logger.Debug("Architecture: [Unknown].red");
                break;
        }
        Logger.Debug($"Version: [{Config.Version}].magenta");
        Logger.Debug($"Https: [{Config.HttpsSecure}].green");
        Logger.Debug($"Port: [{Config.Port}].cyan");
        Logger.Debug($"Ip Allow: [{Config.Ip}].yellow");
        Logger.Debug($"Domain/AccessIP: [{Config.Domain}].magenta");
        Logger.Debug($"Allow Html: [{Config.AllowHtml}].green");
        Logger.Debug($"Log Level: [{Config.LogLevel}].green");
        Logger.Debug($"Log Register Path: [{Config.LogRegisterPath}].green");
        Logger.Debug($"Allow Register Logs: [{Config.AllowRegisterLogs}].green");
        Logger.Debug($"Resources Path: [{string.Join(", ", Config.ResourcesPath)}].magenta");

        Logger.Debug("---[ End Configurations ]---");
        Logger.Info("All configurations have been loaded [successfully].green");
        Logger.Info("Starting [SampleFDL].blue, checking for [updates].green");
        await Updater.UpdateAsync();

        Logger.Info("Starting [Hosting].blue, awaiting...");
        var app = WebHosting.Start(args);
        string ip = await Args.GetIP();
        app.Services.GetRequiredService<IHostApplicationLifetime>()
            .ApplicationStarted.Register(() =>
            {
                Logger.Info($"Started [SampleFDL].green (IP:{Config.Ip}) [{(Config.HttpsSecure ? "https" : "http")}://{Config.Domain}:{Config.Port}].blue.");
            });
        await app.RunAsync($"{(Config.HttpsSecure ? "https" : "http")}://{Config.Ip}:{Config.Port}");
    }
}
