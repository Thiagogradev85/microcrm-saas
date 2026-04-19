# Micro CRM SaaS

## Visão geral

CRM vendido para empresas (B2B), modelo multitenant. Projeto **novo do zero** em .NET 10 — não é uma versão evoluída do projeto anterior em Node.js. O foco do MVP é ser vendável: ter funcionalidades suficientes para começar a abordar clientes reais.

**Repositório:** https://github.com/Thiagogradev85/microcrm-saas
**Documentação:** https://github.com/Thiagogradev85/microcrm-saas-docs (Docusaurus)
**Projeto anterior (Node.js, arquivado):** https://github.com/Thiagogradev85/Micro-CRM-SAAS

---

## Sobre o desenvolvedor

**Thiago Gramuglia** está aprendendo C# e .NET do zero enquanto constrói este projeto.

- **Background:** tem experiência com Node.js e React
- **Nível em .NET:** iniciante
- **Ambiente:** VS Code + Claude Code (casa), dotnetfiddle.net (trabalho)
- **CNPJ:** 64.828.611/0001-05

### Implicações para a geração de código

- **Priorize clareza sobre concisão.** Código enxuto demais e uso pesado de açúcar sintático (pattern matching avançado, LINQ muito encadeado, records com features novas sem explicar) dificultam o aprendizado.
- **Comente decisões conceituais quando aparecer algo novo.** "Por que `sealed`?", "Por que `private set`?", "O que é `init`?". Não precisa ser um tutorial, mas um comentário de 1–2 linhas quando introduzir um conceito ajuda.
- **Quando houver duas formas de escrever a mesma coisa**, explique rapidamente por que escolheu uma em detrimento da outra.
- **Antes de refatorar algo grande, explique a motivação em texto primeiro**, depois aplique a mudança.
- Thiago escreve/lê em **português**. Mensagens de commit, docs e comentários devem ser em português. Identificadores de código (classes, métodos, variáveis) ficam em inglês por convenção .NET.

---

## Stack

### Backend
- **.NET 10 LTS** (SDK travado via `global.json` em 10.0.202)
- **ASP.NET Core 10** (Minimal APIs para endpoints, salvo exceção)
- **EF Core 10** com filtro global por `TenantId` (via `HasQueryFilter`)
- **MediatR** — padrão CQRS, um handler por Command/Query
- **FluentValidation** — validators por Command
- **MassTransit 8.x** — abstração sobre RabbitMQ
- **Serilog** — logs estruturados em JSON
- **OpenTelemetry** — traces e métricas
- **Swagger/OpenAPI** — documentação dos endpoints
- **xUnit + FluentAssertions + Moq** — testes unitários e de integração

### Frontend (fase posterior)
- **React 18 + Vite** + **TypeScript**
- **Tailwind CSS** (design system minimalista)
- **dnd-kit** — drag-and-drop do Kanban (NÃO usar react-beautiful-dnd, descontinuado)
- **TanStack Query** — cache, sincronização e invalidação de dados do servidor
- **Zod** — validação de schemas tipada
- **React Hook Form** — formulários performáticos

### Infraestrutura (free tier para MVP)
- **PostgreSQL** via Neon (serverless)
- **RabbitMQ** via CloudAMQP (1M msgs/mês)
- **Redis** via Upstash (cache, rate limit, locks distribuídos)
- **Deploy** via Render
- **CI/CD** via GitHub Actions

---

## Arquitetura

### Padrão: Monolito Modular + DDD

Um único executável/deploy, porém internamente dividido em **Bounded Contexts** (módulos) com forte isolamento. Cada módulo tem seu próprio banco/schema e se comunica com os outros **apenas via Domain Events** publicados no RabbitMQ (MassTransit). Proibido chamada direta entre módulos.

### Estrutura de projetos (.csproj)

```
microcrm-saas/
├── global.json                         # SDK do .NET travado
├── MicroCrm.sln                        # Solution mestre
├── CLAUDE.md                           # Este arquivo
├── .claudeignore                       # Arquivos que o Claude Code ignora
├── .gitignore
├── README.md
└── src/
    ├── SharedKernel/                   # Abstrações puras (sem dependências externas)
    │   └── SharedKernel.csproj
    └── Modules/
        ├── Tenants/
        │   ├── Tenants.Domain/         # Entidades, ValueObjects, eventos
        │   ├── Tenants.Application/    # Handlers, DTOs, validators
        │   ├── Tenants.Infrastructure/ # EF Core, repositórios, integrações externas
        │   └── Tenants.Api/            # Endpoints Minimal API
        ├── Identity/                   # mesma estrutura
        ├── Clients/
        ├── Sellers/
        ├── Pipeline/
        ├── ActivityLog/
        └── Reports/
```

