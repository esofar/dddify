---
layout: home
pageClass: dddify-home

hero:
  name: DDDIFY
  text: Lightweight DDD Integration Framework
  tagline: Designed for modern ASP.NET Core applications, combining DDD and Clean Architecture practices to help teams build maintainable business systems with clear layering, application orchestration, and infrastructure integration.
  actions:
    - theme: brand
      text: Get Started
      link: /en/guide/getting-started
    - theme: alt
      text: GitHub
      link: https://github.com/esofar/dddify

features:
  - icon: 🧱
    title: Domain Modeling Foundation
    details: Provides built-in abstractions such as aggregate roots, entities, value objects, repository contracts, and domain events to help you quickly build a clear domain model.
  - icon: ⚡
    title: Application Layer Message Flow
    details: Orchestrates business use cases through commands, queries, and handlers, with extension points for validation, transactions, and event dispatching.
  - icon: 🧭
    title: Clean Architecture Oriented
    details: Separates business rules from technical implementations, keeping core logic independent, testable, maintainable, and easy to evolve.
  - icon: 🚀
    title: Built on Mature Ecosystem
    details: Integrates with mature components such as EF Core, MediatR, Scrutor, Mapster, and FluentValidation.
  - icon: 🪶
    title: Native and Flexible
    details: Does not take over authentication, authorization, UI, module systems, or project templates, allowing projects to continue using native ASP.NET Core composition.
  - icon: 🧩
    title: Built-in Common Capabilities
    details: Includes foundational capabilities such as unit of work, current user, data persistence, time and time zone handling, and localization to reduce boilerplate code.
---

<section class="home-section home-scenarios">
  <h2>Use Cases</h2>
  <div class="scenario-grid">
    <article>
      <strong>Complex Business Systems</strong>
      <span>Designed for enterprise applications with complex business rules, clear boundaries, and long-term maintenance needs, helping teams build stable domain models.</span>
    </article>
    <article>
      <strong>Multi-module Domain Modeling</strong>
      <span>Suitable for splitting modules by business capabilities and organizing core business logic with aggregates, entities, value objects, and domain events.</span>
    </article>
    <article>
      <strong>Layered Architecture Projects</strong>
      <span>Designed for typical four-layer architecture, clearly separating responsibilities across domain models, application use cases, infrastructure, and Web entry points.</span>
    </article>
    <article>
      <strong>Legacy Project Architecture Evolution</strong>
      <span>Supports the gradual adoption of DDD, CQRS, unit of work, and domain events in existing ASP.NET Core projects.</span>
    </article>
    <article>
      <strong>From Samples to Production</strong>
      <span>Suitable for .NET teams that want to apply DDD in real business scenarios without building all foundational infrastructure from scratch.</span>
    </article>
  </div>
</section>

<section class="home-section home-abp">
  <h2>Comparison with ABP Framework</h2>
  <div class="home-table">
    <table>
      <thead>
        <tr>
          <th>Dimension</th>
          <th>Dddify</th>
          <th>ABP</th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td><strong>Framework Positioning</strong></td>
          <td>Lightweight DDD integration layer</td>
          <td>Full-featured modular application framework</td>
        </tr>
        <tr>
          <td><strong>Complexity</strong></td>
          <td>Few integration points, fewer conventions, and easy to get started</td>
          <td>Feature-rich and comprehensive, with a relatively steeper learning curve</td>
        </tr>
        <tr>
          <td><strong>Feature Scope</strong></td>
          <td>Focuses on foundational capabilities such as DDD, CQRS, validation, and unit of work</td>
          <td>Covers DDD, multi-tenancy, permission management, UI integration, prebuilt modules, and more</td>
        </tr>
        <tr>
          <td><strong>Extension Model</strong></td>
          <td>Preserves native ASP.NET Core composition and integrates on demand</td>
          <td>Extends through a modular system with rich capabilities but more conventions</td>
        </tr>
        <tr>
          <td><strong>Use Cases</strong></td>
          <td>Small and medium-sized projects, modular monoliths, and gradual modernization of existing projects</td>
          <td>Large business applications, SaaS platforms, and multi-module enterprise systems</td>
        </tr>
        <tr>
          <td><strong>Ecosystem Support</strong></td>
          <td>Mainly relies on the .NET and ASP.NET Core ecosystem</td>
          <td>Provides official modules, templates, documentation, toolchains, and community support</td>
        </tr>
        <tr>
          <td><strong>Development Efficiency</strong></td>
          <td>Lightweight startup, easier foundational wiring, and advanced capabilities built as needed</td>
          <td>Rich prebuilt capabilities for quickly building complete applications, but with higher maintenance costs</td>
        </tr>
      </tbody>
    </table>
  </div>
  <p>
    Dddify is better suited for ASP.NET Core projects that need a lightweight, flexible, and composable DDD foundation; ABP is better suited for one-stop enterprise application development that requires full platform capabilities.
  </p>
</section>