# 📚 Library Solution

A modular, production-ready .NET 9 library management system built with clean architecture, gRPC, SQL Server, and InMemory SQL database for testing.

---

## 🧱 Projects in Solution

Library/
├── Library.API                     --> Entry-point Web API
├── Library.API.SystemTests         --> System tests with mocked gRPC clients
├── Library.Application             --> Application layer (services, use cases)
├── Library.Application.IntegrationTests --> Integration tests (InMemory SQL)
├── Library.Application.UnitTests   --> Unit tests for business logic
├── Library.Domain                  --> Domain models and core entities
├── Library.GrpcContracts           --> gRPC Protos and generated clients
├── Library.Infrastructure          --> Data access (EF Core, SQL Server)
├── Library.Service                 --> gRPC service implementation
└── Library.Service.FunctionalTests --> Functional tests for gRPC services



---

## 🚀 Tech Stack

- **.NET 9**
- **ASP.NET Core**
- **gRPC (Grpc.Net.Client/Grpc.AspNetCore)**
- **Entity Framework Core**
- **SQL Server**
- **InMemory SQL Provider** (Testing)
- **xUnit**, **Moq**, **TestServer**, **WebApplicationFactory**

---

## 🧪 Testing Strategy

- **Unit Tests**: Isolated business logic
- **Integration Tests**: Run against InMemory SQL
- **System Tests**: Full-stack tests with mock gRPC clients
- **Functional Tests**: gRPC service behavior tests

All test projects are fully integrated with Visual Studio Test Explorer.
---

## 🖥️ Running All Projects from Root

You can run the solution using Visual Studio for a seamless experience with build/run/debug/test support.

All tests (unit, integration, system, and functional) can also be run from Visual Studio Test Explorer.

Open a terminal at the root of the solution folder and run the following:

```bash
dotnet build

# Run Web API
dotnet run --project Library.API

# Run gRPC Service
dotnet run --project Library.Service

# Run Tests
dotnet test


