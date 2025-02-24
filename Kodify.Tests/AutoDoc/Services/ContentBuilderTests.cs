using FluentAssertions;
using Kodify.AutoDoc.Services;
using Kodify.Repository.Models;
using Xunit;

namespace Kodify.Tests.AutoDoc.Services
{
    public class ContentBuilderTests
    {
        private readonly ContentBuilder _builder;

        public ContentBuilderTests()
        {
            _builder = new ContentBuilder();
        }

        [Fact]
        public void BuildStructuredContent_ShouldIncludeApiStatus_WhenApiDetected()
        {
            // Arrange
            var projectInfo = new ProjectInfo
            {
                HasWebApi = true,
                License = new LicenseInfo { Type = "MIT" }
            };

            // Act
            var result = _builder.BuildStructuredContent(projectInfo);

            // Assert
            result.Should().Contain("API Detected: Yes");
        }

        [Fact]
        public void BuildStructuredContent_ShouldNotIncludeApiStatus_WhenNoApiDetected()
        {
            // Arrange
            var projectInfo = new ProjectInfo
            {
                HasWebApi = false,
                License = new LicenseInfo { Type = "MIT" }
            };

            // Act
            var result = _builder.BuildStructuredContent(projectInfo);

            // Assert
            result.Should().NotContain("API Detected");
        }

        [Theory]
        [InlineData("MIT")]
        [InlineData("Apache-2.0")]
        [InlineData("None")]
        public void BuildStructuredContent_ShouldIncludeLicenseInfo(string licenseType)
        {
            // Arrange
            var projectInfo = new ProjectInfo
            {
                License = new LicenseInfo { Type = licenseType }
            };

            // Act
            var result = _builder.BuildStructuredContent(projectInfo);

            // Assert
            result.Should().Contain($"License Status: {licenseType}");
        }

        [Fact]
        public void BuildManualContent_ShouldIncludeAllRequiredSections()
        {
            // Arrange
            var projectInfo = new ProjectInfo();
            var projectName = "Test Project";
            var projectSummary = "Test Summary";
            var usageInstructions = "Test Instructions";

            // Act
            var result = _builder.BuildManualContent(projectInfo, projectName, projectSummary, usageInstructions);

            // Assert
            result.Should().Contain("# Test Project");
            result.Should().Contain("Test Summary");
            result.Should().Contain("## Table of Contents");
            result.Should().Contain("## Features");
        }

        [Fact]
        public void BuildManualContent_ShouldDetectFeatures_FromProjectStructure()
        {
            // Arrange
            var projectInfo = new ProjectInfo
            {
                SourceFiles = new List<CodeFile>
                {
                    new CodeFile { FilePath = "Services/TestService.cs" },
                    new CodeFile { FilePath = "Repositories/TestRepo.cs" },
                    new CodeFile { FilePath = "Tests/TestClass.Tests.cs" }
                }
            };

            // Act
            var result = _builder.BuildManualContent(projectInfo, "Test", "Summary", "Usage");

            // Assert
            var content = string.Join("\n", result);
            content.Should().Contain("Service layer architecture");
            content.Should().Contain("Database repository pattern");
            content.Should().Contain("Unit test coverage");
        }
    }
} 