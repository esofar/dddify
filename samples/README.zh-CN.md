# 示例项目

<p align="center">
  <img src="../assets/todoapp-hero.png" alt="TodoApp 示例截图" width="100%" style="border-radius: 16px;" />
</p>

[English](./README.md) | [中文](./README.zh-CN.md)

## 概览

此目录包含 `Dddify` 的官方示例应用。当前示例 `TodoApp` 是一个实用参考实现，展示了如何构建一个分层的领域驱动设计应用，并在领域逻辑、应用用例、基础设施和 Web 界面之间保持清晰分离。

## 技术栈

- .NET 10
- ASP.NET Core Razor Pages
- Entity Framework Core + SQLite
- Tailwind CSS

## 该示例展示了什么

- 标准四层架构：`Domain`、`Application`、`Infrastructure` 和 `Web`
- 围绕 `Todo` 工作流的聚合式建模
- 命令、查询、DTO、校验器和领域事件处理器
- 仓储与 `DbContext` 的集成，以及 unit of work 配置方式
- 一个简洁且适合真实项目参考的 Razor Pages UI

## 项目结构

```text
samples/
|- TodoApp.Domain/          # 领域模型、聚合、领域事件、仓储契约
|- TodoApp.Application/     # 命令、查询、DTO、校验器、应用处理器、服务接口
|- TodoApp.Infrastructure/  # EF Core 持久化、仓储实现、基础服务、迁移
|- TodoApp.Web/             # Razor Pages 前端、组合根、静态资源
```

## 架构说明

该示例遵循经典的 DDD 分层风格：

- `TodoApp.Domain` 包含核心业务模型，同时也是直接引用 `Dddify` 的层，其他层则通过常规项目引用链共享这些基础能力。
- `TodoApp.Application` 通过命令和查询组织业务用例，向表现层返回 DTO，并处理应用层工作流。
- `TodoApp.Infrastructure` 基于 EF Core 和 SQLite 实现持久化，负责仓储落地和相关支撑服务。
- `TodoApp.Web` 是应用入口，负责配置 `Dddify` 并通过 Razor Pages 渲染界面。

## 快速开始

### 前置要求

- .NET 10 SDK

### 本地运行

在仓库根目录执行：

```bash
dotnet run --project samples/TodoApp.Web/TodoApp.Web.csproj
```

应用默认使用 SQLite，数据库文件配置位于 `samples/TodoApp.Web/appsettings.json`：

```json
"ConnectionStrings": {
  "Default": "Data Source=TodoApp.db"
}
```

### 构建示例

```bash
dotnet build samples/TodoApp.Web/TodoApp.Web.csproj
```

## 范围与限制

`TodoApp` 更适合作为一个轻量、聚焦、便于快速浏览的入门案例。由于当前示例主要围绕单一领域聚合展开，因此还无法完整体现 DDD 在处理更复杂业务逻辑时的优势。
