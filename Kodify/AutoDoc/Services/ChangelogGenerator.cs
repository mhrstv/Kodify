using System.Text;
using System.Text.RegularExpressions;
using LibGit2Sharp;
using Kodify.AI.Services;
using Kodify.Repository.Services;

namespace Kodify.AutoDoc.Services
{
    public class ChangelogGenerator
    {
        private readonly IGitRepositoryService _gitService;
        private string _githubRepoUrl; // Normalized URL of the GitHub repo (if available)

        public ChangelogGenerator() : this(new GitRepositoryService())
        {
        }

        public ChangelogGenerator(IGitRepositoryService gitService)
        {
            _gitService = gitService;
        }

        // Generate changelog by automatically detecting the project path.
        public void GenerateChangelog()
        {
            var projectPath = _gitService.DetectProjectRoot();
            GenerateChangelog(projectPath);
        }

        // Generate changelog by providing an input path
        public void GenerateChangelog(string projectPath)
        {
            var rootPath = _gitService.DetectProjectRoot(projectPath);
            var outputPath = Path.Combine(rootPath, "CHANGELOG.md");
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            // Determine if the repository is hosted on GitHub.
            var (hasGit, remoteUrl) = _gitService.CheckForGitRepository(projectPath);
            if (hasGit && !string.IsNullOrEmpty(remoteUrl) && remoteUrl.Contains("github.com"))
            {
                // Store the normalized GitHub URL for linking issue references.
                _githubRepoUrl = remoteUrl;
            }
            else
            {
                _githubRepoUrl = null;
            }

            using (var writer = new StreamWriter(outputPath))
            {
                writer.WriteLine("# Changelog");
                writer.WriteLine();

                var repoPath = LibGit2Sharp.Repository.Discover(projectPath);
                if (repoPath == null)
                {
                    writer.WriteLine("No Git repository found.");
                    return;
                }

                using (var repo = new LibGit2Sharp.Repository(repoPath))
                {
                    // Get tags ordered by commit date.
                    var tags = repo.Tags
                        .Where(t => t.IsAnnotated)
                        .OrderByDescending(t => ((Commit)t.Target).Committer.When)
                        .ToList();

                    if (!tags.Any())
                    {
                        writer.WriteLine("## Unreleased");
                        foreach (var commit in repo.Commits)
                        {
                            WriteCommitDetails(writer, commit);
                        }
                        return;
                    }

                    foreach (var tag in tags)
                    {
                        var commit = (Commit)tag.Target;
                        writer.WriteLine($"## {tag.FriendlyName} ({commit.Committer.When:yyyy-MM-dd})");
                        writer.WriteLine();

                        var previousTag = tags.ElementAtOrDefault(tags.IndexOf(tag) + 1);
                        var filter = new CommitFilter
                        {
                            IncludeReachableFrom = commit,
                            ExcludeReachableFrom = previousTag?.Target
                        };

                        var commitsInRange = repo.Commits.QueryBy(filter).ToList();
                        if (!commitsInRange.Any())
                        {
                            writer.WriteLine("- No changes recorded");
                        }
                        else
                        {
                            foreach (var c in commitsInRange)
                            {
                                WriteCommitDetails(writer, c);
                            }
                        }
                        writer.WriteLine();
                    }

                    writer.WriteLine("## Unreleased");
                    writer.WriteLine();
                    var lastTag = tags.Last().Target;
                    var unreleasedFilter = new CommitFilter
                    {
                        IncludeReachableFrom = repo.Head.Tip,
                        ExcludeReachableFrom = lastTag
                    };

                    foreach (var c in repo.Commits.QueryBy(unreleasedFilter))
                    {
                        WriteCommitDetails(writer, c);
                    }
                }
            }
        }

