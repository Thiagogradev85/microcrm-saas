# Micro CRM SaaS

> CRM B2B multitenant — Monolito Modular com DDD, .NET 10 e React 18

## Checklist — Fim de sessão

```
1. git commit (se tiver código novo)   → atualiza Obsidian automaticamente
2. "Atualiza o Obsidian"               → pedir ao Claude antes de fechar
3. Fecha o VS Code
```

**Backend:** .NET 10 · ASP.NET Core (Minimal API) · EF Core · MediatR · RabbitMQ.Client · FluentValidation · ASP.NET Core Identity · JWT Bearer · Refresh Tokens · Serilog · OpenTelemetry  
**Frontend:** React 18 · Vite · Tailwind CSS · TanStack Query · dnd-kit · Zod · React Hook Form  
**Infra:** PostgreSQL (Neon) · RabbitMQ (CloudAMQP) · Redis (Upstash) · Render · GitHub Actions

---

## Pré-requisitos

| Ferramenta | Versão mínima | Download |
|-----------|--------------|---------|
| .NET SDK | **10.0.202** (travado via `global.json`) | https://dotnet.microsoft.com/download |
| Git | qualquer recente | https://git-scm.com |
| Node.js | 20 LTS (só para o frontend — Fase 2) | https://nodejs.org |

> O `global.json` na raiz garante que o SDK correto seja usado. Rode `dotnet --version` para confirmar.

---

## Instalação e execução (backend)

```bash
# 1. Clone o repositório
git clone https://github.com/Thiagogradev85/microcrm-saas.git
cd microcrm-saas

# 2. Verifique a versão do SDK (deve retornar 10.x)
dotnet --version

# 3. Restaure e compile
dotnet restore
dotnet build

# 4. Execute os testes
dotnet test
```

> O frontend (React) ainda não existe — será implementado na fase final do MVP.

---

## Variáveis de ambiente

Nenhuma configuração necessária nesta fase (nenhum banco ou serviço externo ainda conectado).

Quando os módulos começarem a ser implementados, um arquivo `.env.example` será adicionado com:
- `DATABASE_URL` — string de conexão PostgreSQL (Neon)
- `RABBITMQ_URL` — string de conexão RabbitMQ (CloudAMQP)
- `REDIS_URL` — string de conexão Redis (Upstash)

---

## Estrutura do projeto

```
microcrm-saas/
├── global.json                   # SDK .NET travado em 10.0.202
├── MicroCrm.sln                  # Solution mestre
├── src/
│   ├── SharedKernel/             # Abstrações puras (Entity, ValueObject, erros, interfaces)
│   └── Modules/
│       ├── Tenants/              # Empresas clientes e planos
│       ├── Identity/             # Usuários e roles
│       ├── Clients/              # Leads e clientes (núcleo do CRM)
│       ├── Sellers/              # Vendedores e territórios por UF
│       ├── Pipeline/             # Kanban visual com drag-and-drop
│       ├── ActivityLog/          # Linha do tempo automática
│       └── Reports/              # Dashboard e relatórios básicos
└── scripts/
    └── sync-obsidian.ps1         # Sync automático com vault Obsidian
```

---

## Status dos módulos

| Módulo | Status | Fase |
|--------|--------|------|
| SharedKernel | 🟡 .csproj criado, sem código ainda | MVP |
| Tenants | ⬜ pasta criada | MVP |
| Identity | ⬜ pasta criada | MVP |
| Clients | ⬜ pasta criada | MVP |
| Sellers | ⬜ pasta criada | MVP |
| Pipeline | ⬜ pasta criada | MVP |
| ActivityLog | ⬜ pasta criada | MVP |
| Reports | ⬜ pasta criada | MVP |
| Goals | ⬜ não iniciado | Fase 2 |
| Catalog | ⬜ não iniciado | Fase 2 |
| Messaging | ⬜ não iniciado | Fase 2 |
| Settings | ⬜ não iniciado | Fase 2 |
| Prospecting | ⬜ não iniciado | Fase 3 |

**Legenda:** ✅ Completo · 🟡 Em andamento · ⬜ Não iniciado

---

## Documentação

A documentação completa (arquitetura, decisões, guias, API) está em repositório separado:

- **Repo:** https://github.com/Thiagogradev85/microcrm-saas-docs
- **Site:** https://thiagogradev85.github.io/microcrm-saas-docs (quando o GitHub Pages estiver ativo)

> Toda decisão de arquitetura relevante deve ser documentada lá além de registrada no Obsidian e no CLAUDE.md.

---

## Licença

© 2026 Thiago Gramuglia. Todos os direitos reservados.

Este repositório é público para fins de portfólio e aprendizado.
O código-fonte não pode ser copiado, modificado, redistribuído
ou utilizado comercialmente sem autorização expressa do autor.
