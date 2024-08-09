using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Accelerator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Variables.Preset();
            Logger.Info("Starting [application].blue, checking for [updates].green");
            await Updater.UpdateAsync();
            Logger.Info("Starting [webServer].blue, awaiting...");
            var app = Http.Start(args);
            string ip = await Variables.GetIP();
            string port = Variables.GetStringArg("--accelerator", "3000");
            app.Services.GetRequiredService<IHostApplicationLifetime>()
                .ApplicationStarted.Register(() =>
                {
                    Logger.Info($"Started Accelerator [http://{ip}:{port}].blue, starting mta...");
                    MtaStarter.Execute("./", args);
                });
            await app.RunAsync($"http://0.0.0.0:{port}");
        }
    }
}
