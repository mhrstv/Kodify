using System.Threading.Tasks;
using Kodify.AutoDoc.Models;
using Kodify.AI.Services;
using Kodify.AI.Configuration;
using Kodify.AI;
using Kodify.AI.Models;
using Kodify.AutoDoc.Services.Documentation;
using Kodify.AutoDoc.Services.Repository;

namespace Kodify.AutoDoc.Services.Documentation
{
    public class MarkdownGenerator
    {
        private readonly ReadmeGenerator _readmeGenerator;
        private readonly ChangelogGenerator _changelogGenerator;
        private readonly ProjectAnalyzer _projectAnalyzer;
        private ProjectInfo _projectInfo;

        // Parameterless constructor overload.
        public MarkdownGenerator() : this(null)
        {
            _readmeGenerator = new ReadmeGenerator();
            _changelogGenerator = new ChangelogGenerator();
            _projectAnalyzer = new ProjectAnalyzer();
            _projectInfo = _projectAnalyzer.Analyze();
        }

        // Constructor with AI service implemented
        public MarkdownGenerator(IAIService aiService)
        {
            _readmeGenerator = new ReadmeGenerator(aiService);
            _changelogGenerator = new ChangelogGenerator();
            _projectAnalyzer = new ProjectAnalyzer();
            _projectInfo = _projectAnalyzer.Analyze();
        }

        public async Task GenerateReadMeAsync(string projectName, string projectSummary, string usageInstructions, string template = null)
        {
            await _readmeGenerator.GenerateReadMeAsync(_projectInfo, projectName, projectSummary, usageInstructions, template);
        }

        public async Task GenerateReadMeAsync(ProjectInfo projectInfo, string projectName, string projectSummary, string usageInstructions, string template = null)
        {
            await _readmeGenerator.GenerateReadMeAsync(projectInfo, projectName, projectSummary, usageInstructions, template);
        }

        public void GenerateChangelog()
        {
            _changelogGenerator.GenerateChangelog();
        }

        public void GenerateChangelog(string projectPath)
        {
            _changelogGenerator.GenerateChangelog(projectPath);
        }

        public async Task GenerateChangelogAsync(IAIService aiService)
        {
            await _changelogGenerator.GenerateChangelogAsync(aiService);
        }
    }
}