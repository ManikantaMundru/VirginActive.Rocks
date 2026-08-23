# VirginActive Rocks API

A .NET 10 Web API for managing weekly member commitments ("Rocks") and enriching member information using an external profile API.

The solution focuses on API design, validation, resilience, observability, security, and maintainability.

## Technology

- .NET 10
- C# 14
- ASP.NET Core Web API
- xUnit
- Serilog
- Microsoft.Extensions.Http.Resilience / Polly
- In-memory persistence
- Swagger / OpenAPI

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