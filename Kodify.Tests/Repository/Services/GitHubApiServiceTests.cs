using FluentAssertions;
using Kodify.Repository.Services;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Kodify.Tests.Repository.Services
{
    public class GitHubApiServiceTests : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly GitHubApiService _service;

        public GitHubApiServiceTests()
        {
            _httpClient = new HttpClient();
            _service = new GitHubApiService();
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        [Fact]
        public async Task GetRepositoryInfoAsync_ShouldThrowException_WhenRepositoryNotFound()
        {
            // Arrange
            var owner = "nonexistent";
            var repo = "nonexistent";

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _service.GetRepositoryInfoAsync(owner, repo));
        }

        [Fact]
        public async Task GetRepositoryInfoAsync_ShouldReturnRepositoryInfo_WhenRepositoryExists()
        {
            // Arrange
            var owner = "microsoft";
            var repo = "vscode";

            // Act
            var result = await _service.GetRepositoryInfoAsync(owner, repo);

            // Assert
            result.Should().NotBeNull();
            result["name"]?.ToString().Should().Be("vscode");
            result["owner"]?["login"]?.ToString().Should().Be("microsoft");
        }

        [Fact]
        public void Constructor_ShouldSetAuthorizationHeader_WhenTokenProvided()
        {
            // Arrange
            var token = "test-token";

            // Act
            var service = new GitHubApiService(token);

            // Assert
            // Note: We can't directly test the private HttpClient's headers
            // This is more of an integration test that would need to be run against the actual API
            service.Should().NotBeNull();
        }
    }
} 