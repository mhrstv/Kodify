using FluentAssertions;
using Kodify.AI.Configuration;
using Kodify.AI.Services;
using Kodify.Repository.Models;
using Moq;
using Xunit;

namespace Kodify.Tests.AI.Services
{
    public class OpenAIServiceTests
    {
        private readonly Mock<IAIService> _aiServiceMock;

        public OpenAIServiceTests()
        {
            _aiServiceMock = new Mock<IAIService>();
            _aiServiceMock.Setup(x => x.GenerateDocumentationAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<LicenseInfo>()))
                .ReturnsAsync("Generated Documentation");

            _aiServiceMock.Setup(x => x.EnhanceDocumentationAsync(
                It.IsAny<string>(),
                It.IsAny<List<string>>()))
                .ReturnsAsync("Enhanced Documentation");

            _aiServiceMock.Setup(x => x.EnhanceChangelogAsync(
                It.IsAny<string>()))
                .ReturnsAsync("Enhanced Changelog");
        }

        [Fact]
        public async Task GenerateDocumentationAsync_ShouldReturnFormattedDocumentation()
        {
            // Arrange
            var projectName = "TestProject";
            var projectSummary = "A test project";
            var usageInstructions = "How to use";
            var structuredContent = "Content";
            var hasApi = true;
            var license = new LicenseInfo { Type = "MIT" };

            // Act
            var result = await _aiServiceMock.Object.GenerateDocumentationAsync(
                projectName,
                projectSummary,
                usageInstructions,
                structuredContent,
                hasApi,
                license);

            // Assert
            result.Should().Be("Generated Documentation");
        }

        [Fact]
        public async Task EnhanceDocumentationAsync_ShouldReturnEnhancedContent()
        {
            // Arrange
            var template = "# Template";
            var sections = new List<string> { "Section 1", "Section 2" };

            // Act
            var result = await _aiServiceMock.Object.EnhanceDocumentationAsync(template, sections);

            // Assert
            result.Should().Be("Enhanced Documentation");
        }

        [Fact]
        public async Task EnhanceChangelogAsync_ShouldReturnEnhancedChangelog()
        {
            // Arrange
            var rawChangelog = "# Changelog\n## v1.0.0";

            // Act
            var result = await _aiServiceMock.Object.EnhanceChangelogAsync(rawChangelog);

            // Assert
            result.Should().Be("Enhanced Changelog");
        }
    }
} 