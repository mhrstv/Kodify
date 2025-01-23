using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kodify.AutoDoc.Models;
using Kodify.AI.Services;
using LibGit2Sharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kodify.AutoDoc.Services
{
    public class MarkdownGenerator
    {
        private readonly OpenAIService _aiService;

        public MarkdownGenerator(OpenAIService aiService)
        {
            _aiService = aiService;
        }

        public async Task GenerateReadMe(
            ProjectInfo projectInfo,
            string outputPath,
            string projectName,
            string projectSummary,
            string usageInstructions,
            string template = null)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                using (var writer = new StreamWriter(outputPath))
                {
                    string finalContent;

                    if (!string.IsNullOrEmpty(template))
                    {
                        var readmeContent = BuildManualContent(projectInfo, projectName, projectSummary, usageInstructions);
                        finalContent = await _aiService.EnhanceDocumentationAsync(template, readmeContent);
                    }
                    else
                    {
                        var codeContent = GetCodeContent(projectInfo);
                        finalContent = await _aiService.GenerateDocumentationAsync(
                            projectName,
                            projectSummary,
                            usageInstructions,
                            codeContent);
                    }

                    writer.WriteLine(finalContent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating markdown: {ex.Message}");
                throw;
            }
        }

        private List<string> BuildManualContent(
            ProjectInfo projectInfo,
            string projectName,
            string projectSummary,
            string usageInstructions)
        {
            var content = new List<string>
            {
                $"# {projectName}",
                "![Build Status](https://img.shields.io/badge/build-passing-brightgreen)",
                "![License](https://img.shields.io/badge/license-MIT-blue)",
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

            // Dynamic feature detection
            content.AddRange(DetectFeatures(projectInfo));
            content.Add("");

            // Installation section
            content.AddRange(GetInstallationInstructions(projectInfo));
            content.Add("");

            // Usage section
            content.AddRange(GetUsageInstructions(usageInstructions, projectInfo));
            content.Add("");

            // Project Structure
            content.Add("## Project Structure");
            content.Add("```");
            content.AddRange(projectInfo.Structure.Directories
                .Select(d => d.Replace(projectInfo.ProjectPath, "")));
            content.Add("```");
            content.Add("");

            // API Reference
            content.Add("## API Reference");
            content.AddRange(GenerateApiReference(projectInfo));
            content.Add("");

            // Standard sections
            content.AddRange(new[]
            {
                "## Contributing",
                "1. Fork the Project",
                "2. Create your Feature Branch",
                "3. Commit your Changes",
                "4. Push to the Branch",
                "5. Open a Pull Request",
                "",
                "## License",
                "Distributed under the MIT License. See `LICENSE` for more information."
            });

            return content;
        }

        private IEnumerable<string> DetectFeatures(ProjectInfo projectInfo)
        {
            var features = new List<string>();

            // Detect architectural features
            if (projectInfo.SourceFiles.Any(f => f.FilePath.Contains("Services")))
                features.Add("- Service layer architecture");

            if (projectInfo.SourceFiles.Any(f => f.FilePath.Contains("Repositories")))
                features.Add("- Database repository pattern");

            // Detect testing framework
            if (projectInfo.SourceFiles.Any(f => f.FilePath.EndsWith("Tests.cs")))
                features.Add("- Unit test coverage");

            // Detect web features
            if (projectInfo.SourceFiles.Any(f => f.FilePath.Contains("Controllers")))
                features.Add("- Web API endpoints");

            // Fallback if no features detected
            if (!features.Any())
            {
                features.Add("- Clean architecture");
                features.Add("- Modern development practices");
                features.Add("- Comprehensive documentation");
            }

            return features;
        }

        private IEnumerable<string> GetInstallationInstructions(ProjectInfo projectInfo)
        {
            var instructions = new List<string>
            {
                "## Installation",
                "```bash",
                $"git clone {GetRepositoryUrl(projectInfo)}",
                $"cd {Path.GetFileName(projectInfo.ProjectPath)}",
                "dotnet restore",
                "```"
            };

            // Detect package managers
            if (File.Exists(Path.Combine(projectInfo.ProjectPath, "package.json")))
                instructions.Add("npm install");

            if (File.Exists(Path.Combine(projectInfo.ProjectPath, "Dockerfile")))
                instructions.Add("docker build -t myapp .");

            instructions.Add("```");
            return instructions;
        }

        private string GetRepositoryUrl(ProjectInfo projectInfo)
        {
            try
            {
                var repoPath = Repository.Discover(projectInfo.ProjectPath);
                using var repo = new Repository(repoPath);
                return repo.Network.Remotes["origin"]?.Url ?? "https://github.com/user/repository.git";
            }
            catch
            {
                return "https://github.com/user/repository.git";
            }
        }

        private IEnumerable<string> GetUsageInstructions(string usageInstructions, ProjectInfo projectInfo)
        {
            var usage = new List<string>
            {
                "## Usage",
                usageInstructions
            };

            // Find a representative class
            var sampleClass = projectInfo.SourceFiles
                .SelectMany(f => f.Classes)
                .FirstOrDefault(c => c.Methods.Any());

            if (sampleClass != null)
            {
                usage.Add("```csharp");
                usage.Add($"var service = new {sampleClass.Name}();");
                if (sampleClass.Methods.Any())
                    usage.Add($"service.{sampleClass.Methods.First().Name}();");
                usage.Add("```");
            }

            return usage;
        }

        private IEnumerable<string> GenerateApiReference(ProjectInfo projectInfo)
        {
            var apiContent = new List<string>();

            foreach (var file in projectInfo.SourceFiles)
            {
                apiContent.Add($"### {Path.GetFileName(file.FilePath)}");

                foreach (var cls in file.Classes)
                {
                    apiContent.Add($"#### {cls.Name}");
                    apiContent.Add(cls.Summary);

                    if (cls.Methods.Count > 0)
                    {
                        apiContent.Add("**Methods:**");
                        foreach (var method in cls.Methods)
                        {
                            var parameters = string.Join(", ",
                                method.Parameters.Select(p => $"{p.Type} {p.Name}"));
                            apiContent.Add($"- `{method.ReturnType} {method.Name}({parameters})`");
                            apiContent.Add($"  {method.Summary}");
                        }
                    }
                }
            }

            return apiContent;
        }

        private string GetCodeContent(ProjectInfo projectInfo)
        {
            var codeContents = new List<string>();
            foreach (var file in projectInfo.SourceFiles)
            {
                try
                {
                    codeContents.Add(File.ReadAllText(file.FilePath));
                }
                catch
                {
                    // Gracefully handle unreadable files
                }
            }
            return string.Join("\n\n", codeContents);
        }

        public void GenerateChangelog(string outputPath)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                using (var writer = new StreamWriter(outputPath))
                {
                    writer.WriteLine("# Changelog");
                    writer.WriteLine("\nAll notable changes to this project will be documented in this file.\n");

                    var repoPath = Repository.Discover(Environment.CurrentDirectory);
                    if (repoPath == null)
                    {
                        writer.WriteLine("No Git repository found.");
                        return;
                    }

                    using (var repo = new Repository(repoPath))
                    {
                        var tags = repo.Tags
                            .Where(t => t.IsAnnotated)
                            .OrderByDescending(t => ((Commit)t.Target).Author.When)
                            .ToList();

                        foreach (var tag in tags)
                        {
                            var commit = (Commit)tag.Target;
                            writer.WriteLine($"## {tag.FriendlyName} ({commit.Author.When:yyyy-MM-dd})");
                            writer.WriteLine();

                            var previousTag = tags.ElementAtOrDefault(tags.IndexOf(tag) + 1);
                            var filter = new CommitFilter
                            {
                                IncludeReachableFrom = commit,
                                ExcludeReachableFrom = previousTag?.Target
                            };

                            foreach (var c in repo.Commits.QueryBy(filter))
                            {
                                writer.WriteLine($"- {c.MessageShort} ({c.Sha[..7]})");
                            }
                            writer.WriteLine();
                        }

                        // Add unreleased changes
                        var lastTag = tags.LastOrDefault()?.Target;
                        if (lastTag != null)
                        {
                            writer.WriteLine("## Unreleased");
                            writer.WriteLine();

                            var filter = new CommitFilter
                            {
                                IncludeReachableFrom = repo.Head.Tip,
                                ExcludeReachableFrom = lastTag
                            };

                            foreach (var c in repo.Commits.QueryBy(filter))
                            {
                                writer.WriteLine($"- {c.MessageShort} ({c.Sha[..7]})");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating changelog: {ex.Message}");
            }
        }
    }
}