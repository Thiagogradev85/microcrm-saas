# Micro CRM SaaS — CLAUDE.md

CRM B2B multitenant em **.NET 10**. Padrão: **Monolito Modular + DDD**. MVP focado em ser vendável.

> Este arquivo é o índice + invariantes do projeto. Para detalhes, **a fonte de verdade é o vault Obsidian** (ver abaixo).
> Boas práticas inspiradas em [shanraisshan/claude-code-best-practice](https://github.com/shanraisshan/claude-code-best-practice).

---

## 🧠 Memória do projeto — Obsidian (LEIA PRIMEIRO em toda sessão)

**Vault:** `C:\Users\thiag\Documents\Obsidian Vault\Micro-CRM-SAAS\`

O Obsidian é a memória persistente. Este `CLAUDE.md` resume; o vault detalha. **Em caso de divergência, o vault vence.**

### Ordem de leitura ao iniciar a sessão
1. `00 - Índice.md` — hub central
2. `05 - Progresso/Estado Atual.md` — fotografia do código hoje
3. `06 - Próximos Passos/Próximos Passos.md` — o que fazer agora

### Atualizar ao final da sessão
- Criou/modificou `.cs` → `05 - Progresso/Estado Atual.md`
- Commit importante → `05 - Progresso/Commits Importantes.md` (hook automático)
- Decisão estratégica → `04 - Decisões/Decisões Estratégicas.md`
- Próximo passo mudou → `06 - Próximos Passos/Próximos Passos.md`
- Módulo implementado → `02 - Arquitetura/Módulos MVP.md` **+ README.md**
- Stack mudou → `01 - Projeto/Stack Tecnológica.md` **+ README.md**

Sync automático: `.git/hooks/post-commit` → `scripts/sync-obsidian.ps1`. Manual: `powershell -File scripts/sync-obsidian.ps1 -Full`.

### Como responder perguntas sobre o projeto
**Para qualquer dúvida sobre regra de negócio, padrão arquitetural ou estado do projeto, busque PRIMEIRO no vault Obsidian.** Só recorra a docs externas (Microsoft Learn, blogs) ou conhecimento de treino se a resposta não estiver no vault. Quando recorrer a fonte externa para uma decisão de arquitetura, registre o achado em `04 - Decisões/Decisões Estratégicas.md`.

---

## 👤 Sobre o desenvolvedor

**Thiago Gramuglia** — dev .NET Júnior aprendendo C#/.NET enquanto constrói o projeto. Background em Node.js + React.

**Implicações na geração de código:**
- **Clareza > concisão.** Sem one-liners de LINQ, pattern matching avançado ou açúcar sintático novo sem explicação.
- **Comentar conceitos novos** com 1–2 linhas — *"por que `sealed`?"*, *"o que é `init`?"*, *"por que `record` aqui?"*.
- **Duas formas de escrever?** Explique brevemente o porquê da escolha.
- **Refatorar algo grande?** Explique a motivação em texto antes de aplicar.
- **Idioma:** português em commits, docs e comentários. Identificadores de código em inglês (convenção .NET).

---

## 🔗 Repositórios

- **Backend (este repo):** https://github.com/Thiagogradev85/microcrm-saas
- **Docs (Docusaurus):** https://github.com/Thiagogradev85/microcrm-saas-docs · clone: `C:\Users\thiag\Documents\Projetos\microcrm-saas-docs`
- **Site publicado:** https://thiagogradev85.github.io/microcrm-saas-docs/
- **Projeto anterior (Node.js, arquivado):** https://github.com/Thiagogradev85/Micro-CRM-SAAS

---

## 🛠 Stack (resumo — detalhes em `01 - Projeto/Stack Tecnológica.md`)

**Backend:** .NET 10 LTS (SDK 10.0.202 via `global.json`) · ASP.NET Core 10 (Minimal APIs) · EF Core 10 · ASP.NET Core Identity (PBKDF2) · JWT Bearer + Refresh Tokens · MediatR (CQRS) · FluentValidation · **RabbitMQ.Client direto** (sem MassTransit) · Serilog · OpenTelemetry · Swagger · xUnit + FluentAssertions + Moq

**Frontend (fase posterior):** React 18 + Vite + TS · Tailwind · **dnd-kit** (NÃO `react-beautiful-dnd`) · TanStack Query · Zod · React Hook Form · **Mobile First obrigatório**

**Infra (free tier):** PostgreSQL (Neon) · RabbitMQ (CloudAMQP) · Redis (Upstash) · Render · GitHub Actions

---

## 🏛 Arquitetura — Monolito Modular + DDD

Um único deploy, internamente dividido em **Bounded Contexts** isolados. Cada módulo tem seu próprio schema. Comunicação entre módulos **só via Domain Events** no RabbitMQ — chamada direta é **proibida**.

### Estrutura por módulo
```
ModuloX/
├── ModuloX.Domain/         # Entidades, ValueObjects, eventos
├── ModuloX.Application/    # Handlers MediatR, DTOs, validators
├── ModuloX.Infrastructure/ # EF Core, repositórios, integrações
└── ModuloX.Api/            # Endpoints Minimal API
```

### SharedKernel — zero dependências externas
**Regra rígida:** `SharedKernel.csproj` NUNCA referencia pacote NuGet (EF, MediatR, etc.). Apenas `System.*`.

Contém: `Entity`, `AggregateRoot`, `ValueObject`, hierarquia `AppError` (`NotFoundError`/404, `ValidationError`/422, `UnauthorizedError`/403, `DuplicateError`/409, `PlanLimitError`/402, `DomainError`/400) e interfaces transversais (`ITenantContext`, `ICurrentUser`, `IRepository<T>`, `IUnitOfWork`, `IEventBus`, `IDomainEvent`).

Detalhes em `02 - Arquitetura/SharedKernel.md`.

### Módulos (Bounded Contexts)
- **MVP:** Tenants · Identity · Clients · Sellers · Pipeline · ActivityLog · Reports
- **Fase 2:** Goals · Catalog · Messaging · Settings
- **Fase 3:** Prospecting · IA/Enriquecimento

Detalhes em `02 - Arquitetura/Módulos MVP.md`.

### Hierarquia de usuários
- **SuperAdmin** (Thiago) — gerencia tenants e planos
- **TenantAdmin** — administra a empresa cliente
- **User** — operador dentro das permissões do TenantAdmin

---

## ⚠️ Regras de negócio críticas (invariantes)

O código DEVE garantir. Não são opcionais. Detalhes em `03 - Regras de Negócio/Regras Críticas.md`.

1. **Isolamento por tenant absoluto.** Toda query EF que toca tabela tenant-owned passa por `HasQueryFilter(x => x.TenantId == _tenantContext.CurrentTenantId)`. Nunca filtrar no Application.
2. **UF não bloqueia cadastro.** Cliente sem UF recebe tag automática `Sem UF` e fila laranja no Kanban.
3. **Exclusividade de UF por vendedor.** 1 vendedor por UF por tenant — violação retorna `DuplicateError` (409).
4. **Ganhou/Perdeu são finais.** Reabertura exige ação explícita + registro com motivo no ActivityLog.
5. **`blocked` impede campanhas.** Antes de qualquer disparo no Messaging, conferir status.
6. **Arquivado = soft delete.** Persiste no banco; queries normais filtram `archived = false`.
7. **Idempotência de eventos.** Mesmo Domain Event 2x **não pode** gerar efeito duplicado. Handler confere estado antes de aplicar.
8. **Último TenantAdmin não sai.** Bloqueado até outro ser promovido.

### Pipeline — 3 camadas independentes (não é um campo "status" único)
- **Coluna do Kanban** (enum): Novo Lead → Em Contato → Qualificado → Proposta Enviada → Em Negociação → Ganhou ✅ / Perdeu ❌
- **Status de Relacionamento** (enum): `lead | customer | inactive | partner | archived | blocked`
- **Tags** (lista de strings): `Catálogo Enviado`, `WhatsApp Ativo`, `Sem UF`, `VIP`, `Indicação`, `Feira/Evento`, `Aguardando Retorno`, `Follow-up Urgente`

Detalhes em `03 - Regras de Negócio/Pipeline e Status.md`.

---

## 📐 Convenções de código

### Nomenclatura ([Microsoft Design Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/general-naming-conventions))
- **Classes, records, métodos públicos:** `PascalCase` — `ClientAggregate`, `CreateClientCommand`
- **Variáveis locais e parâmetros:** `camelCase` — `tenantId`
- **Campos privados:** `_camelCase` — `_repository`
- **Interfaces:** prefixo `I` — `ITenantContext`
- **Booleanos:** `Is` / `Can` / `Has` — `IsActive`, `CanDelete`
- **Métodos async:** sufixo `Async` — `GetByIdAsync()`
- **Namespaces:** `MicroCrm.<Modulo>.<Camada>` — `MicroCrm.Clients.Domain`

### Padrões obrigatórios
- Toda entidade herda de `Entity` ou `AggregateRoot` do SharedKernel.
- ValueObjects são imutáveis — sem setters públicos, igualdade por valor.
- Construtor de domínio privado (`private Client() { }` para o EF). Instâncias via factory estático (`Client.Create(...)`) que valida invariantes.
- Métodos públicos que podem falhar retornam **`Result<T>`** (padrão principal). Nunca lançam `Exception` genérica — propagam `AppError` ou retornam erro no `Result<T>`.
- DTOs são **`record`** (imutáveis, igualdade por valor).
- Nunca retornar tupla em método público — usar `Result<T>`, `record` ou DTO.
- Handlers MediatR em `Application/Commands/<Feature>/<FeatureCommandHandler>.cs`.
- Um tipo por arquivo. Nome do arquivo = nome do tipo. Namespaces espelham pastas.

### ORM — EF Core + Dapper (CQRS híbrido)
- **EF Core** → lado de **Commands** (escrita): transações, agregados DDD, change tracking, `HasQueryFilter` multitenant.
- **Dapper** → lado de **Queries** (leitura): relatórios complexos, projeções, performance crítica. Adicionar quando `Reports` evoluir.
- Coexistem no mesmo projeto. Fonte: [Microsoft Learn — EF Core + DDD CQRS](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-implementation-entity-framework-core).

### Testes
- Estrutura espelha `src/`: `tests/Modules/Clients.Tests/...`
- Nome: `MethodName_StateUnderTest_ExpectedBehavior`
- Padrão **AAA** (Arrange / Act / Assert) com linha em branco entre blocos.
- Foco: agregados e handlers. Endpoints só em testes de integração.

Mais em `02 - Arquitetura/Padrões e Convenções.md`.

---

## 🚫 O que NÃO fazer

- ❌ `async void` (exceto event handlers do framework)
- ❌ Chamada direta entre módulos — usar Domain Events
- ❌ Lógica de negócio em controllers/endpoints
- ❌ Acessar `DbContext` direto no Application — passar por repositório
- ❌ `DateTime.Now` — sempre `DateTime.UtcNow` ou injetar `IClock`
- ❌ Adicionar pacote NuGet ao `SharedKernel.csproj`
- ❌ Filtros de tenant no Application — sempre via `HasQueryFilter` no EF
- ❌ `react-beautiful-dnd` (descontinuado) — usar `dnd-kit`
- ❌ Criar `package.json` na raiz ainda (só quando o frontend entrar)

---

## 💻 Comandos do dia a dia

```bash
dotnet --version                                              # confirmar SDK 10.x
dotnet build                                                  # build da solution
dotnet test                                                   # rodar testes
dotnet new classlib -o src/Modules/<Nome>/<Nome>.Domain       # novo .csproj
dotnet sln add src/Modules/<Nome>/<Nome>.Domain/<Nome>.Domain.csproj
dotnet add <A>.csproj reference <B>.csproj                    # referência entre projetos
dotnet add package MediatR                                    # NuGet (jamais no SharedKernel)
```

---

## 🗂 Estado atual e ordem do MVP

**Hoje:** SharedKernel base implementado (`Entity`, `AggregateRoot`, `ValueObject`, `IDomainEvent`, hierarquia `AppError`, `Result`/`Result<T>` e interfaces transversais) — build limpo. Módulos ainda são pastas vazias. **Próximo passo (Aula 03):** Infrastructure base + middlewares globais (TenantContext, CurrentUser, ExceptionHandler).

> Antes de codar, **sempre conferir** `05 - Progresso/Estado Atual.md` e `06 - Próximos Passos/Próximos Passos.md` — o vault é fonte de verdade.

**Ordem do MVP:** SharedKernel + Infra base + middlewares (TenantContext, CurrentUser, ExceptionHandler) → Tenants → Identity → Clients + Sellers (UF) → Pipeline (Kanban) → ActivityLog → Reports básico → Frontend React + dnd-kit.

---

## 🔄 Fluxo de fim de sessão

```
1. git commit                          # hook post-commit sincroniza Obsidian
2. "Atualiza o Obsidian" ao Claude     # checagem manual antes de fechar
3. Fecha o VS Code
```

---

*Última revisão deste arquivo: 2026-05-06*
