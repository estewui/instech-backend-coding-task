using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

using API.Contracts.Requests;
using API.Contracts.Responses;
using API.Contracts.Types;

namespace Claims.Tests.Integration
{
    /// <summary>
    /// Integration tests that exercise the full HTTP pipeline end-to-end:
    /// routing → controllers → services → validation → repositories → real databases (via Testcontainers).
    /// </summary>
    public class CoverAndClaimLifecycleTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        private readonly HttpClient _client;

        public CoverAndClaimLifecycleTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        /// <summary>
        /// Full happy-path lifecycle: create a cover, file a claim against it,
        /// retrieve the claim, delete it, and confirm it's gone.
        /// </summary>
        [Fact]
        public async Task CreateCover_CreateClaim_GetClaim_DeleteClaim_ReturnsNotFound()
        {
            // 1. Create a cover
            var coverRequest = new CreateCoverRequest
            {
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(60),
                Type = CoverType.Yacht
            };

            var coverResponse = await _client.PostAsJsonAsync("/Covers", coverRequest, JsonOptions, CancellationToken.None);
            coverResponse.EnsureSuccessStatusCode();
            var cover = await coverResponse.Content.ReadFromJsonAsync<CoverResponse>(JsonOptions, CancellationToken.None);

            Assert.NotNull(cover);
            Assert.False(string.IsNullOrEmpty(cover.Id));
            Assert.True(cover.Premium > 0, "Premium should be computed by the service.");
            Assert.Equal(CoverType.Yacht, cover.Type);

            // 2. Create a claim against that cover
            var claimRequest = new CreateClaimRequest
            {
                CoverId = cover.Id,
                Created = DateTime.UtcNow.AddDays(2), // within cover period
                Name = "Integration Test Claim",
                Type = ClaimType.Collision,
                DamageCost = 5000m
            };

            var claimCreateResponse = await _client.PostAsJsonAsync("/Claims", claimRequest, JsonOptions, CancellationToken.None);
            claimCreateResponse.EnsureSuccessStatusCode();
            var createdClaim = await claimCreateResponse.Content.ReadFromJsonAsync<ClaimResponse>(JsonOptions, CancellationToken.None);

            Assert.NotNull(createdClaim);
            Assert.False(string.IsNullOrEmpty(createdClaim.Id));
            Assert.Equal("Integration Test Claim", createdClaim.Name);
            Assert.Equal(cover.Id, createdClaim.CoverId);
            Assert.Equal(ClaimType.Collision, createdClaim.Type);
            Assert.Equal(5000m, createdClaim.DamageCost);

            // 3. Retrieve the claim by ID
            var getResponse = await _client.GetAsync($"/Claims/{createdClaim.Id}", CancellationToken.None);
            getResponse.EnsureSuccessStatusCode();
            var fetchedClaim = await getResponse.Content.ReadFromJsonAsync<ClaimResponse>(JsonOptions, CancellationToken.None);

            Assert.NotNull(fetchedClaim);
            Assert.Equal(createdClaim.Id, fetchedClaim.Id);
            Assert.Equal("Integration Test Claim", fetchedClaim.Name);

            // 4. Delete the claim
            var deleteResponse = await _client.DeleteAsync($"/Claims/{createdClaim.Id}", CancellationToken.None);
            deleteResponse.EnsureSuccessStatusCode();

            // 5. Verify it's gone
            var getAfterDelete = await _client.GetAsync($"/Claims/{createdClaim.Id}", CancellationToken.None);
            Assert.Equal(HttpStatusCode.NotFound, getAfterDelete.StatusCode);
        }

        /// <summary>
        /// Verifies that creating a claim with a date outside the cover period
        /// is rejected by the service-layer business rule.
        /// </summary>
        [Fact]
        public async Task CreateClaim_WithDateOutsideCoverPeriod_ReturnsBadRequest()
        {
            // 1. Create a cover
            var coverRequest = new CreateCoverRequest
            {
                StartDate = DateTime.UtcNow.AddDays(10),
                EndDate = DateTime.UtcNow.AddDays(40),
                Type = CoverType.PassengerShip
            };

            var coverResponse = await _client.PostAsJsonAsync("/Covers", coverRequest, JsonOptions, CancellationToken.None);
            coverResponse.EnsureSuccessStatusCode();
            var cover = await coverResponse.Content.ReadFromJsonAsync<CoverResponse>(JsonOptions, CancellationToken.None);
            Assert.NotNull(cover);

            // 2. Create a claim with a date BEFORE the cover start — should be rejected
            var claimRequest = new CreateClaimRequest
            {
                CoverId = cover.Id,
                Created = DateTime.UtcNow.AddDays(1), // before cover starts at +10 days
                Name = "Should Fail",
                Type = ClaimType.Fire,
                DamageCost = 1000m
            };

            var claimResponse = await _client.PostAsJsonAsync("/Claims", claimRequest, JsonOptions, CancellationToken.None);
            Assert.Equal(HttpStatusCode.BadRequest, claimResponse.StatusCode);
        }
    }
}