### SharedKernel (zero dependências externas)

Contém **apenas abstrações puras** reutilizadas por todos os módulos:

- **Tipos base:** `Entity`, `AggregateRoot`, `ValueObject`
- **Hierarquia de erros (AppError):**
  - `NotFoundError` → HTTP 404
  - `ValidationError` → HTTP 422
  - `UnauthorizedError` → HTTP 403
  - `DuplicateError` → HTTP 409
  - `PlanLimitError` → HTTP 402
  - `DomainError` → HTTP 400 (genérico)
- **Interfaces transversais:** `ITenantContext`, `ICurrentUser`, `IRepository<T>`, `IUnitOfWork`, `IEventBus`, `IDomainEvent`

**Regra rígida:** `SharedKernel.csproj` NUNCA deve referenciar pacotes NuGet externos (EF, MediatR, etc.). Apenas `System.*`.

---

## Módulos (Bounded Contexts)

| # | Módulo | Fase | Responsabilidade |
|---|---|---|---|
| 1 | Tenants | MVP | Empresas clientes, planos (Free/Starter/Pro), limites |
| 2 | Identity | MVP | Usuários, roles (SuperAdmin/TenantAdmin/User), permissões |
| 3 | Clients | MVP | Leads e clientes (núcleo do CRM) |
| 4 | Sellers | MVP | Vendedores, territórios por UF, exclusividade |
| 5 | Pipeline | MVP | Kanban visual com drag-and-drop |
| 6 | ActivityLog | MVP | Linha do tempo automática |
| 7 | Reports | MVP | Dashboard e relatórios básicos |
| 8 | Goals | Fase 2 | Metas mensais por vendedor |
| 9 | Catalog | Fase 2 | Catálogos e produtos em PDF |
| 10 | Messaging | Fase 2 | Campanhas e-mail e WhatsApp (Saga MassTransit) |
| 11 | Prospecting | Fase 3 | Busca via Google Maps e APIs externas |
| 12 | Settings | Fase 2 | Configurações por tenant |

---

## Pipeline e Status — 3 camadas independentes

**Muito importante:** o pipeline não tem um único campo "status". São **três conceitos separados** que coexistem:

### 1. Estágio do Kanban (coluna)
Campo enum representando onde o cliente está no funil. Colunas padrão:
1. Novo Lead
2. Em Contato
3. Qualificado
4. Proposta Enviada
5. Em Negociação
6. Ganhou ✅ (final)
7. Perdeu ❌ (final)

Fixas no MVP, configuráveis pelo TenantAdmin na Fase 2.

### 2. Status de Relacionamento
Campo **separado** do estágio do Kanban. Enum:
`lead | customer | inactive | partner | archived | blocked`

### 3. Tags (múltiplas simultâneas)
Lista de strings. Padrão:
`Catálogo Enviado`, `WhatsApp Ativo`, `Sem UF`, `VIP`, `Indicação`, `Feira/Evento`, `Aguardando Retorno`, `Follow-up Urgente`.

---

## Hierarquia de usuários

- **SuperAdmin** (Thiago) — cria/gerencia tenants, altera planos, acesso global
- **TenantAdmin** — administra a empresa cliente (usuários, vendedores, configs)
- **User** — operação do CRM dentro das permissões definidas pelo TenantAdmin

---

## Regras de negócio críticas

Estas são **invariantes** — o código DEVE garantir:

1. **Isolamento por tenant absoluto.** Toda query EF Core que toca tabela tenant-owned passa por `HasQueryFilter(x => x.TenantId == _tenantContext.CurrentTenantId)`. Nunca confiar em filtros no application layer.
2. **UF obrigatória para clientes.** Cliente sem UF recebe automaticamente a tag `Sem UF` e vai para fila laranja. Não bloqueia cadastro, mas sinaliza visualmente.
3. **Exclusividade de UF.** 1 vendedor por UF por tenant. Tentativa de violar gera `DuplicateError`.
4. **Ganhou/Perdeu são estados finais.** Reabertura exige ação explícita e registro no ActivityLog com motivo.
5. **Bloqueado impede campanhas.** Cliente com status `blocked` nunca recebe e-mail, WhatsApp ou qualquer mensagem automatizada.
6. **Arquivado = soft delete.** Some do Kanban mas persiste no banco. Queries normais filtram out; queries administrativas podem incluir.
7. **Idempotência de eventos.** Mesmo Domain Event processado duas vezes NÃO pode gerar efeito duplicado. Handlers devem conferir estado antes de aplicar.
8. **Último TenantAdmin não sai.** Tentativa de remover o último TenantAdmin de um tenant é bloqueada até que outro seja promovido.

