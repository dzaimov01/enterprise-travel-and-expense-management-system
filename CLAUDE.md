# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Quick Start

### Common Commands

**Build the project:**
```bash
dotnet build
```

**Run the API:**
```bash
dotnet run
```
The API starts on `http://localhost:5142` (HTTP) and `https://localhost:7259` (HTTPS) with Swagger UI at `/swagger/index.html`.

**Run with a specific launch profile:**
```bash
dotnet run --launch-profile https
```

**Restore dependencies:**
```bash
dotnet restore
```

**Add a new NuGet package:**
```bash
dotnet add package <PackageName>
```

## Architecture

### High-Level Design

This project implements an **Enterprise Travel & Expense Management System** using the **Mediator pattern** (via MediatR) and **CQRS (Command Query Responsibility Segregation)** principles:

- **MediatR**: Decouples commands/queries from handlers, enabling loose coupling between components and clean separation of concerns
- **FluentValidation**: Provides validation rules with full async support and dependency injection integration
- **Entity Framework Core**: Data access layer with SQL Server backend
- **ASP.NET Core Web API**: RESTful endpoints with Swagger documentation

### Key Patterns

**Commands & Queries (CQRS):**
- **Commands**: Represent state-changing operations (e.g., `ApprovalTravelRequest`, `ProcessExpense`)
- **Queries**: Represent read-only operations (e.g., `GetTravelRequest`, `ListPendingApprovals`)
- Use MediatR's `IRequest<T>` for command/query definitions
- Implement `IRequestHandler<TRequest, TResponse>` for handlers

**Validation:**
- Each command/query **must** have a corresponding `FluentValidation` validator implementing `AbstractValidator<T>`
- Validators are auto-registered in `Program.cs` via `AddValidatorsFromAssembly()`
- MediatR's validation pipeline automatically runs validators before handlers

**Side Effects:**
- Complex business logic (e.g., approval chain → email notification → audit log) is handled by command handlers
- Use intermediate events/notifications if a handler needs to trigger multiple side effects
- Consider `INotification` and `NotificationHandler<>` for pub-sub patterns

### Dependency Injection

All components are registered in `Program.cs`:
- MediatR handlers auto-scan the assembly for `IRequestHandler<,>` implementations
- Validators are auto-registered from the assembly
- Controllers are added with `AddControllers()`
- Swagger generation is configured for OpenAPI documentation

## Project Structure (Recommended)

```
enterprise-travel-and-expense-management-system/
├── Features/
│   ├── TravelRequests/
│   │   ├── Commands/
│   │   │   ├── CreateTravelRequestCommand.cs
│   │   │   ├── ApproveTravelRequestCommand.cs
│   │   │   └── ...
│   │   ├── Queries/
│   │   │   ├── GetTravelRequestQuery.cs
│   │   │   ├── ListPendingApprovalsQuery.cs
│   │   │   └── ...
│   │   ├── Handlers/
│   │   │   ├── CreateTravelRequestCommandHandler.cs
│   │   │   ├── ApproveTravelRequestCommandHandler.cs
│   │   │   ├── GetTravelRequestQueryHandler.cs
│   │   │   └── ...
│   │   ├── Validators/
│   │   │   ├── CreateTravelRequestValidator.cs
│   │   │   ├── ApproveTravelRequestValidator.cs
│   │   │   └── ...
│   │   └── TravelRequestController.cs
│   ├── Expenses/
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── Handlers/
│   │   ├── Validators/
│   │   └── ExpenseController.cs
│   └── ...
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── Migrations/
│   └── ...
├── Models/
│   ├── Entities/
│   └── DTOs/
├── Program.cs
├── appsettings.json
└── appsettings.Development.json
```

## Database

### Entity Framework Core Setup
- Provider: SQL Server
- Tools: EF Core Command-Line Interface

**Key Commands:**
```bash
# Create a new migration
dotnet ef migrations add <MigrationName>

# Update database to latest migration
dotnet ef database update

# Remove the last migration
dotnet ef migrations remove

# Drop and recreate database (development only)
dotnet ef database drop --force
dotnet ef database update
```

### DbContext
Define `ApplicationDbContext` inheriting from `DbContext`. Register it in `Program.cs` with the connection string from `appsettings.json`.

## Target Framework

- **Framework**: .NET 11.0
- **Language Features**: Nullable reference types enabled, implicit usings enabled
- Ensure all projects target the same framework for consistency

## Key Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| MediatR | 12.2.0 | CQRS/Mediator pattern |
| FluentValidation.DependencyInjectionExtensions | 11.9.0 | Data validation |
| Microsoft.EntityFrameworkCore.SqlServer | 10.0.5 | EF Core SQL Server provider |
| Microsoft.EntityFrameworkCore.Tools | 10.0.5 | EF Core CLI tools |

## Development Notes

- **Async/Await**: All MediatR handlers are async by default (`IRequestHandler<TRequest, TResponse>` is async-friendly)
- **Validation**: Never skip validators; they're part of the clean architecture
- **Controllers**: Keep controllers thin—delegate business logic to command/query handlers
- **Naming Convention**: Use PascalCase for namespaces matching folder structure (e.g., `Features.TravelRequests.Commands`)
- **Error Handling**: Implement a centralized exception middleware for consistent error responses
- **Logging**: Configure structured logging in `appsettings.json` for debugging complex mediator flows
