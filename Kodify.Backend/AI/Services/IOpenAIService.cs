using Kodify.AI.Models;
namespace Kodify.AI.Services;

public interface IOpenAIService
{
    IAsyncEnumerable<CodeSolution> GenerateSolutionsAsync(
        string description,
        string input,
        string output,
        string language,
        string culture,
        int solutionCount = 3);
} 