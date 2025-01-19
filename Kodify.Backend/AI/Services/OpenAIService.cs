using Kodify.AI.Models;
using Kodify.AI.Configuration;
using Kodify.AI.Constants;
using OpenAI.Chat;
namespace Kodify.AI.Services;

public class OpenAIService : IOpenAIService
{
    private readonly ChatClient _client;
    private readonly string _model;

    public OpenAIService(OpenAIConfig config)
    {
        _client = new ChatClient(model: config.Model, apiKey: config.ApiKey);
        _model = config.Model;
    }

    public async IAsyncEnumerable<CodeSolution> GenerateSolutionsAsync(
        string description,
        string input,
        string output,
        string language,
        string culture,
        int solutionCount = 3)
    {
        var previousCodes = new List<string>();
        var languageId = LanguageMapping.GetLanguageIdentifier(language);

        for (int i = 0; i < solutionCount; i++)
        {
            var solution = await GenerateSingleSolution(
                description,
                input,
                output,
                language,
                culture,
                previousCodes);

            previousCodes.Add(solution.Code);
            yield return solution;
        }
    }

    private async Task<CodeSolution> GenerateSingleSolution(
        string description,
        string input,
        string output,
        string language,
        string culture,
        List<string> previousSolutions)
    {
        // Generate code
        var codePrompt = PromptTemplates.GetSolutionPrompt(language, description, input, output, previousSolutions);
        ChatCompletion codeCompletion = await _client.CompleteChatAsync(codePrompt);
        var code = codeCompletion.Content[0].Text;

        // Generate explanation
        var explanationPrompt = PromptTemplates.GetExplanationPrompt(code, culture);
        ChatCompletion explanationCompletion = await _client.CompleteChatAsync(explanationPrompt);
        var explanation = explanationCompletion.Content[0].Text;

        // Generate accuracy analysis
        var analysisPrompt = PromptTemplates.GetAnalysisPrompt(description, input, output, code);
        ChatCompletion analysisCompletion = await _client.CompleteChatAsync(analysisPrompt);
        var accuracy = analysisCompletion.Content[0].Text;

        return new CodeSolution
        {
            Code = code,
            Explanation = explanation,
            Accuracy = accuracy,
            Language = language
        };
    }
} 