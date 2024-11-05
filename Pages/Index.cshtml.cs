using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace ServiceControllerApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public string ServiceStatus { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            ServiceStatus = GetServiceStatus();
        }

        public void OnPostStartService()
        {
            ExecuteCommand("sudo systemctl start myworker.service");
            ServiceStatus = GetServiceStatus();
        }

        public void OnPostStopService()
        {
            ExecuteCommand("sudo systemctl stop myworker.service");
            ServiceStatus = GetServiceStatus();
        }

        private string ExecuteCommand(string command)
        {
            var processInfo = new ProcessStartInfo("bash", "-c \"" + command + "\"")
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process();
            process.StartInfo = processInfo;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (!string.IsNullOrEmpty(error))
            {
                return $"Error: {error}";
            }

            return output;
        }

        public string GetServiceStatus()
        {
            return ExecuteCommand("sudo systemctl status myworker.service");
        }
    }
}