        public async Task GenerateChangelogAsync(IAIService aiService)
        {
            // Determine the project root.
            var projectPath = _gitService.DetectProjectRoot();
            var rootPath = _gitService.DetectProjectRoot(projectPath);
            var outputPath = Path.Combine(rootPath, "CHANGELOG.md");
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            // Determine if the repository is hosted on GitHub.
            var (hasGit, remoteUrl) = _gitService.CheckForGitRepository(projectPath);
            if (hasGit && !string.IsNullOrEmpty(remoteUrl) && remoteUrl.Contains("github.com"))
                _githubRepoUrl = remoteUrl;
            else
                _githubRepoUrl = null;

            var rawSb = new StringBuilder();
            using (var writer = new StringWriter(rawSb))
            {
                writer.WriteLine("# Changelog");
                writer.WriteLine();

                var repoPath = LibGit2Sharp.Repository.Discover(projectPath);
                if (repoPath == null)
                {
                    writer.WriteLine("No Git repository found.");
                }
                else
                {
                    using (var repo = new LibGit2Sharp.Repository(repoPath))
                    {
                        var tags = repo.Tags
                            .Where(t => t.IsAnnotated)
                            .OrderByDescending(t => ((Commit)t.Target).Committer.When)
                            .ToList();

                        if (!tags.Any())
                        {
                            writer.WriteLine("## Unreleased");
                            foreach (var commit in repo.Commits)
                            {
                                WriteCommitDetails(writer, commit);
                            }
                        }
                        else
                        {
                            foreach (var tag in tags)
                            {
                                var commit = (Commit)tag.Target;
                                writer.WriteLine($"## {tag.FriendlyName} ({commit.Committer.When:yyyy-MM-dd})");
                                writer.WriteLine();

                                var previousTag = tags.ElementAtOrDefault(tags.IndexOf(tag) + 1);
                                var filter = new CommitFilter
                                {
                                    IncludeReachableFrom = commit,
                                    ExcludeReachableFrom = previousTag?.Target
                                };

                                var commitsInRange = repo.Commits.QueryBy(filter).ToList();
                                if (!commitsInRange.Any())
                                {
                                    writer.WriteLine("- No changes recorded");
                                }
                                else
                                {
                                    foreach (var c in commitsInRange)
                                    {
                                        WriteCommitDetails(writer, c);
                                    }
                                }
                                writer.WriteLine();
                            }
                            writer.WriteLine("## Unreleased");
                            writer.WriteLine();
                            var lastTag = tags.Last().Target;
                            var unreleasedFilter = new CommitFilter
                            {
                                IncludeReachableFrom = repo.Head.Tip,
                                ExcludeReachableFrom = lastTag
                            };

                            foreach (var c in repo.Commits.QueryBy(unreleasedFilter))
                            {
                                WriteCommitDetails(writer, c);
                            }
                        }
                    }
                }
            }

            // Enhance the changelog using the dedicated AI method for changelog enhancement.
            string rawChangelog = rawSb.ToString();
            string enhancedChangelog = await aiService.EnhanceChangelogAsync(rawChangelog);

            // Write the enhanced changelog to the output file.
            File.WriteAllText(outputPath, enhancedChangelog);
        }

        private void WriteCommitDetails(TextWriter writer, Commit commit)
        {
            var messageParts = commit.Message.Split(new[] { '\n' }, 2);
            var subject = ReplaceIssueReferences(messageParts[0].Trim());
            var body = messageParts.Length > 1 ? ReplaceIssueReferences(messageParts[1].Trim()) : null;
            
            // Write the subject line with a forced markdown line break (using two trailing spaces)
            writer.WriteLine($"- **{subject}**  ");
            
            // Write the commit details (shortened SHA, date, author) with a forced line break
            writer.WriteLine($"  *Commit: {commit.Sha.Substring(0, 7)} | Date: {commit.Committer.When:yyyy-MM-dd} | Author: {commit.Author.Name}*  ");
            
            // If there is extra commit message body, join the lines and output it on a new line
            if (!string.IsNullOrWhiteSpace(body))
            {
                 var shortBody = string.Join(" ", 
                     body.Split('\n')
                         .Select(line => line.Trim())
                         .Where(line => !string.IsNullOrWhiteSpace(line))
                 );
                 writer.WriteLine($"{shortBody}");
            }
            
            // A blank line to separate commit items
            writer.WriteLine();
        }

        private string ReplaceIssueReferences(string text)
        {
            if (string.IsNullOrEmpty(_githubRepoUrl))
                return text;

            return Regex.Replace(text, "#(\\d+)", m =>
            {
                var issueNumber = m.Groups[1].Value;
                return $"[#{issueNumber}]({_githubRepoUrl}/issues/{issueNumber})";
            });
        }
    }
} 