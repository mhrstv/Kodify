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

    public async IAsyncEnumerable<CodeSolution> GenerateSolutionsAsync(
        string description,
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

    private async Task<CodeSolution> GenerateSingleSolution(
        string description,
        string language,
        string culture,
        List<string> previousSolutions)
    {
        // Generate code
        var codePrompt = PromptTemplates.GetSolutionPrompt(language, description, previousSolutions);
        ChatCompletion codeCompletion = await _client.CompleteChatAsync(codePrompt);
        var code = codeCompletion.Content[0].Text;

        // Generate explanation
        var explanationPrompt = PromptTemplates.GetExplanationPrompt(code, culture);
        ChatCompletion explanationCompletion = await _client.CompleteChatAsync(explanationPrompt);
        var explanation = explanationCompletion.Content[0].Text;

        // Generate accuracy analysis
        var analysisPrompt = PromptTemplates.GetAnalysisPrompt(description, code);
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
    public async IAsyncEnumerable<GeneratedProblem> GenerateProblemsAsync(
        string keywords,
        string difficulty,
        string culture,
        int? examplesCount = null,
        int problemCount = 3)
    {
        var previousProblems = new List<string>();

        for (int i = 0; i < problemCount; i++)
        {
            var problem = await GenerateSingleProblem(
                keywords,
                difficulty,
                culture,
                examplesCount,
                previousProblems);

            previousProblems.Add(problem.Description);
            yield return problem;
        }
    }

    private async Task<GeneratedProblem> GenerateSingleProblem(
        string keywords,
        string difficulty,
        string culture,
        int? examplesCount,
        List<string> previousProblems)
    {
        var prompt = PromptTemplates.GetProblemGenerationPrompt(
            keywords,
            difficulty,
            examplesCount,
            culture,
            previousProblems);

        ChatCompletion completion = await _client.CompleteChatAsync(prompt);
        var description = completion.Content[0].Text;

        return new GeneratedProblem
        {
            Description = description,
            Keywords = keywords,
            Difficulty = difficulty
        };
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