<h1 align="center">DDDIFY</h1>

<p align="center">
  A lightweight DDD integration framework for modern ASP.NET Core applications.
</p>

<p align="center">
  <a href="https://github.com/esofar/dddify/actions/workflows/dotnet-ci.yml">
    <img src="https://img.shields.io/github/actions/workflow/status/esofar/dddify/dotnet-ci.yml?branch=main&style=for-the-badge&label=Build" alt="Build Status" />
  </a>
  <a href="https://www.nuget.org/packages/dddify">
    <img src="https://img.shields.io/nuget/v/dddify.svg?style=for-the-badge&label=NuGet&color=0B6CFF" alt="NuGet Version" />
  </a>
  <a href="https://github.com/esofar/dddify/blob/main/LICENSE">
    <img src="https://img.shields.io/badge/license-MIT-44CC11?style=for-the-badge" alt="License" />
  </a>
  <img src="https://img.shields.io/badge/.NET-10%2B-512BD4?style=for-the-badge" alt=".NET 10+" />
</p>

<p align="center">
  <a href="./README.md">English</a> |
  <a href="./README.zh-CN.md">简体中文</a>
</p>

## Overview

`Dddify` helps teams build applications with a clear separation between domain logic, application workflows, infrastructure concerns, and delivery layers. It brings together a set of practical building blocks around DDD, CQRS, validation, dependency registration, current-user access, localization, and Entity Framework Core integration.

Rather than replacing the ASP.NET Core ecosystem, `Dddify` builds on proven tools such as MediatR, FluentValidation, Scrutor, Mapster, and EF Core, so you can stay focused on business modeling instead of wiring repetitive infrastructure code.

## Features

- Domain-driven design building blocks such as aggregates, value objects, repositories, and domain events
- Clean Architecture-oriented layering that keeps business rules isolated from infrastructure details
- CQRS-friendly abstractions for commands, queries, and handlers
- Built-in MediatR pipeline behaviors for validation and unit of work
- Convention-based dependency registration with Scrutor
- FluentValidation integration out of the box
- EF Core unit-of-work support and repository base types
- JSON localization support
- Built-in time and timezone handling based on a UTC storage baseline
- Current-user abstractions for application and web scenarios
- Lightweight, composable configuration through `AddDddify(...)`

## Why Dddify

- Keep domain code explicit and easier to maintain
- Reduce boilerplate around handler registration, validation, and infrastructure wiring
- Follow layered architecture and clean architecture practices without excessive ceremony
- Integrate naturally with the ASP.NET Core ecosystem
- Stay flexible instead of being locked into a heavy framework

## Installation

```bash
dotnet add package Dddify
```

## Quick Start

Register `Dddify` in your ASP.NET Core application:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDddify(cfg =>
{
    cfg.AddTiming();
    cfg.AddLocalization();
    cfg.AddCurrentUser();

    cfg.AddDbContextWithUnitOfWork<ApplicationDbContext>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
    });

    ...
});
```

Then organize your application with clear layers such as:

```text
MyComany.MyApp.Domain
MyComany.MyApp.Application
MyComany.MyApp.Infrastructure
MyComany.MyApp.Web
```

## Sample

The repository includes an official sample application under [samples/README.md](https://github.com/esofar/dddify/tree/main/samples/README.md).

`TodoApp` demonstrates:

- A layered DDD structure
- Aggregate-centric domain modeling
- Commands, queries, validators, and domain events
- EF Core persistence with unit of work
- A working ASP.NET Core Razor Pages UI

The sample is intentionally small and focused. It is a good onboarding reference, but because it currently centers on a single aggregate, it does not fully demonstrate the strengths of DDD in more complex business domains.

## Ecosystem

`Dddify` is designed to work with:

- ASP.NET Core
- Entity Framework Core
- FluentValidation
- MediatR
- Mapster
- Scrutor
