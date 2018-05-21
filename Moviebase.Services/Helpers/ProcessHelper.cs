using System.Diagnostics;
using System.Security;
using System.Threading.Tasks;

namespace Moviebase.Services.Helpers
{
    public enum RedirectStream
    {
        StandardOutput,
        StandardError
    }

    [SecurityCritical]
    public static class ProcessHelper
    {
        [SecurityCritical]
        [DebuggerStepThrough]
        public static Task<string> StartWithOutput(string filePath, string arguments, RedirectStream stream)
        {
            var tcs = new TaskCompletionSource<string>();
            var process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    Arguments = arguments,
                    CreateNoWindow = true,
                    FileName = filePath,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    RedirectStandardError = stream == RedirectStream.StandardError,
                    RedirectStandardOutput = stream == RedirectStream.StandardOutput
                }
            };
            process.Exited += async (sender, args) =>
            {
                switch (stream)
                {
                    case RedirectStream.StandardError:
                        tcs.SetResult(await process.StandardError.ReadToEndAsync());
                        break;
                    case RedirectStream.StandardOutput:
                        tcs.SetResult(await process.StandardOutput.ReadToEndAsync());
                        break;
                }
            };

            process.Start();
            return tcs.Task;
        }

    }
}
