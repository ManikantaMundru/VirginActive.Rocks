using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using VirginActive.Rocks.Api.Contracts.Requests;
using VirginActive.Rocks.Api.Contracts.Responses;
using VirginActive.Rocks.Domain.Enums;
using VirginActive.Rocks.IntegrationTests.Helpers;

namespace VirginActive.Rocks.IntegrationTests
{
    public sealed class RocksEndpointsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private const string ApiKeyHeaderName = "X-Api-Key";
        private const string ValidTitle = "test rock";
        private const string DefaultMemberId = "test-member";

        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public RocksEndpointsTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Add(ApiKeyHeaderName, CustomWebApplicationFactory.ApiKey);
        }

        [Fact]
        public async Task CreateRock_WhenRequestIsValid_ShouldReturnCreated()
        {
            // Arrange
            var memberId = Guid.NewGuid().ToString();
            var request = new CreateRockRequest(ValidTitle, RockCategory.Other, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)), null);

            // Act
            var response = await _client.PostAsJsonAsync($"/members/{memberId}/rocks", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var rock = await response.Content.ReadFromJsonAsync<RockResponse>(TestJsonOptions.Default);

            Assert.NotNull(rock);
            Assert.Equal(memberId, rock.MemberId);
            Assert.Equal(request.Title, rock.Title);
            Assert.Equal(RockStatus.Pending, rock.Status);
        }

        [Fact]
        public async Task CreateRock_WhenTitleIsEmpty_ShouldReturnBadRequest()
        {
            // Arrange
            var memberId = Guid.NewGuid().ToString();
            var request = new CreateRockRequest(" ", RockCategory.Other, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)), null);

            // Act
            var response = await _client.PostAsJsonAsync($"/members/{memberId}/rocks", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetRocks_WhenApiKeyIsMissing_ShouldReturnUnauthorized()
        {
            // Arrange
            using var clientWithoutApiKey = _factory.CreateClient();

            // Act
            var response = await clientWithoutApiKey.GetAsync($"/members/{DefaultMemberId}/rocks");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateRockStatus_WhenRockDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var memberId = Guid.NewGuid().ToString();
            var rockId = Guid.NewGuid();
            var request = new UpdateRockStatusRequest(RockStatus.Completed);

            // Act
            var response = await _client.PatchAsJsonAsync($"/members/{memberId}/rocks/{rockId}", request);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateRockStatus_WhenTransitionIsInvalid_ShouldReturnUnprocessableEntity()
        {
            // Arrange
            var memberId = Guid.NewGuid().ToString();
            var createRequest = new CreateRockRequest(ValidTitle, RockCategory.Other, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)), null);

            var createResponse = await _client.PostAsJsonAsync($"/members/{memberId}/rocks", createRequest);

            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var rock = await createResponse.Content.ReadFromJsonAsync<RockResponse>(TestJsonOptions.Default);

            Assert.NotNull(rock);

            // Move the Rock from Pending to Completed first, so we can test the invalid Completed → Missed transition.
            var completeRequest = new UpdateRockStatusRequest(RockStatus.Completed);
            var completeResponse = await _client.PatchAsJsonAsync($"/members/{memberId}/rocks/{rock.Id}", completeRequest);

            Assert.Equal(HttpStatusCode.OK, completeResponse.StatusCode);

            var missedRequest = new UpdateRockStatusRequest(RockStatus.Missed);

            // Act
            var response = await _client.PatchAsJsonAsync($"/members/{memberId}/rocks/{rock.Id}", missedRequest);

            // Assert
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }
    }
}
