using Kodify.AI.Models;
using Kodify.AI.Configuration;
using Kodify.AI.Constants;
using OpenAI.Chat;
using System.Threading.Tasks;
using Kodify.AutoDoc.Models;
namespace Kodify.AI.Services;

public class OpenAIService : IAIService
{
    private readonly ChatClient _client;
    private readonly string _model;

    public OpenAIService(OpenAIConfig config)
    {
        _client = new ChatClient(model: config.Model, apiKey: config.ApiKey);
        _model = config.Model;
    }

    public async Task<string> GenerateDocumentationAsync(
        string projectName,
        string projectSummary,
        string usageInstructions,
        string structuredContent,
        bool hasApi,
        LicenseInfo license)
    {
        var prompt = PromptTemplates.GetDocumentationPrompt(
            projectName,
            projectSummary,
            usageInstructions,
            structuredContent,
            hasApi,
            license.Type
        );

        ChatCompletion completion = await _client.CompleteChatAsync(prompt);
    return completion.Content[0].Text;
}

    public async Task<string> EnhanceDocumentationAsync(string template, List<string> readmeSections)
    {
        try
        {
            var prompt = PromptTemplates.GetDocumentationEnhancementPrompt(template, readmeSections);
            ChatCompletion completion = await _client.CompleteChatAsync(prompt);
            return ProcessEnhancedDocumentation(completion.Content[0].Text);
        }
        catch (Exception ex)
        {
            // Fallback to original content
            return string.Join("\n", readmeSections);
        }
    }

    private string ProcessEnhancedDocumentation(string content)
    {
        // Post-processing rules
        return content
            .Replace("```markdown", "")
            .Replace("```", "")
            .Trim();
    }
} 