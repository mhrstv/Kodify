using FluentAssertions;
using Kodify.Utilities;
using Xunit;

namespace Kodify.Tests.Utilities
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("hello world", "Hello World")]
        [InlineData("HELLO WORLD", "Hello World")]
        [InlineData("hELLo WoRLD", "Hello World")]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void ToTitleCase_ShouldConvertStringCorrectly(string input, string expected)
        {
            // Act
            var result = input.ToTitleCase();

            // Assert
            result.Should().Be(expected);
        }
    }
} 