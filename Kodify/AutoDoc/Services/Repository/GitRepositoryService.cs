using System;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace Kodify.AutoDoc.Repository
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

        public string GetDefaultBranch()
        {
            var projectPath = DetectProjectRoot();
            try
            {
                var repoPath = LibGit2Sharp.Repository.Discover(projectPath);
                if (string.IsNullOrEmpty(repoPath)) return "main"; // default fallback

                using var repo = new LibGit2Sharp.Repository(repoPath);
                
                // Try to get the default branch from the origin remote
                var origin = repo.Network.Remotes["origin"];
                if (origin != null)
                {
                    var originRef = repo.Branches
                        .FirstOrDefault(b => b.IsRemote && b.FriendlyName.StartsWith("origin/HEAD"))
                        ?.FriendlyName;
                        
                    if (originRef != null)
                    {
                        // Convert "origin/HEAD -> origin/main" to "main"
                        var parts = originRef.Split(new[] { " -> " }, StringSplitOptions.None);
                        if (parts.Length > 1)
                        {
                            return parts[1].Replace("origin/", "");
                        }
                    }
                }

                // Fallback: Check common branch names
                var commonBranches = new[] { "main", "master", "develop" };
                foreach (var branchName in commonBranches)
                {
                    if (repo.Branches[branchName] != null)
                    {
                        return branchName;
                    }
                }

                // Final fallback: return the current branch name
                return repo.Head.FriendlyName;
            }
            catch
            {
                return "main"; // default fallback if anything goes wrong
            }
        }
    }
} 