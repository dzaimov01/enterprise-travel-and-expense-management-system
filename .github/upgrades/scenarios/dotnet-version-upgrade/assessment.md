# Projects and dependencies analysis

This document provides a comprehensive overview of the projects and their dependencies in the context of upgrading to .NETCoreApp,Version=v11.0.

## Table of Contents

- [Executive Summary](#executive-Summary)
  - [Highlevel Metrics](#highlevel-metrics)
  - [Projects Compatibility](#projects-compatibility)
  - [Package Compatibility](#package-compatibility)
  - [API Compatibility](#api-compatibility)
- [Aggregate NuGet packages details](#aggregate-nuget-packages-details)
- [Top API Migration Challenges](#top-api-migration-challenges)
  - [Technologies and Features](#technologies-and-features)
  - [Most Frequent API Issues](#most-frequent-api-issues)
- [Projects Relationship Graph](#projects-relationship-graph)
- [Project Details](#project-details)

  - [enterprise-travel-and-expense-management-system/enterprise-travel-and-expense-management-system.csproj](#enterprise-travel-and-expense-management-systementerprise-travel-and-expense-management-systemcsproj)


## Executive Summary

### Highlevel Metrics

| Metric | Count | Status |
| :--- | :---: | :--- |
| Total Projects | 1 | All require upgrade |
| Total NuGet Packages | 4 | 2 need upgrade |
| Total Code Files | 1 |  |
| Total Code Files with Incidents | 2 |  |
| Total Lines of Code | 24 |  |
| Total Number of Issues | 4 |  |
| Estimated LOC to modify | 1+ | at least 4,2% of codebase |

### Projects Compatibility

| Project | Target Framework | Difficulty | Package Issues | API Issues | Est. LOC Impact | Description |
| :--- | :---: | :---: | :---: | :---: | :---: | :--- |
| [enterprise-travel-and-expense-management-system/enterprise-travel-and-expense-management-system.csproj](#enterprise-travel-and-expense-management-systementerprise-travel-and-expense-management-systemcsproj) | net10.0 | 🟢 Low | 2 | 1 | 1+ | AspNetCore, Sdk Style = True |

### Package Compatibility

| Status | Count | Percentage |
| :--- | :---: | :---: |
| ✅ Compatible | 2 | 50,0% |
| ⚠️ Incompatible | 0 | 0,0% |
| 🔄 Upgrade Recommended | 2 | 50,0% |
| ***Total NuGet Packages*** | ***4*** | ***100%*** |

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 1 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 43 |  |
| ***Total APIs Analyzed*** | ***44*** |  |

## Aggregate NuGet packages details

| Package | Current Version | Suggested Version | Projects | Description |
| :--- | :---: | :---: | :--- | :--- |
| FluentValidation.DependencyInjectionExtensions | 11.9.0 |  | [enterprise-travel-and-expense-management-system.csproj](#enterprise-travel-and-expense-management-systementerprise-travel-and-expense-management-systemcsproj) | ✅Compatible |
| MediatR | 12.2.0 |  | [enterprise-travel-and-expense-management-system.csproj](#enterprise-travel-and-expense-management-systementerprise-travel-and-expense-management-systemcsproj) | ✅Compatible |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.2 | 10.0.5 | [enterprise-travel-and-expense-management-system.csproj](#enterprise-travel-and-expense-management-systementerprise-travel-and-expense-management-systemcsproj) | NuGet package upgrade is recommended |
| Microsoft.EntityFrameworkCore.Tools | 8.0.2 | 10.0.5 | [enterprise-travel-and-expense-management-system.csproj](#enterprise-travel-and-expense-management-systementerprise-travel-and-expense-management-systemcsproj) | NuGet package upgrade is recommended |

## Top API Migration Challenges

### Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |

### Most Frequent API Issues

| API | Count | Percentage | Category |
| :--- | :---: | :---: | :--- |
| T:Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions | 1 | 100,0% | Binary Incompatible |

## Projects Relationship Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart LR

```

## Project Details

