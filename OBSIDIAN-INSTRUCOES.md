# Instruções para Claude Code — Obsidian como Cérebro do Projeto

## Vault do Obsidian
**Caminho local:** `C:\Users\thiag\Documents\Obsidian Vault\Micro-CRM-SAAS\`

## Regra Principal
**Antes de iniciar qualquer sessão de trabalho** no projeto Micro CRM SaaS, consulte:
1. `C:\Users\thiag\Documents\Obsidian Vault\Micro-CRM-SAAS\00 - Índice.md` — hub central
2. `C:\Users\thiag\Documents\Obsidian Vault\Micro-CRM-SAAS\05 - Progresso\Estado Atual.md` — o que existe no código
3. `C:\Users\thiag\Documents\Obsidian Vault\Micro-CRM-SAAS\06 - Próximos Passos\Próximos Passos.md` — o que fazer

## Quando Atualizar o Obsidian
Atualize as notas relevantes **após cada sessão** quando:
- Arquivos .cs foram criados ou modificados → atualizar `Estado Atual.md`
- Um commit importante foi feito → adicionar em `Commits Importantes.md`
- Uma decisão estratégica foi tomada → registrar em `Decisões Estratégicas.md`
- O próximo passo mudou → atualizar `Próximos Passos.md`

## Estrutura do Vault

```
Micro-CRM-SAAS/
├── 00 - Índice.md                          ← HUB CENTRAL, ler primeiro
├── 01 - Projeto/
│   ├── Visão Geral.md                      ← produto, modelo de negócio, hierarquia de usuários
│   └── Stack Tecnológica.md                ← todas as tecnologias e versões
├── 02 - Arquitetura/
│   ├── Padrões e Convenções.md             ← naming, estrutura, padrões obrigatórios
│   ├── Módulos MVP.md                      ← bounded contexts e fases
│   └── SharedKernel.md                     ← tipos base, interfaces, regras da lib
├── 03 - Regras de Negócio/
│   ├── Regras Críticas.md                  ← invariantes que o código DEVE garantir
│   └── Pipeline e Status.md                ← 3 camadas: Kanban + Relacionamento + Tags
├── 04 - Decisões/
│   └── Decisões Estratégicas.md            ← decisões importantes + raciocínio
├── 05 - Progresso/
│   ├── Estado Atual.md                     ← fotografia do código hoje
│   └── Commits Importantes.md              ← histórico de commits relevantes
└── 06 - Próximos Passos/
    └── Próximos Passos.md                  ← o que fazer na próxima sessão
```

## Como usar no início de cada sessão
```
1. Leia: C:\Users\thiag\Documents\Obsidian Vault\Micro-CRM-SAAS\00 - Índice.md
2. Leia: Estado Atual.md
3. Leia: Próximos Passos.md
4. Trabalhe
5. Atualize as notas afetadas antes de terminar
```
