# VirginActive Rocks API

A .NET 10 Web API for managing weekly member commitments ("Rocks") and enriching member information using an external profile API.

The solution focuses on API design, validation, resilience, observability, security, and maintainability.

## Technology and NuGet Packages

- .NET 10
- C# 14
- ASP.NET Core Web API
- xUnit
- Serilog
- Microsoft.Extensions.Http.Resilience / Polly
- In-memory persistence
- Swagger / OpenAPI

NuGet packages used by the solution include:

-   `Serilog.AspNetCore` - ASP.NET Core integration for structured
    logging.
-   `Serilog.Formatting.Compact` - JSON/compact structured console log
    formatting.
-   `Microsoft.Extensions.Http.Resilience` - resilience pipeline for the
    external profile HTTP client.
-   `Swashbuckle.AspNetCore` - Swagger/OpenAPI documentation and manual
    endpoint testing.
-   `xunit` - unit and integration testing.
-   `xunit.runner.visualstudio` - Visual Studio xUnit test runner.
-   `Microsoft.NET.Test.Sdk` - .NET test infrastructure.
-   `Moq` - mocking dependencies in application unit tests.
-   `Microsoft.AspNetCore.Mvc.Testing` - `WebApplicationFactory` support
    for HTTP integration tests.

## Solution Structure

```text
VirginActive.Rocks.Domain
    Entities, enums, domain exceptions and category validation strategies

VirginActive.Rocks.Application
    Application services, abstractions, DTOs and orchestration

VirginActive.Rocks.Infrastructure
    In-memory repository, external profile integration and resilience configuration

VirginActive.Rocks.Api
    Controllers, authentication, exception handling, correlation and API contracts

VirginActive.Rocks.UnitTests
    Domain and application unit tests

VirginActive.Rocks.IntegrationTests
    HTTP endpoint integration tests

Dependencies follow the Clean Architecture principle of keeping business rules independent from infrastructure concerns.

## Build and Run Locally

### Prerequisites

-   .NET 10 SDK

From the solution root:

``` bash
dotnet restore
dotnet build
```

### Configure the local API key

The API key is intentionally not stored in source control. From the API
project directory:

``` bash
dotnet user-secrets init
dotnet user-secrets set "Authentication:ApiKey" "rocks-local-development-key"
```

Run the API:

``` bash
dotnet run --project VirginActive.Rocks.Api
```

Using the HTTPS development profile, Swagger is available at:

``` text
https://localhost:7073/swagger
```

Click **Authorize** in Swagger and enter the configured API key.

## Testing the Endpoints

All API endpoints require:

``` http
X-Api-Key: rocks-local-development-key
```

The following examples assume the local HTTP profile is running on
`http://localhost:5262`.

A Postman collection is included for manual API testing:

`postman/VirginActive.Rocks.PostmanCollection.json`

Import the collection into Postman and configure the collection variables:

- `baseUrl` - defaults to `https://localhost:7073`
- `apiKey` - your locally configured API key
- `memberId` - defaults to `1` for testing only

The Create Rock request automatically stores the returned Rock ID for subsequent status update requests.

### Create a Rock

``` bash
curl -X POST "http://localhost:5262/members/1/rocks" \
  -H "Content-Type: application/json" \
  -H "X-Api-Key: rocks-local-development-key" \
  -d '{
    "title": "Increase monthly membership revenue",
    "category": "Revenue",
    "dueDate": "2026-09-15",
    "note": "Increase revenue by improving member retention"
  }'
```

A successful request returns HTTP `201 Created` with a generated Rock ID
and `Pending` status.

### Get Member Rocks

``` bash
curl "http://localhost:5262/members/1/rocks" \
  -H "X-Api-Key: rocks-local-development-key"
```

Filter by status:

``` bash
curl "http://localhost:5262/members/1/rocks?status=Pending" \
  -H "X-Api-Key: rocks-local-development-key"
```

### Update Rock Status

Replace `{rockId}` with the ID returned by the create request:

``` bash
curl -X PATCH "http://localhost:5262/members/1/rocks/{rockId}" \
  -H "Content-Type: application/json" \
  -H "X-Api-Key: rocks-local-development-key" \
  -d '{
    "status": "Completed"
  }'
```

Valid transitions are:

``` text
Pending -> Completed
Pending -> Missed
```

Any other transition returns HTTP `422 Unprocessable Entity`.

### Get Enriched Member Profile

``` bash
curl "http://localhost:5262/members/1/profile/enriched" \
  -H "X-Api-Key: rocks-local-development-key"
```

Member ID `1` is useful for manual testing because it maps to a
JSONPlaceholder user.

If the external profile API remains unavailable after retries, the
endpoint still returns the member's Rocks and marks enrichment as
unavailable.

## Automated Tests

Run all tests from the solution root:

``` bash
dotnet test
```

The tests focus on high-value behaviours including:

-   Rock state transitions.
-   Category validation strategies.
-   Strategy resolution.
-   Application service orchestration.
-   API-key authentication.
-   HTTP 400, 404 and 422 behaviour.
-   Profile enrichment behaviour.

External profile calls are replaced with test implementations during
integration tests so the automated suite is deterministic and does not
depend on internet availability.

## Design Decisions

### Clean separation of responsibilities

The solution separates Domain, Application, Infrastructure and API
concerns. This keeps business rules independent of HTTP, persistence and
third-party integration details while still keeping the solution small
enough for the assessment.

### Single Rock service

The three Rock operations are exposed through one
`IRockService`/`RockService`. For the current scope they form a cohesive
set of operations and separate handlers for every operation would add
unnecessary ceremony.

