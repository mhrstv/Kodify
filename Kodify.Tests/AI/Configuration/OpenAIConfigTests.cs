using FluentAssertions;
using Kodify.AI.Configuration;
using Xunit;

namespace Kodify.Tests.AI.Configuration
{
    public class OpenAIConfigTests
    {
        [Fact]
        public void Default_ShouldUseEnvironmentVariable_ForApiKey()
        {
            // Arrange
            var expectedApiKey = "test-api-key";
            Environment.SetEnvironmentVariable("OPENAI_API_KEY", expectedApiKey);

            // Act
            var config = OpenAIConfig.Default;

            // Assert
            config.ApiKey.Should().Be(expectedApiKey);
            config.Model.Should().Be("gpt-4o-mini");

            // Cleanup
            Environment.SetEnvironmentVariable("OPENAI_API_KEY", null);
        }

        [Fact]
        public void Default_ShouldUseDefaultApiKey_WhenEnvironmentVariableNotSet()
        {
            // Arrange
            Environment.SetEnvironmentVariable("OPENAI_API_KEY", null);

            // Act
            var config = OpenAIConfig.Default;

            // Assert
            config.ApiKey.Should().Be("api-key");
            config.Model.Should().Be("gpt-4o-mini");
        }

        [Fact]
        public void Constructor_ShouldCreateInstance_WithDefaultModel()
        {
            // Act
            var config = new OpenAIConfig();

            // Assert
            config.Model.Should().Be("gpt-4o-mini");
        }
    }
} 