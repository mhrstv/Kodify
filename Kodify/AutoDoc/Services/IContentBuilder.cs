using Kodify.Repository.Models;

namespace Kodify.AutoDoc.Services
{
    public interface IContentBuilder
    {
        string BuildStructuredContent(ProjectInfo projectInfo);
        List<string> BuildManualContent(ProjectInfo projectInfo, string projectName, string projectSummary, string usageInstructions);
    }
} 