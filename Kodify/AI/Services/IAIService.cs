using Kodify.AI.Models;
using Kodify.AutoDoc.Models;
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

   Task<string> EnhanceChangelogAsync(string rawChangelog);
} 