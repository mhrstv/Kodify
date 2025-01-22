using System.Diagnostics;

namespace AutoDoc.Services
{
    public class DocFxGenerator
    {
        public void GenerateDocFx(string inputPath, string outputPath)
        {
            // Assuming DocFX is installed and available in the PATH
            var startInfo = new ProcessStartInfo
            {
                FileName = "docfx",
                Arguments = $"build {inputPath} -o {outputPath}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(startInfo))
            {
                process.WaitForExit();
            }
        }
    }
}