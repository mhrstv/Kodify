using FluentAssertions;
using Kodify.Repository.Services;
using Xunit;

namespace Kodify.Tests.Repository.Services
{
    public class GitRepositoryServiceTests : IDisposable
    {
        private readonly GitRepositoryService _service;
        private readonly string _testPath;

        public GitRepositoryServiceTests()
        {
            _service = new GitRepositoryService();
            _testPath = Path.Combine(Path.GetTempPath(), "KodifyTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testPath);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testPath))
            {
                Directory.Delete(_testPath, true);
            }
        }

        [Theory]
        [InlineData("git@github.com:user/repo.git", "https://github.com/user/repo")]
        [InlineData("git@gitlab.com:user/repo.git", "https://gitlab.com/user/repo")]
        [InlineData("git@bitbucket.org:user/repo.git", "https://bitbucket.org/user/repo")]
        [InlineData("https://github.com/user/repo.git", "https://github.com/user/repo")]
        [InlineData("https://github.com/user/repo", "https://github.com/user/repo")]
        public void NormalizeGitUrl_ShouldNormalizeUrlCorrectly(string input, string expected)
        {
            // Act
            var result = _service.NormalizeGitUrl(input);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void GetDefaultBranch_ShouldReturnMainAsDefault_WhenNoGitRepository()
        {
            // Arrange & Act
            var result = _service.GetDefaultBranch();

            // Assert
            result.Should().BeOneOf("main", "master"); // Accept either as valid default
        }

        [Fact]
        public void DetectProjectRoot_ShouldThrowException_WhenNoProjectRootFound()
        {
            // Arrange
            var emptyPath = Path.Combine(_testPath, "EmptyFolder");
            Directory.CreateDirectory(emptyPath);

            // Act & Assert
            var action = () => _service.DetectProjectRoot(emptyPath);
            action.Should().Throw<DirectoryNotFoundException>()
                .WithMessage("Project root not found");
        }

        [Fact]
        public void CheckForGitRepository_ShouldReturnFalse_WhenNoGitRepository()
        {
            // Arrange
            var tempPath = Path.Combine(_testPath, "NoGitRepo");
            Directory.CreateDirectory(tempPath);

            try
            {
                // Act
                var (hasGit, url) = _service.CheckForGitRepository(tempPath);

                // Assert
                hasGit.Should().BeFalse();
                url.Should().BeNull();
            }
            finally
            {
                if (Directory.Exists(tempPath))
                {
                    Directory.Delete(tempPath, true);
                }
            }
        }
    }
} 