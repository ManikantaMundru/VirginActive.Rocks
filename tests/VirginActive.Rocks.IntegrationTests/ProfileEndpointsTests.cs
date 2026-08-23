using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using VirginActive.Rocks.Api.Contracts.Requests;
using VirginActive.Rocks.Api.Contracts.Responses;
using VirginActive.Rocks.Domain.Enums;
using VirginActive.Rocks.IntegrationTests.Helpers;

namespace VirginActive.Rocks.IntegrationTests
{
    public sealed class ProfileEndpointsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private const string ApiKeyHeaderName = "X-Api-Key";
        private const string MemberId = "test-member-id";
        private const string ValidTitle = "test rock";

        private readonly HttpClient _client;

        public ProfileEndpointsTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Add(ApiKeyHeaderName, CustomWebApplicationFactory.ApiKey);
        }

        [Fact]
        public async Task GetEnrichedProfile_WhenProfileApiSucceeds_ShouldReturnProfileAndRocks()
        {
            // Arrange
            var createRequest = new CreateRockRequest(ValidTitle, RockCategory.Other, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)), null);

            var createResponse = await _client.PostAsJsonAsync($"/members/{MemberId}/rocks", createRequest);

            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            // Act
            var response = await _client.GetAsync($"/members/{MemberId}/profile/enriched");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<EnrichedProfileResponse>(TestJsonOptions.Default);

            Assert.NotNull(result);
            Assert.Equal(MemberId, result.MemberId);
            Assert.NotNull(result.Profile);
            Assert.True(result.Enrichment.Available);
            Assert.NotEmpty(result.Rocks);
        }
    }
}
