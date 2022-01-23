namespace Mint.Common.Utilities
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public static class Command
    {
        /// <summary>
        /// Executes a specified command on specified directory.
        /// The default directory is the current working directory of the application.
        /// </summary>
        public static void Execute(string command, string directory = null)
        {
            using (var process = new Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WorkingDirectory = directory ?? Directory.GetCurrentDirectory();
                process.StartInfo.FileName = Path.Combine(Environment.SystemDirectory, "cmd.exe");

                // Redirects the standard input so that commands can be sent to the shell.
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                // process.StartInfo.RedirectStandardError = true;

                // process.OutputDataReceived += ProcessOutputDataHandler;
                // process.ErrorDataReceived += ProcessErrorDataHandler;

                process.Start();
                // process.BeginOutputReadLine();
                // process.BeginErrorReadLine();

                // Send command and exit.
                process.StandardInput.WriteLine(command);
                process.StandardInput.WriteLine("exit");
                process.WaitForExit();
            }
        }

        private static void ProcessOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            // ConsoleLog.Debug(outLine.Data);
        }

        private static void ProcessErrorDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            // ConsoleLog.Debug(outLine.Data);
        }
    }
}