---

## Convenções de código

### Nomenclatura
- **Classes, records, structs, enums, métodos públicos:** `PascalCase` (`ClientAggregate`, `CreateClientCommand`)
- **Variáveis locais, parâmetros:** `camelCase` (`tenantId`, `clientName`)
- **Campos privados:** `_camelCase` com underscore (`_repository`, `_eventBus`)
- **Constantes:** `PascalCase` ou `UPPER_CASE` quando for constante pública
- **Interfaces:** prefixo `I` (`ITenantContext`, `IRepository<T>`)

### Arquivos e pastas
- Um tipo por arquivo. Nome do arquivo = nome do tipo (`Client.cs` contém `class Client`)
- Namespaces espelham a estrutura de pastas
- Agregados ficam em pasta própria dentro do Domain: `Domain/Clients/Client.cs`, `Domain/Clients/ClientEmail.cs`

### Padrões obrigatórios
- **Toda entidade de negócio herda de `Entity` ou `AggregateRoot` do SharedKernel.**
- **ValueObjects são imutáveis** — sem setters públicos, igualdade por valor (herdam de `ValueObject` base).
- **Construtor de domínio é privado** (`private Client() { }` para o EF) e instâncias são criadas via **factory methods estáticos** (`Client.Create(...)`) que validam invariantes.
- **Commands retornam `Result<T>`** ou lançam `AppError`. Nunca lançam `Exception` genérica.
- **Handlers MediatR** ficam em `Application/Commands/<Feature>/<FeatureCommandHandler>.cs`.

### Testes
- Estrutura espelha o src: `tests/Modules/Clients.Tests/...`
- Nome do teste: `MethodName_StateUnderTest_ExpectedBehavior`
- Padrão AAA (Arrange / Act / Assert), com linha em branco separando cada bloco
- Preferência por testar agregados e handlers. Controllers/Endpoints não são testados isoladamente — ficam nos testes de integração.

---

## Comandos do dia a dia

```bash
# Verificar versão do SDK (deve retornar 10.x)
dotnet --version

# Restaurar pacotes (raramente necessário, build faz sozinho)
dotnet restore

# Build da solution inteira
dotnet build

# Rodar testes
dotnet test

# Criar novo projeto class library
dotnet new classlib -o src/Modules/<Nome>/<Nome>.Domain

# Adicionar projeto à solution
dotnet sln add src/Modules/<Nome>/<Nome>.Domain/<Nome>.Domain.csproj

# Adicionar referência entre projetos
dotnet add <A>.csproj reference <B>.csproj

# Adicionar pacote NuGet
dotnet add package MediatR
```

---

## Fase atual do projeto

**Onde estamos:** setup inicial do backend .NET. Pastas criadas, `global.json` e `MicroCrm.sln` ainda não criados. Nenhum `.csproj` existe ainda.

**Próximo passo imediato:** criar `global.json`, criar `MicroCrm.sln`, criar `SharedKernel.csproj` e implementar as classes base (`Entity`, `AggregateRoot`, `ValueObject`) — isso corresponde à Aula 02 do aprendizado de .NET.

### Ordem de implementação do MVP

1. **SharedKernel** + Infrastructure base + middlewares globais (TenantContext, CurrentUser, ExceptionHandler)
2. Módulo **Tenants**
3. Módulo **Identity**
4. Módulo **Clients** + **Sellers** (ficam conectados por UF)
5. Módulo **Pipeline** (Kanban) — diferencial visual do produto
6. Módulo **ActivityLog**
7. Módulo **Reports** básico
8. Frontend React com Kanban (dnd-kit)

### Fases futuras

- **Fase 2 (expansão operacional):** Goals + Catalog + Messaging + Settings
- **Fase 3 (diferenciais competitivos):** Prospecting + IA/enriquecimento + Reports avançados

---

## O que NÃO fazer

- Não usar `async void` exceto em event handlers do framework
- Não fazer chamadas diretas entre módulos — usar Domain Events
- Não colocar lógica de negócio em controllers/endpoints
- Não acessar `DbContext` direto no Application — passar por repositório
- Não usar `DateTime.Now` — sempre `DateTime.UtcNow` ou injetar `IClock`
- Não adicionar pacotes NuGet ao `SharedKernel.csproj`
- Não usar `react-beautiful-dnd` no frontend (descontinuado) — é `dnd-kit`
- Não criar `package.json` na raiz do repositório ainda (só quando o frontend entrar)
