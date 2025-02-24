namespace Kodify.Repository.Services
{
    public interface IGitRepositoryService
    {
        string DetectProjectRoot(string startPath = null);
        (bool HasGit, string Url) CheckForGitRepository(string projectPath);
        string NormalizeGitUrl(string gitUrl);
        string GetDefaultBranch();
    }
} 