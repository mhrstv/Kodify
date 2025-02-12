using System;
using System.IO;
using System.Threading.Tasks;
using Kodify.AutoDoc.Models;
using Kodify.AI.Services;
using Kodify.AutoDoc.Services.Repository;

namespace Kodify.AutoDoc.Services.Documentation
{
    public class ReadmeGenerator
    {
        private readonly OpenAIService _aiService;
        private readonly ContentBuilder _contentBuilder;
        private readonly GitRepositoryService _gitService;

        // New constructor that only requires the OpenAIService.
        public ReadmeGenerator(OpenAIService aiService)
            : this(aiService, new ContentBuilder(), new GitRepositoryService())
        {
        }

        // Constructor that allows explicit dependency injection.
        public ReadmeGenerator(OpenAIService aiService, ContentBuilder contentBuilder, GitRepositoryService gitService)
        {
            _aiService = aiService;
            _contentBuilder = contentBuilder;
            _gitService = gitService;
        }

        public async Task GenerateReadMe(ProjectInfo projectInfo, string projectName, string projectSummary, string usageInstructions, string template = null)
        {
            // Determine the project root using the Git service.
            var rootPath = _gitService.DetectProjectRoot(projectInfo.ProjectPath);
            var outputPath = Path.Combine(rootPath, "README.md");
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            using (var writer = new StreamWriter(outputPath))
            {
                string finalContent;
                if (!string.IsNullOrEmpty(template))
                {
                    // Build manual content and then enhance it using the AI service.
                    var readmeContent = _contentBuilder.BuildManualContent(projectInfo, projectName, projectSummary, usageInstructions);
                    finalContent = await _aiService.EnhanceDocumentationAsync(template, readmeContent);
                }
                else
                {
                    // Build structured content and generate documentation from it.
                    var structuredContent = _contentBuilder.BuildStructuredContent(projectInfo);
                    finalContent = await _aiService.GenerateDocumentationAsync(
                        projectName,
                        projectSummary,
                        usageInstructions,
                        structuredContent,
                        projectInfo.HasWebApi, projectInfo.License);
                }
                await writer.WriteLineAsync(finalContent);
            }
        }
    }
} 