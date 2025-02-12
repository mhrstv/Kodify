using System;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using System;
using System.Threading.Tasks;
using Kodify.AutoDoc.Models;
using Kodify.AI.Services;
using Kodify.AutoDoc.Services;
using Kodify.AutoDoc.Services.Documentation;
using Kodify.AutoDoc.Services.Repository;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using System.Reflection;

namespace Kodify.AutoDoc.Services.Documentation
{
    public class ChangelogGenerator
    {
        private readonly GitRepositoryService _gitService;

        public ChangelogGenerator() : this(new GitRepositoryService())
        {
        }

        public ChangelogGenerator(GitRepositoryService gitService)
        {
            _gitService = gitService;
        }

        // Parameterless GenerateChangelog method that automatically detects the project path.
        public void GenerateChangelog()
        {
            // Automatically detect the project path using the GitRepositoryService.
            var projectPath = _gitService.DetectProjectRoot();
            GenerateChangelog(projectPath);
        }

        // Existing GenerateChangelog method that accepts a projectPath.
        public void GenerateChangelog(string projectPath)
        {
            // Determine root and prepare the output file.
            var rootPath = _gitService.DetectProjectRoot(projectPath);
            var outputPath = Path.Combine(rootPath, "CHANGELOG.md");
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            using (var writer = new StreamWriter(outputPath))
            {
                writer.WriteLine("# Changelog");

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

        private void WriteCommitDetails(StreamWriter writer, Commit commit)
        {
            var messageParts = commit.Message.Split(new[] { '\n' }, 2);
            var subject = messageParts[0].Trim();
            var body = messageParts.Length > 1 ? messageParts[1].Trim() : null;

            writer.WriteLine($"- **{subject}**");
            writer.WriteLine($"  *Commit: {commit.Sha[..7]} | Date: {commit.Committer.When:yyyy-MM-dd} | Author: {commit.Author.Name}*");

            if (!string.IsNullOrWhiteSpace(body))
            {
                writer.WriteLine();
                writer.WriteLine("  ```");
                foreach (var line in body.Split('\n'))
                {
                    writer.WriteLine($"  {line.Trim()}");
                }
                writer.WriteLine("  ```");
            }
            writer.WriteLine();
        }
    }
} 