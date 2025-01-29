using Kodify.AI.Models;
using Kodify.AutoDoc.Models;
namespace Kodify.AI.Services;

public interface IAIService
{
    IAsyncEnumerable<CodeSolution> GenerateSolutionsAsync(
        string description,
        string input,
        string output,
        string language,
        string culture,
        int solutionCount = 3);

    IAsyncEnumerable<GeneratedProblem> GenerateProblemsAsync(
        string keywords,
        string difficulty,
        string culture,
        int? examplesCount = null,
        int problemCount = 3);

    Task<string> GenerateDocumentationAsync(
       string projectName,
       string projectSummary,
       string usageInstructions,
       string structuredContent,
       bool hasApi,
       LicenseInfo license 
   );
} 