using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerSoft.DevToolkit.Models;

namespace TowerSoft.DevToolkit.Utilities {
    public static class ProcessUtilities {
        public static async Task<ProcessResult> GetOutput(string filename, string arguements, string workingDirectory) {
            using Process process = new() {
                StartInfo = new() {
                    FileName = filename,
                    Arguments = arguements,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            await process.WaitForExitAsync();

            ProcessResult result = new() {
                Output = process.StandardOutput.ReadToEnd(),
                Error = process.StandardError.ReadToEnd()
            };

            return result;
        }
    }
}
