using FluentAssertions;
using Kodify.Repository.Services;
using Xunit;

namespace Kodify.Tests.Repository.Services
{
    public class GitRepositoryServiceTests
    {
        private readonly GitRepositoryService _service;

        public GitRepositoryServiceTests()
        {
            _service = new GitRepositoryService();
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
            result.Should().Be("main");
        }

        [Fact]
        public void DetectProjectRoot_ShouldThrowException_WhenNoProjectRootFound()
        {
            // Arrange
            var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            // Act & Assert
            var action = () => _service.DetectProjectRoot(nonExistentPath);
            action.Should().Throw<DirectoryNotFoundException>()
                .WithMessage("Project root not found");
        }

        [Fact]
        public void CheckForGitRepository_ShouldReturnFalse_WhenNoGitRepository()
        {
            // Arrange
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
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
                Directory.Delete(tempPath, true);
            }
        }
    }
} 