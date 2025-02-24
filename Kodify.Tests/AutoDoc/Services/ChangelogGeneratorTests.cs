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
        private readonly Mock<IGitRepositoryService> _gitServiceMock;
        private readonly Mock<IAIService> _aiServiceMock;
        private readonly ChangelogGenerator _generator;
        private readonly string _testPath;

        public ChangelogGeneratorTests()
        {
            _gitServiceMock = new Mock<IGitRepositoryService>();
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
            var expectedPath = _testPath; // Use existing test path
            _gitServiceMock.Setup(x => x.DetectProjectRoot(It.IsAny<string>()))
                .Returns(expectedPath);
            _gitServiceMock.Setup(x => x.CheckForGitRepository(expectedPath))
                .Returns((false, null));

            // Act
            _generator.GenerateChangelog();

            // Assert
            _gitServiceMock.Verify(x => x.DetectProjectRoot(It.IsAny<string>()));
            _gitServiceMock.Verify(x => x.CheckForGitRepository(expectedPath), Times.Once);
        }

        [Fact]
        public async Task GenerateChangelogAsync_ShouldCallAIService()
        {
            // Arrange
            _gitServiceMock.Setup(x => x.DetectProjectRoot(It.IsAny<string>()))
                .Returns(_testPath);
            _gitServiceMock.Setup(x => x.CheckForGitRepository(It.IsAny<string>()))
                .Returns((false, null));
            _aiServiceMock.Setup(x => x.EnhanceChangelogAsync(It.IsAny<string>()))
                .ReturnsAsync("Enhanced changelog");

            // Act
            await _generator.GenerateChangelogAsync(_aiServiceMock.Object);

            // Assert
            _aiServiceMock.Verify(x => x.EnhanceChangelogAsync(It.IsAny<string>()), Times.Once);
            _gitServiceMock.Verify(x => x.CheckForGitRepository(It.IsAny<string>()), Times.Once);
        }
    }
}