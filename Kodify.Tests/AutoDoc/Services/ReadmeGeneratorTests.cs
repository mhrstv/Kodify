using FluentAssertions;
using Kodify.AI.Services;
using Kodify.AutoDoc.Services;
using Kodify.Repository.Models;
using Kodify.Repository.Services;
using Moq;
using Xunit;

namespace Kodify.Tests.AutoDoc.Services
{
    public class ReadmeGeneratorTests : IDisposable
    {
        private readonly Mock<IAIService> _aiServiceMock;
        private readonly Mock<ContentBuilder> _contentBuilderMock;
        private readonly Mock<GitRepositoryService> _gitServiceMock;
        private readonly ReadmeGenerator _generator;
        private readonly string _testPath;

        public ReadmeGeneratorTests()
        {
            _aiServiceMock = new Mock<IAIService>();
            _contentBuilderMock = new Mock<ContentBuilder>();
            _gitServiceMock = new Mock<GitRepositoryService>();
            _generator = new ReadmeGenerator(_aiServiceMock.Object, _contentBuilderMock.Object, _gitServiceMock.Object);
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

        [Fact]
        public void Constructor_ShouldCreateInstance_WithDefaultDependencies()
        {
            // Act
            var generator = new ReadmeGenerator();

            // Assert
            generator.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_ShouldCreateInstance_WithAIService()
        {
            // Act
            var generator = new ReadmeGenerator(_aiServiceMock.Object);

            // Assert
            generator.Should().NotBeNull();
        }

        [Fact]
        public async Task GenerateReadMeAsync_ShouldUseContentBuilder_WhenNoAIService()
        {
            // Arrange
            var generator = new ReadmeGenerator(null, _contentBuilderMock.Object, _gitServiceMock.Object);
            var projectInfo = new ProjectInfo { ProjectPath = _testPath };
            var structuredContent = "Test content";

            _gitServiceMock.Setup(x => x.DetectProjectRoot(It.IsAny<string>()))
                .Returns(_testPath);
            _contentBuilderMock.Setup(x => x.BuildStructuredContent(It.IsAny<ProjectInfo>()))
                .Returns(structuredContent);

            // Act
            await generator.GenerateReadMeAsync(
                projectInfo,
                "Test Project",
                "Test Summary",
                "Test Usage");

            // Assert
            _contentBuilderMock.Verify(x => x.BuildStructuredContent(It.IsAny<ProjectInfo>()), Times.Once);
        }

        [Fact]
        public async Task GenerateReadMeAsync_ShouldUseAIService_WhenAvailable()
        {
            // Arrange
            var projectInfo = new ProjectInfo { ProjectPath = _testPath };
            var enhancedContent = "Enhanced content";

            _gitServiceMock.Setup(x => x.DetectProjectRoot(It.IsAny<string>()))
                .Returns(_testPath);
            _aiServiceMock.Setup(x => x.GenerateDocumentationAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<LicenseInfo>()))
                .ReturnsAsync(enhancedContent);

            // Act
            await _generator.GenerateReadMeAsync(
                projectInfo,
                "Test Project",
                "Test Summary",
                "Test Usage");

            // Assert
            _aiServiceMock.Verify(x => x.GenerateDocumentationAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<LicenseInfo>()), Times.Once);
        }

        [Fact]
        public async Task GenerateReadMeAsync_ShouldUseTemplate_WhenProvided()
        {
            // Arrange
            var projectInfo = new ProjectInfo { ProjectPath = _testPath };
            var template = "# Template";
            var readmeContent = new List<string> { "Content" };

            _gitServiceMock.Setup(x => x.DetectProjectRoot(It.IsAny<string>()))
                .Returns(_testPath);
            _contentBuilderMock.Setup(x => x.BuildManualContent(
                It.IsAny<ProjectInfo>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .Returns(readmeContent);
            _aiServiceMock.Setup(x => x.EnhanceDocumentationAsync(
                It.IsAny<string>(),
                It.IsAny<List<string>>()))
                .ReturnsAsync("Enhanced content");

            // Act
            await _generator.GenerateReadMeAsync(
                projectInfo,
                "Test Project",
                "Test Summary",
                "Test Usage",
                template);

            // Assert
            _aiServiceMock.Verify(x => x.EnhanceDocumentationAsync(
                template,
                It.IsAny<List<string>>()), Times.Once);
        }
    }
} 