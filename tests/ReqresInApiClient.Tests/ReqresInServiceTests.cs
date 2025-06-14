using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using ReqresInApiClient.Options;
using ReqresInApiClient.Services;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace ReqresInApiClient.Tests
{
    public class ReqresInServiceTests : IDisposable
    {
        private readonly Mock<HttpMessageHandler> _handlerMock;
        private readonly HttpClient _httpClient;
        private readonly ReqresInOptions _options;
        private readonly Mock<IOptions<ReqresInOptions>> _optionsMock;
        private readonly IMemoryCache _realCache;
        private readonly ReqresInService _service;
        private readonly ITestOutputHelper _output;

        public ReqresInServiceTests(ITestOutputHelper output)
        {
            _output = output;

            _options = new ReqresInOptions { BaseUrl = "https://test.api/" };
            _optionsMock = new Mock<IOptions<ReqresInOptions>>();
            _optionsMock.Setup(o => o.Value).Returns(_options);

            _handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            _httpClient = new HttpClient(_handlerMock.Object)
            {
                BaseAddress = new Uri(_options.BaseUrl)
            };

            _realCache = new MemoryCache(new MemoryCacheOptions());

            _service = new ReqresInService(
                _httpClient,
                _optionsMock.Object,
                _realCache);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _realCache.Dispose();
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_WhenFound()
        {
            // Arrange
            var user = new Models.User
            {
                Id = 2,
                First_Name = "Janet",
                Last_Name = "Weaver",
                Email = "janet.weaver@reqres.in",
                Avatar = "https://reqres.in/img/faces/2-image.jpg"
            };
            var response = new Models.SingleUserResponse<Models.User> { Data = user };

            SetupResponse(HttpStatusCode.OK, response, "api/users/1");

            _output.WriteLine("Starting GetUserByIdAsync test...");

            // Act
            var result = await _service.GetUserByIdAsync(1);

            // Assert
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.First_Name, result.First_Name);
            Assert.Equal(user.Last_Name, result.Last_Name);
            _output.WriteLine($"Test completed for user ID: {result.Id}");
        }

        private void SetupResponse(HttpStatusCode statusCode, object content = null, string requestUri = null)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = content == null
                    ? null
                    : new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json")
            };

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        requestUri == null || req.RequestUri.ToString().Contains(requestUri)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response)
                .Verifiable();
        }
    }
}
