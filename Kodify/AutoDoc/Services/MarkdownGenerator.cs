using System;
using System.Collections.Generic;
using System.IO;
using Kodify.AutoDoc.Models;
using Kodify.AI.Services;

namespace Kodify.AutoDoc.Services
{
    public class MarkdownGenerator
    {
        private readonly OpenAIService _aiService;

        public MarkdownGenerator(OpenAIService aiService)
        {
            _aiService = aiService;
        }

        public async Task GenerateMarkdown(List<DocumentationModel> documentation, string outputPath, string projectName, string projectSummary, string usageInstructions)
        {
            try
            {
                // Ensure the output directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                using (var writer = new StreamWriter(outputPath))
                {
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
    }
}