# Micro CRM SaaS

> Multi-tenant B2B CRM — Modular Monolith with DDD, .NET 10 and React 18

## Stack

**Backend:** .NET 10 · ASP.NET Core · EF Core · MediatR · MassTransit · FluentValidation · Serilog · OpenTelemetry  
**Frontend:** React 18 · Vite · Tailwind CSS · TanStack Query · dnd-kit · Zod · React Hook Form  
**Infra:** PostgreSQL (Neon) · RabbitMQ (CloudAMQP) · Redis (Upstash) · Render · GitHub Actions

## Architecture

Modular Monolith with DDD — 12 bounded contexts: Tenants, Identity, Clients, Sellers,
Pipeline, ActivityLog, Goals, Catalog, Messaging, Prospecting, Reports, Settings.

## Status

🚧 In development — MVP in progress

---

## Licença

© 2026 Thiago Gramuglia. Todos os direitos reservados.

Este repositório é público para fins de portfólio e aprendizado.
O código-fonte não pode ser copiado, modificado, redistribuído
ou utilizado comercialmente sem autorização expressa do autor.
