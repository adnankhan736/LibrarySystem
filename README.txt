Library Solution
================

A modular, production-ready .NET 9 library management system built with clean architecture, gRPC, SQL Server, and InMemory SQL database for testing.

Projects in Solution
--------------------

Library/
├── Library.API                   --> Entry-point Web API
├── Library.API.SystemTests      --> System tests with mocked gRPC clients
├── Library.Application          --> Application layer (services, use cases)
├── Library.Application.IntegrationTests --> Integration tests (InMemory SQL)
├── Library.Application.UnitTests        --> Unit tests for business logic
├── Library.Domain               --> Domain models
├── Library.GrpcContracts        --> gRPC Protos and generated clients
├── Library.Infrastructure       --> SQL database access, EF Core
├── Library.Service              --> gRPC service project
└── Library.Service.FunctionalTests --> Functional tests for gRPC services

Tech Stack
----------

- .NET 9
- ASP.NET Core
- gRPC (Grpc.Net.Client, Grpc.AspNetCore)
- Entity Framework Core
- SQL Server
- InMemory SQL Provider (for Testing)
- xUnit, Moq, TestServer, WebApplicationFactory

Testing Strategy
----------------

- Unit Tests: Isolated business logic
- Integration Tests: Run against InMemory SQL
- System Tests: Full-stack tests with mock gRPC clients
- Functional Tests: gRPC service behavior tests

Running the Solution
--------------------

You can run the solution using Visual Studio for full IDE support.

Or use the terminal from the root folder:

1. Build the solution:
   dotnet build

2. Run the Web API:
   dotnet run --project Library.API

3. Run the gRPC Service:
   dotnet run --project Library.Service

4. Run all tests:
   dotnet test

All tests (unit, integration, system, and functional) can also be run using the Visual Studio Test Explorer.
