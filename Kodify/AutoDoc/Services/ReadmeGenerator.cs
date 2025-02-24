using Kodify.AI.Services;
using Kodify.Repository.Services;
using Kodify.Repository.Models;

namespace Kodify.AutoDoc.Services
{
    public class ReadmeGenerator
    {
        private readonly IAIService _aiService;
        private readonly ContentBuilder _contentBuilder;
        private readonly GitRepositoryService _gitService;

        // Parameterless constructor overload.
        public ReadmeGenerator() : this(null, new ContentBuilder(), new GitRepositoryService())
        {
        }

        // Constructor with AI service implemented
        public ReadmeGenerator(IAIService aiService)
            : this(aiService, new ContentBuilder(), new GitRepositoryService())
        {
        }

        // Constructor that allows explicit dependency injection.
        public ReadmeGenerator(IAIService aiService, ContentBuilder contentBuilder, GitRepositoryService gitService)
        {
            _aiService = aiService;
            _contentBuilder = contentBuilder;
            _gitService = gitService;
        }

        public async Task GenerateReadMeAsync(ProjectInfo projectInfo, string projectName, string projectSummary, string usageInstructions, string template = null)
        {
            // Determine the project root using the Git service.
            var rootPath = _gitService.DetectProjectRoot(projectInfo.ProjectPath);
            var outputPath = Path.Combine(rootPath, "README.md");
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            using (var writer = new StreamWriter(outputPath))
            {
                string finalContent;
                if (_aiService == null)
                {
                    // If no AI service is available, then simply use the generated content.
                    if (!string.IsNullOrEmpty(template))
                    {
                        // Build manual content from the content builder
                        var readmeContent = _contentBuilder.BuildManualContent(projectInfo, projectName, projectSummary, usageInstructions);
                        finalContent = string.Join("\n", readmeContent);
                    }
                    else
                    {
                        finalContent = _contentBuilder.BuildStructuredContent(projectInfo);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(template))
                    {
                        // Build manual content and then enhance it using the AI service.
                        var readmeContent = _contentBuilder.BuildManualContent(projectInfo, projectName, projectSummary, usageInstructions);
                        finalContent = await _aiService.EnhanceDocumentationAsync(template, readmeContent);
                    }
                    else
                    {
                        // Build structured content and generate documentation from it via the AI service.
                        var structuredContent = _contentBuilder.BuildStructuredContent(projectInfo);
                        finalContent = await _aiService.GenerateDocumentationAsync(
                            projectName,
                            projectSummary,
                            usageInstructions,
                            structuredContent,
                            projectInfo.HasWebApi, projectInfo.License);
                    }
                }
                await writer.WriteLineAsync(finalContent);
            }
        }
    }
} 