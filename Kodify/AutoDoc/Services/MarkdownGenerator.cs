using System;
using System.Collections.Generic;
using System.IO;
using Kodify.AutoDoc.Models;
using Kodify.AI.Services;
using LibGit2Sharp;

namespace Kodify.AutoDoc.Services
{
    public class MarkdownGenerator
    {
        private readonly OpenAIService _aiService;

        public MarkdownGenerator(OpenAIService aiService)
        {
            _aiService = aiService;
        }

        public async Task GenerateReadMe(List<DocumentationModel> documentation, string outputPath, string projectName, string projectSummary, string usageInstructions, string template = null)
        {
            try
            {
                // Ensure the output directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                using (var writer = new StreamWriter(outputPath))
                {
                    // Use the provided template if available
                    if (!string.IsNullOrEmpty(template))
                    {
                        // Logic to apply the template to the documentation
                    }

                    // Generate documentation
                    var docGen = await _aiService.GenerateDocumentationAsync(projectName, projectSummary, usageInstructions, string.Join("\n", documentation));

                    // Write documentation to the file
                    writer.WriteLine(docGen);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating markdown: {ex.Message}");
            }
        }

        public bool ValidateReadme(string readmeContent)
        {
            // Logic to validate README completeness
            return true; // Placeholder
        }

        public void GenerateChangelog(string outputPath)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                using (var writer = new StreamWriter(outputPath))
                {
                    writer.WriteLine("# Changelog");

                    string repoPath = Repository.Discover(Environment.CurrentDirectory);
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
                                writer.WriteLine($"- {c.MessageShort} ({c.Sha.Substring(0, 7)})");
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
                                writer.WriteLine($"- {c.MessageShort} ({c.Sha.Substring(0, 7)})");
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