using System.Threading.Tasks;
using Kodify.AutoDoc.Models;
using Kodify.AI.Services;
using Kodify.AutoDoc.Services.Documentation;
using Kodify.AutoDoc.Services.Repository;

namespace Kodify.AutoDoc.Services.Documentation
{
    public class MarkdownGenerator
    {
        private readonly ReadmeGenerator _readmeGenerator;
        private readonly ChangelogGenerator _changelogGenerator;

        public MarkdownGenerator(OpenAIService aiService)
        {
            var contentBuilder = new ContentBuilder();
            var gitRepositoryService = new GitRepositoryService();

            _readmeGenerator = new ReadmeGenerator(aiService, contentBuilder, gitRepositoryService);
            _changelogGenerator = new ChangelogGenerator(gitRepositoryService);
        }

        public async Task GenerateReadMe(ProjectInfo projectInfo, string projectName, string projectSummary, string usageInstructions, string template = null)
        {
            await _readmeGenerator.GenerateReadMe(projectInfo, projectName, projectSummary, usageInstructions, template);
        }

        public void GenerateChangelog()
        {
            _changelogGenerator.GenerateChangelog();
        }

        public void GenerateChangelog(string projectPath)
        {
            _changelogGenerator.GenerateChangelog(projectPath);
        }
    }
}