using Kodify.AI.Models;
using Kodify.Repository.Models;
namespace Kodify.AI.Services;

public interface IAIService
{
    Task<string> GenerateDocumentationAsync(
       string projectName,
       string projectSummary,
       string usageInstructions,
       string structuredContent,
       bool hasApi,
       LicenseInfo license 
   );
   Task<string> EnhanceDocumentationAsync(string template, List<string> readmeSections);
   Task<string> EnhanceChangelogAsync(string rawChangelog);
} 