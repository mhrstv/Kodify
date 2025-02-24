using FluentAssertions;
using Kodify.AI.Configuration;
using Kodify.AI.Services;
using Kodify.Repository.Models;
using Moq;
using OpenAI.Chat;
using Xunit;

namespace Kodify.Tests.AI.Services
{
    public class OpenAIServiceTests
    {
        private readonly OpenAIConfig _config;
        private readonly OpenAIService _service;

        public OpenAIServiceTests()
        {
            _config = new OpenAIConfig
            {
                ApiKey = "test-key",
                Model = "test-model"
            };
            _service = new OpenAIService(_config);
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
            var result = await _service.GenerateDocumentationAsync(
                projectName,
                projectSummary,
                usageInstructions,
                structuredContent,
                hasApi,
                license);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task EnhanceDocumentationAsync_ShouldReturnEnhancedContent()
        {
            // Arrange
            var template = "# Template";
            var sections = new List<string> { "Section 1", "Section 2" };

            // Act
            var result = await _service.EnhanceDocumentationAsync(template, sections);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotContain("```markdown");
            result.Should().NotContain("```");
        }

        [Fact]
        public async Task EnhanceChangelogAsync_ShouldReturnEnhancedChangelog()
        {
            // Arrange
            var rawChangelog = "# Changelog\n## v1.0.0";

            // Act
            var result = await _service.EnhanceChangelogAsync(rawChangelog);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotContain("```markdown");
            result.Should().NotContain("```");
        }
    }
} 