---
outline: [2, 3] 
---

# 分层架构

Dddify 遵循基于整洁架构设计思想的分层设计方案，将代码划分为四个主要层：**领域层**、**应用层**、**基础设施层** 和 **展示层**。各层之间职责分离，依赖关系清晰，有助于项目模块化、低耦合以及提高代码的可维护性和可测试性。

## 代码分层

各层的主要职责和依赖关系如下：

### 领域层

- **主要职责**
    - **业务建模**：定义领域模型，包括聚合根、实体、值对象等，完整表达业务概念。
    - **业务规则**：实现领域服务与业务规则，确保领域对象始终处于一致有效状态。
    - **保持纯净**：领域层作为系统核心，不依赖于其他层，只专注于业务规则和领域概念。

- **依赖关系**
    - **无对外依赖**：领域层不依赖于应用层、基础设施层或展示层，仅供其他层调用。
    - **引用基础库**：需要引用 Dddify 等支持性 NuGet 包。

### 应用层

- **主要职责**
    - **用例编排**：定义并协调系统的各个用例（命令与查询），负责调用领域层的模型与服务来完成业务流程。
    - **事务管理**：处理事务边界、工作单元模式等，确保业务操作的一致性。
    - **接口定义**：声明服务接口，供基础设施层提供具体实现。

- **依赖关系**
    - **依赖领域层**：应用层依赖领域层来执行业务逻辑，但不直接涉及技术实现细节。
    - **对外隔离**：展示层只需调用应用层暴露的接口，无需了解内部业务规则。

### 基础设施层

- **主要职责**
    - **技术实现**：实现持久化（如 EF Core 上下文）、外部服务、缓存、消息总线等技术细节。
    - **接口实现**：对领域层定义的仓储接口和应用层定义的服务接口进行具体实现。
    - **辅助支持**：负责与操作系统、网络、数据库等低层平台打交道。

- **依赖关系**
    - **依赖领域层和应用层**：通过实现领域层和应用层定义的接口来提供技术支持，通过依赖注入与上层解耦。
    - **内向依赖**：确保所有外部依赖均通过应用层或领域层的抽象接口来隔离。

### 展示层

- **主要职责**
    - **用户交互**：负责处理用户请求、展示结果，如 Web API、MVC 控制器、前端 UI 等。
    - **调用服务**：将用户请求转化为应用层用例调用，获取业务结果后展示。
    - **界面逻辑**：处理界面相关的展示逻辑，但不包含业务规则的实现。

- **依赖关系**
    - **主要依赖应用层**：通过调用应用层服务（中介者发送命令或查询）来执行业务逻辑，保持与领域层和基础设施层的隔离。
    - **依赖基础设施层**：展示层在启动配置中可能引用基础设施层（如注册 EF Core 上下文），但仅用于配置目的，不直接调用业务逻辑。


## 解决方案目录

以下是一个基于上述设计原则的 **推荐解决方案目录** 结构，同时包含对应的测试和文档目录。

```
├── MyCompany.MyProject.sln                     # 解决方案文件
├── docs/
│   ├── README.md                               # 项目简介及说明
│   │   ...
├── src/
│   ├── MyCompany.MyProject.Domain/             # 领域层
│   │   ├── Entities/                           # 实体
│   │   ├── ValueObjects/                       # 值对象
│   │   ├── Aggregates/                         # 聚合根
│   │   ├── Services/                           # 领域服务
│   │   ├── Events/                             # 领域事件
│   │   └── Repositories/                       # 仓储接口
│   │   ...
│   ├── MyCompany.MyProject.Application/        # 应用层
│   │   ├── Commands/                           # 命令、命令处理器、命令验证器
│   │   ├── Queries/                            # 查询、查询处理器
│   │   ├── Services/                           # 服务接口
│   │   ├── Behaviors/                          # 自定义 MediatR 管道行为
│   │   ├── Mappings/                           # 对象映射配置
│   │   ├── Dtos/                               # 数据传输对象
│   │   └── EventHandlers/                      # 领域事件处理器
│   │   ...
│   ├── MyCompany.MyProject.Infrastructure/     # 基础设施层
│   │   ├── Data/                               # EF Core 上下文、实体配置、数据迁移
│   │   ├── Repositories/                       # 仓储实现
│   │   ├── Services/                           # 服务实现
│   │   ...
│   └── MyCompany.MyProject.Presentation/       # 展示层
│       ├── Controllers/                        # WebAPI、MVC 控制器
│       ├── Models/                             # 请求参数模型
│       └── Middlewares/                        # 自定义中间件
│       └── Filters/                            # 自定义过滤器
│       └── Pages/                              # Razor 视图
│       └── Resources/                          # 本地化资源
│       ...
├── tests/
│   ├── MyCompany.MyProject.Domain.Tests/       # 领域层测试
│   ├── MyCompany.MyProject.Application.Tests/  # 应用层测试
│   └── MyCompany.MyProject.Integration.Tests/  # 集成测试
```

**引用关系**

```
MyCompany.MyProject.Application --> MyCompany.MyProject.Domain
MyCompany.MyProject.Infrastructure --> MyCompany.MyProject.Application
MyCompany.MyProject.Infrastructure --> MyCompany.MyProject.Domain
MyCompany.MyProject.Presentation --> MyCompany.MyProject.Application
MyCompany.MyProject.Presentation --> MyCompany.MyProject.Infrastructure
```

**命名约定**

- `MyCompany`：表示公司的名称或组织名称。
- `MyProject`：表示项目的名称或应用程序的名称。

::: info
Dddify 定位“轻量级”框架，旨在保持其核心功能的简洁性，因此选择不提供项目创建模板，开发者需参考自行创建或调整解决方案目录。
:::