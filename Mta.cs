using System.Diagnostics;

namespace Accelerator
{
    class MtaStarter 
    {
        public static void Execute(string filePath, string[] args)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = filePath,
                    Arguments = string.Join(" ", args),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };

            process.OutputDataReceived += (sender, data) => 
            {
                Logger.Info(data.Data!.ToString());
            };

            process.ErrorDataReceived += (sender, data) => 
            {
                Logger.Error(data.Data!.ToString());
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }
    }
}