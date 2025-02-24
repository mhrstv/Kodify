using FluentAssertions;
using Kodify.Utilities;
using Xunit;

namespace Kodify.Tests.Utilities
{
    public class FileHelperTests : IDisposable
    {
        private readonly string _testDirectory;

        public FileHelperTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "KodifyTests", Guid.NewGuid().ToString());
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [Fact]
        public void EnsureDirectoryExists_ShouldCreateDirectory_WhenItDoesNotExist()
        {
            // Arrange
            var path = Path.Combine(_testDirectory, "TestDir");
            Directory.Exists(path).Should().BeFalse();

            // Act
            FileHelper.EnsureDirectoryExists(path);

            // Assert
            Directory.Exists(path).Should().BeTrue();
        }

        [Fact]
        public void EnsureDirectoryExists_ShouldNotThrow_WhenDirectoryAlreadyExists()
        {
            // Arrange
            var path = Path.Combine(_testDirectory, "TestDir");
            Directory.CreateDirectory(path);

            // Act & Assert
            var action = () => FileHelper.EnsureDirectoryExists(path);
            action.Should().NotThrow();
        }
    }
} 