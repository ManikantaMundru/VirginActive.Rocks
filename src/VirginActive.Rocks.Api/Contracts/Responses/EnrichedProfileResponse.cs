namespace VirginActive.Rocks.Api.Contracts.Responses
{
    public sealed record EnrichedProfileResponse(
      string MemberId,
      ProfileResponse? Profile,
      IReadOnlyCollection<RockResponse> Rocks,
      EnrichmentResponse Enrichment);
}
