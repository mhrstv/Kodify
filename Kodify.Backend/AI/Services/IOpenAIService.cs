using Kodify.AI.Models;
namespace Kodify.AI.Services;

public interface IOpenAIService
{
    Task<List<CodeSolution>> GenerateSolutions(
        string description, 
        string input, 
        string output, 
        string language,
        string culture,
        int solutionCount = 3);
} 