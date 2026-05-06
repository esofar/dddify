---
layout: home
pageClass: dddify-home

hero:
  name: DDDIFY
  text: 轻量级 DDD 集成框架
  tagline: 面向现代 ASP.NET Core 应用，融合 DDD 与 Clean Architecture 实践，帮助团队以清晰分层、应用编排与基础设施集成构建可维护的业务系统。
  actions:
    - theme: brand
      text: 快速开始
      link: /guide/getting-started
    - theme: alt
      text: GitHub
      link: https://github.com/esofar/dddify

features:
  - icon: 🧱
    title: 领域建模基础
    details: 内置聚合根、实体、值对象、仓储契约与领域事件等基础抽象，可快速搭建清晰的领域模型。
  - icon: ⚡
    title: 应用层消息流
    details: 基于命令、查询与处理器编排业务用例，支持验证、事务、事件分发等扩展点。
  - icon: 🧭
    title: 遵循整洁架构
    details: 分离业务规则与技术实现，让核心逻辑保持独立，代码更易测试、维护与持续演进。
  - icon: 🚀
    title: 复用成熟生态
    details: 基于 EF Core、MediatR、Scrutor、Mapster、FluentValidation 等成熟组件集成。
  - icon: 🪶
    title: 保持原生灵活
    details: 不接管认证授权、UI、模块系统或项目模板，让项目继续使用 ASP.NET Core 原生组合方式。
  - icon: 🧩
    title: 常用能力内置
    details: 集成工作单元、当前用户、数据持久化、时间与时区、本地化等基础能力，减少样板代码。
---

<section class="home-section home-scenarios">
  <h2>适用场景</h2>
  <div class="scenario-grid">
    <article>
      <strong>复杂业务系统</strong>
      <span>面向业务规则复杂、边界清晰、需要长期维护的企业级应用，帮助团队沉淀稳定的领域模型。</span>
    </article>
    <article>
      <strong>多模块领域建模</strong>
      <span>适合按业务能力拆分模块，通过聚合、实体、值对象与领域事件组织核心业务逻辑。</span>
    </article>
    <article>
      <strong>分层架构项目</strong>
      <span>面向典型四层架构设计，清晰划分领域模型、应用用例、基础设施与 Web 入口职责。</span>
    </article>
    <article>
      <strong>旧项目架构演进</strong>
      <span>支持在现有 ASP.NET Core 项目中渐进式引入 DDD、CQRS、工作单元与领域事件能力。</span>
    </article>
    <article>
      <strong>从示例走向生产</strong>
      <span>适合希望在真实业务中落地 DDD，而不想从零搭建基础设施的 .NET 团队。</span>
    </article>
  </div>
</section>
<section class="home-section home-abp">
  <h2>与 ABP 框架对比</h2>
  <div class="home-table">
    <table>
      <thead>
        <tr>
          <th>维度</th>
          <th>Dddify</th>
          <th>ABP</th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td><strong>框架定位</strong></td>
          <td>轻量级 DDD 集成层</td>
          <td>完整模块化应用框架</td>
        </tr>
        <tr>
          <td><strong>复杂程度</strong></td>
          <td>接入点少、约定较少，简单易上手</td>
          <td>功能丰富、体系完整，学习曲线相对较高</td>
        </tr>
        <tr>
          <td><strong>功能范围</strong></td>
          <td>聚焦 DDD、CQRS、验证、工作单元等基础能力</td>
          <td>覆盖 DDD、多租户、权限管理、UI 集成、预构建模块等</td>
        </tr>
        <tr>
          <td><strong>扩展方式</strong></td>
          <td>保留 ASP.NET Core 原生组合方式，按需集成</td>
          <td>基于模块化体系扩展，能力完整但约定更多</td>
        </tr>
        <tr>
          <td><strong>适用场景</strong></td>
          <td>中小型项目、模块化单体、已有项目渐进式改造</td>
          <td>大型业务应用、SaaS 平台、多模块企业系统</td>
        </tr>
        <tr>
          <td><strong>生态支持</strong></td>
          <td>主要依托 .NET 与 ASP.NET Core 生态</td>
          <td>拥有官方模块、模板、文档、工具链和社区支持</td>
        </tr>
        <tr>
          <td><strong>开发效率</strong></td>
          <td>启动轻量，基础接线更省心，高级能力按需自建</td>
          <td>预置能力丰富，可快速搭建完整应用，但维护成本更高</td>
        </tr>
      </tbody>
    </table>
  </div>
  <p>
    Dddify 更适合需要轻量、灵活、可组合 DDD 基础层的 ASP.NET Core 项目；ABP 更适合需要完整平台能力的一站式企业应用开发。
  </p>
</section>
