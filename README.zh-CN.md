<h1 align="center">DDDIFY</h1>

<p align="center">
  一个面向现代 ASP.NET Core 应用程序的轻量级 DDD 集成框架。
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

## 概览

`Dddify` 帮助团队构建在领域逻辑、应用工作流、基础设施关注点与交付层之间具有清晰边界的应用。它围绕 DDD、CQRS、校验、依赖注册、当前用户访问、本地化以及 Entity Framework Core 集成，提供了一组实用的基础构件。

`Dddify` 并不是要替代 ASP.NET Core 生态，而是建立在 MediatR、FluentValidation、Scrutor、Mapster 和 EF Core 等成熟工具之上，让你可以把更多精力放在业务建模上，而不是重复编写基础设施胶水代码。

## 特性

- 提供聚合、值对象、仓储和领域事件等领域驱动设计基础构件
- 体现整洁架构思想的分层方式，让业务规则与基础设施细节保持隔离
- 提供面向 CQRS 的命令、查询与处理器抽象
- 内置基于 MediatR 的数据校验与工作单元管道行为
- 基于 Scrutor 的约定式依赖注册
- 开箱即用的 FluentValidation 集成
- 提供 EF Core 工作单元支持与仓储基类
- 支持基于 JSON 的本地化
- 提供基于 UTC 存储基线的时间与时区处理能力
- 提供适用于应用层与 Web 场景的当前用户抽象
- 通过 `AddDddify(...)` 提供轻量且可组合的配置方式

## 为什么选择 Dddify

- 让领域代码更清晰、更易维护
- 减少处理器注册、校验和基础设施接线的样板代码
- 在不过度增加框架负担的前提下遵循分层架构与整洁架构实践
- 与 ASP.NET Core 生态自然集成
- 保持灵活性，而不是被重量级框架绑定

## 安装

```bash
dotnet add package Dddify
```

## 快速开始

在 ASP.NET Core 应用中注册 `Dddify`：

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDddify(cfg =>
{
    ...
});
```

然后按清晰的分层结构组织你的应用，例如：

```text
MyComany.MyApp.Domain
MyComany.MyApp.Application
MyComany.MyApp.Infrastructure
MyComany.MyApp.Web
```

## 示例

仓库在 [samples/README.md](https://github.com/esofar/dddify/tree/main/samples/README.md) 下提供了官方示例应用。

`TodoApp` 展示了：

- 分层 DDD 结构
- 围绕聚合展开的领域建模
- 命令、查询、校验器和领域事件
- 基于工作单元的 EF Core 持久化
- 一个可运行的 ASP.NET Core Razor Pages UI

该示例被刻意保持为小而聚焦的形式。它适合作为入门参考，但由于目前主要围绕单一聚合展开，还不能完整体现 DDD 在更复杂业务领域中的优势。

## 生态

`Dddify` 设计上可与以下技术良好协作：

`Dddify` is designed to work with:

- ASP.NET Core
- Entity Framework Core
- FluentValidation
- MediatR
- Mapster
- Scrutor
