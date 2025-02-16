using System;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace Kodify.AutoDoc.Services.Repository
{
    public class GitRepositoryService
    {
        public string DetectProjectRoot(string startPath = null)
        {
            var directory = new DirectoryInfo(startPath ?? Directory.GetCurrentDirectory());
            DirectoryInfo trueRoot = null;

            // First check for Git repository root.
            try
            {
                var repoPath = LibGit2Sharp.Repository.Discover(directory.FullName);
                if (!string.IsNullOrEmpty(repoPath))
                {
                    using var repo = new LibGit2Sharp.Repository(repoPath);
                    trueRoot = new DirectoryInfo(repo.Info.WorkingDirectory);
                }
            }
            catch { }

            // Fallback to finding solution/project files.
            if (trueRoot == null)
            {
                var currentDir = directory;
                while (currentDir != null)
                {
                    if (currentDir.GetFiles().Any(f => f.Extension == ".sln" || f.Extension == ".csproj"))
                    {
                        trueRoot = currentDir;
                        break;
                    }
                    currentDir = currentDir.Parent;
                }
            }

            return trueRoot?.FullName ?? throw new DirectoryNotFoundException("Project root not found");
        }

        public (bool HasGit, string Url) CheckForGitRepository(string projectPath)
        {
            try
            {
                var repoPath = LibGit2Sharp.Repository.Discover(projectPath);
                if (string.IsNullOrEmpty(repoPath)) return (false, null);

                using var repo = new LibGit2Sharp.Repository(repoPath);
                var remote = repo.Network.Remotes.FirstOrDefault(r => r.Name == "origin");
                if (remote == null) return (true, null);
                var url = NormalizeGitUrl(remote.Url);
                return (true, url);
            }
            catch
            {
                return (false, null);
            }
        }

        public string NormalizeGitUrl(string gitUrl)
        {
            if (gitUrl.StartsWith("git@"))
            {
                return gitUrl
                    .Replace("git@github.com:", "https://github.com/")
                    .Replace("git@gitlab.com:", "https://gitlab.com/")
                    .Replace(".git", "");
            }
            
            if (gitUrl.EndsWith(".git"))
            {
                gitUrl = gitUrl.Substring(0, gitUrl.Length - 4);
            }
            return gitUrl;
        }
    }
} 