If Rock management grew significantly, I would split larger use cases
into dedicated handlers/services rather than allow `RockService` to
become a broad general-purpose service.

### In-memory repository

The repository is intentionally in-memory as required by the assessment.
It is registered as a singleton so data survives across HTTP requests
during the application lifetime. Concurrent access is considered because
multiple requests can access the singleton simultaneously.

In production this would be replaced with durable persistence.

### Centralized error handling

Domain/application exceptions are allowed to propagate to a global
exception handler rather than being caught individually by controllers.
The handler converts them into RFC 7807 Problem Details:

  Scenario                     HTTP Status
  -------------------------- -------------
  Validation failure                   400
  Missing/invalid API key              401
  Rock not found                       404
  Invalid state transition             422
  Unexpected failure                   500

Unexpected errors return a generic message and never expose stack
traces.

### Structured logging and correlation

Serilog is configured at the API host and writes structured JSON logs.

An incoming `X-Correlation-Id` is reused when supplied; otherwise one is
generated. The ID is added to the logging context and returned to the
caller. Request completion logs include status code and duration.

Lower layers use `Microsoft.Extensions.Logging` abstractions rather than
depending directly on Serilog.

### API-key authentication

All endpoints require `X-Api-Key`. The expected key is read from
configuration rather than hardcoded. Local development uses .NET User
Secrets.

A fixed-time comparison is used when comparing API keys.

### External API resilience

The profile integration is registered through `AddHttpClient` as a typed
client.

The resilience configuration includes:

-   Minimum 3 retries.
-   Exponential backoff.
-   Jitter.
-   Transient network/server failure handling.
-   Independently configured HTTP timeout.
-   Structured retry logging with attempt, delay and reason.
-   Graceful degradation when enrichment remains unavailable.

The external API base URL and timeout are configuration-driven.

### Health Check

A lightweight health endpoint is available for deployment and monitoring probes:

`GET /health`

The endpoint does not require API-key authentication.

## Category Validation Strategy - Requirement 3

Category-specific validation uses the Strategy Pattern through
`IRockValidationStrategy`.

Implementations are:

-   `RevenueRockValidationStrategy` - due date must be within the
    current quarter.
-   `HealthRockValidationStrategy` - title must contain at least 10
    characters.
-   `CareerRockValidationStrategy` - a note explaining why the Rock
    matters is required.
-   `OtherRockValidationStrategy` - no additional category rule.

`RockValidationStrategyResolver` selects the appropriate strategy for
the requested category.

This approach was chosen to follow the Open/Closed Principle. Category
rules are not implemented as a growing `switch` statement inside
`RockService`. A new category can be supported by adding a new strategy
and registering it with dependency injection, keeping existing
orchestration code unchanged.

Base validation that applies to every Rock remains separate from
category-specific rules.

For this relatively small validation set I intentionally did not
introduce FluentValidation. If request validation grew substantially,
FluentValidation would be a reasonable alternative for request-level
rules while domain invariants would remain in the domain model.

## Azure Production Architecture - Requirement 8

For this workload I would use **Azure App Service**. It is a good fit
for a conventional stateless HTTP API and provides managed hosting,
scaling, TLS integration, deployment slots and monitoring without
introducing unnecessary container orchestration complexity.

A production request path could be:

``` text
External Client
      |
Azure Front Door
      |
Azure API Management
      |
Azure App Service
      |
External Profile API
```

### Azure Front Door

Front Door would be used when the service needs an internet-facing
global edge, Web Application Firewall, TLS termination, global routing
or improved availability across regions.

### Azure API Management

API Management would provide API-specific capabilities such as client
access policies, throttling, quotas, versioning, transformations and API
governance.

Both services are not automatically required for every deployment. For a
smaller or internal workload, API Management may be sufficient. Front
Door becomes more valuable for globally exposed workloads requiring edge
routing and WAF capabilities.

### Secrets and configuration

Production secrets such as API keys and connection strings would be
stored in **Azure Key Vault**.

The App Service would use **Managed Identity** to access Key Vault,
avoiding credentials in source control or deployment pipelines.

Non-sensitive values such as the external profile API URL and timeout
can be supplied through application/environment configuration.

### Infrastructure and deployment

I would provision Azure resources using **Bicep** and use GitHub Actions
or Azure DevOps for CI/CD.

A production pipeline would broadly perform:

``` text
Restore
-> Build
-> Unit Tests
-> Integration Tests
-> Publish
-> Validate/Provision Infrastructure
-> Deploy
-> Smoke/Health Checks
```

For production releases I would use deployment slots or an equivalent
staged deployment approach to reduce deployment risk.

## What I Would Improve Given More Time

Given more time, I would consider:

-   Database persistence.
-   OpenTelemetry and Azure Application Insights.
-   API versioning.
-   Rate limiting.
-   Additional integration and external API contract tests.
-   Bicep infrastructure definitions and a working CI/CD pipeline.
-   Docker Support

These were intentionally kept outside the assessment scope to prioritize
the requested behaviour, code quality and production-oriented design
decisions.

## AI Usage

AI-assisted development tools were used during this assessment for:

- Assisting with boilerplate and repetitive code, particularly test setup, to reduce time spent on repetitive implementation and allow more focus on the core requirements.
- Structuring and refining the README for clarity and readability, to ensure the implementation and design decisions are communicated clearly. This is purely to fix my english writing skill. 
- I used an existing Clean Architecture project creation script which was generated using AI to generate the initial solution and project structure. This is a personal development utility I use to avoid repetitive project setup. The assessment-specific implementation and configuration were added separately.

