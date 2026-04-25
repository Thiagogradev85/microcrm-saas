# sync-obsidian.ps1
# Sincroniza o projeto com o vault Obsidian.
#
# Modos:
#   ./scripts/sync-obsidian.ps1          → adiciona o último commit ao vault
#   ./scripts/sync-obsidian.ps1 -Full    → sync completo (commits + data do Estado Atual)
#
# Chamado automaticamente pelo git hook .git/hooks/post-commit
# e pelo Claude Code via PostToolUse hook (ver .claude/settings.json)

param([switch]$Full)

$VaultBase    = "C:\Users\thiag\Documents\Obsidian Vault\Micro-CRM-SAAS"
$CommitsFile  = Join-Path $VaultBase "05 - Progresso\Commits Importantes.md"
$EstadoFile   = Join-Path $VaultBase "05 - Progresso\Estado Atual.md"
$Today        = Get-Date -Format "yyyy-MM-dd"

# ---------------------------------------------------------
# 1. Adicionar último commit ao Obsidian
# ---------------------------------------------------------
$hash = (git rev-parse --short HEAD 2>&1).ToString().Trim()
$msg  = (git log -1 --pretty=%s 2>&1).ToString().Trim()

if (-not $hash -or $hash -match "fatal") {
    Write-Host "[Obsidian] Nao e um repositorio git ou nao ha commits. Abortando."
    exit 0
}

$content = [System.IO.File]::ReadAllText($CommitsFile, [System.Text.Encoding]::UTF8)

# Idempotente: nao adiciona se o hash ja constar no arquivo
if ($content.Contains($hash)) {
    Write-Host "[Obsidian] Commit $hash ja registrado. Nada a fazer."
} else {
    $newRow     = "| ``$hash`` | $msg | — |"
    $tableBlock = "| Hash | Mensagem | Significado |`n|------|----------|-------------|`n$newRow"
    $newSection = "`n`n## $Today`n`n$tableBlock"

    # Insere antes do bloco de template (marcador fixo no arquivo)
    $marker = "## Template para novos commits importantes"
    if ($content.Contains($marker)) {
        $content = $content.Replace($marker, "$newSection`n`n$marker")
    } else {
        # Fallback: appenda no final
        $content = $content.TrimEnd() + $newSection
    }

    [System.IO.File]::WriteAllText($CommitsFile, $content, [System.Text.Encoding]::UTF8)
    Write-Host "[Obsidian] Commit $hash adicionado em Commits Importantes.md"
}

# ---------------------------------------------------------
# 2. Full sync: atualiza data em Estado Atual.md
# ---------------------------------------------------------
if ($Full) {
    $estado = [System.IO.File]::ReadAllText($EstadoFile, [System.Text.Encoding]::UTF8)
    $estado = [regex]::Replace($estado, "## Data da ultima atualizacao: \d{4}-\d{2}-\d{2}", "## Data da ultima atualizacao: $Today")
    $estado = [regex]::Replace($estado, "## Data da última atualização: \d{4}-\d{2}-\d{2}", "## Data da última atualização: $Today")
    [System.IO.File]::WriteAllText($EstadoFile, $estado, [System.Text.Encoding]::UTF8)
    Write-Host "[Obsidian] Estado Atual.md — data atualizada para $Today"
    Write-Host "[Obsidian] Sync completo concluido."
}
