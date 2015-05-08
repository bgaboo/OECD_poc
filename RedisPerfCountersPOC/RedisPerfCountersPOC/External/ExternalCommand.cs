using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisPerfCountersPOC.External
{
    public class ExternalCommand
    {
        public static InfoResult CallRedis()
        {
            return CallRedis(null, null);
        }

        public static InfoResult CallRedis(string server, int? port)
        {
            string serverPort = string.Empty;
            if (!string.IsNullOrWhiteSpace(server))
                serverPort = "-h " + serverPort;
            if (port.HasValue)
            {
                if (serverPort != string.Empty)
                    serverPort += " ";
                serverPort += "-p " + port.Value;
            }
            return Run("C:\\Redis\\bin", "redis-cli", string.Format("{0} info all", serverPort));
        }

        internal static InfoResult Run(string path, string filename, string arguments = null)
        {
            string output = string.Empty;
            bool isSuccessful = true;

            Process process = new Process();

            process.StartInfo.FileName = Path.Combine(path, filename);
            if (!string.IsNullOrEmpty(arguments))
            {
                process.StartInfo.Arguments = arguments;
            }

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            StringBuilder stdOutput = new StringBuilder();
            process.OutputDataReceived += (sender, args) => stdOutput.AppendLine(args.Data);

            string stdError = null;
            try
            {
                process.Start();
                process.BeginOutputReadLine();
                stdError = process.StandardError.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                output = string.Format("Operating system error while executing process {0}\r\n\r\n", FormatCommand(filename, arguments), e.Message);
                isSuccessful = false;
                //throw new Exception(output, e);
            }

            if (isSuccessful)
            {
                if (process.ExitCode == 0)
                {
                    output = stdOutput.ToString();
                }
                else
                {
                    output = string.Format("Process {0} finished with exit code = {1}\r\n\r\n{2}{3}",
                        FormatCommand(filename, arguments), process.ExitCode,
                        (string.IsNullOrEmpty(stdError) ? string.Empty : string.Format("{0}\r\n\r\n", stdError)),
                        (stdOutput.ToString().Length == 0 ? string.Empty : string.Format("Output:\r\n{0}\r\n", stdOutput.ToString())));
                    //throw new Exception(output);
                    isSuccessful = false;
                }
            }

            return new InfoResult(output);
        }

        private static string FormatCommand(string filename, string arguments)
        {
            return string.Format("'{0} {1}'", filename, string.IsNullOrWhiteSpace(arguments) ? string.Empty : arguments).Trim();

        }
    }
}
