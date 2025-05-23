using System.Text;
using Kodify.Repository.Models;

namespace Kodify.AutoDoc.Services
{
    public class ContentBuilder : IContentBuilder
    {
        public virtual string BuildStructuredContent(ProjectInfo projectInfo)
        {
            var sb = new StringBuilder();

            // Only show API status if detected
            if (projectInfo.HasWebApi)
            {
                sb.AppendLine("### Technical Overview");
                sb.AppendLine("- API Detected: Yes");
            }

            // Filter out autogenerated files (a simple example).
            var filteredFiles = projectInfo.SourceFiles
                .Where(f => !IsAutogeneratedFile(f.FilePath))
                .ToList();

            if (filteredFiles.Any())
            {
                sb.AppendLine("\n### Key Features");
                sb.AppendLine("- Interactive user interface");
                sb.AppendLine("- Responsive design");
                sb.AppendLine("- Cross-browser compatibility");
            }

            sb.AppendLine($"License Status: {projectInfo.License.Type}");
            if (projectInfo.License.Type != "None")
            {
                sb.AppendLine($"License File: {Path.GetFileName(projectInfo.License.FilePath)}");
            }

            return sb.ToString();
        }

        public virtual List<string> BuildManualContent(ProjectInfo projectInfo, string projectName, string projectSummary, string usageInstructions)
        {
            var content = new List<string>
            {
                $"# {projectName}",
                $"![Build Status](https://img.shields.io/badge/build-passing-brightgreen)",
                GetLicenseBadge(projectInfo.License),
                projectSummary,
                "",
                "## Table of Contents",
                "- [Features](#features)",
                "- [Installation](#installation)",
                "- [Usage](#usage)",
                "- [Project Structure](#project-structure)",
                "- [API Reference](#api-reference)",
                "- [Contributing](#contributing)",
                "- [License](#license)",
                "",
                "## Features"
            };

            content.AddRange(DetectFeatures(projectInfo));
            content.Add("");

            return content;
        }

        private IEnumerable<string> DetectFeatures(ProjectInfo projectInfo)
        {
            var features = new List<string>();

            if (projectInfo.SourceFiles.Any(f => f.FilePath.Contains("Services")))
                features.Add("- Service layer architecture");

            if (projectInfo.SourceFiles.Any(f => f.FilePath.Contains("Repositories")))
                features.Add("- Database repository pattern");

            if (projectInfo.SourceFiles.Any(f => f.FilePath.EndsWith("Tests.cs")))
                features.Add("- Unit test coverage");

            if (projectInfo.SourceFiles.Any(f => f.FilePath.Contains("Controllers")))
                features.Add("- Web API endpoints");

            if (!features.Any())
            {
                features.Add("- Clean architecture");
                features.Add("- Modern development practices");
                features.Add("- Comprehensive documentation");
            }

            return features;
        }

        private bool IsAutogeneratedFile(string filePath)
        {
            var excludedFiles = new[]
            {
                "AssemblyInfo.cs",
                "GlobalUsings.g.cs",
                ".NETCoreApp,Version"
            };

            return excludedFiles.Any(e => filePath.Contains(e));
        }

        private string GetLicenseBadge(LicenseInfo license)
        {
            var licenseType = license.Type switch
            {
                "MIT" => "MIT",
                "Apache-2.0" => "Apache%202.0",
                "GPL-3.0" => "GPL%203.0",
                "MPL-2.0" => "MPL%202.0",
                "Unlicense" => "Unlicense",
                "Custom" => "Custom",
                _ => "None"
            };

            return $"![License](https://img.shields.io/badge/license-{licenseType}-blue)";
        }
    }
} 