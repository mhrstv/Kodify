using FluentAssertions;
using Kodify.AI.Services;
using Kodify.AutoDoc.Services;
using Kodify.Repository.Services;
using Moq;
using Xunit;

namespace Kodify.Tests.AutoDoc.Services
{
    public class ChangelogGeneratorTests
    {
        private readonly Mock<GitRepositoryService> _gitServiceMock;
        private readonly Mock<IAIService> _aiServiceMock;
        private readonly ChangelogGenerator _generator;
        private readonly string _testPath;

        public ChangelogGeneratorTests()
        {
            _gitServiceMock = new Mock<GitRepositoryService>();
            _aiServiceMock = new Mock<IAIService>();
            _generator = new ChangelogGenerator(_gitServiceMock.Object);
            _testPath = Path.Combine(Path.GetTempPath(), "KodifyTests", Guid.NewGuid().ToString());
        }

        [Fact]
        public void Constructor_ShouldCreateInstance_WithDefaultGitService()
        {
            // Act
            var generator = new ChangelogGenerator();

            // Assert
            generator.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_ShouldCreateInstance_WithProvidedGitService()
        {
            // Act
            var generator = new ChangelogGenerator(_gitServiceMock.Object);

            // Assert
            generator.Should().NotBeNull();
        }

        [Fact]
        public void GenerateChangelog_ShouldUseDetectedProjectRoot()
        {
            // Arrange
            var expectedPath = Path.Combine(_testPath, "project");
            _gitServiceMock.Setup(x => x.DetectProjectRoot())
                .Returns(expectedPath);

            // Act & Assert
            var action = () => _generator.GenerateChangelog();
            action.Should().NotThrow();

            _gitServiceMock.Verify(x => x.DetectProjectRoot(), Times.Once);
        }

        [Fact]
        public async Task GenerateChangelogAsync_ShouldCallAIService()
        {
            // Arrange
            _gitServiceMock.Setup(x => x.DetectProjectRoot())
                .Returns(_testPath);
            _gitServiceMock.Setup(x => x.CheckForGitRepository(It.IsAny<string>()))
                .Returns((false, null));
            _aiServiceMock.Setup(x => x.EnhanceChangelogAsync(It.IsAny<string>()))
                .ReturnsAsync("Enhanced changelog");

            // Act
            await _generator.GenerateChangelogAsync(_aiServiceMock.Object);

            // Assert
            _aiServiceMock.Verify(x => x.EnhanceChangelogAsync(It.IsAny<string>()), Times.Once);
        }
    }
} 