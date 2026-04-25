param([switch]$Full)

$VaultBase   = "C:\Users\thiag\Documents\Obsidian Vault\Micro-CRM-SAAS"
$CommitsFile = Join-Path $VaultBase "05 - Progresso\Commits Importantes.md"
$EstadoFile  = Join-Path $VaultBase "05 - Progresso\Estado Atual.md"
$Today       = Get-Date -Format "yyyy-MM-dd"

$hash = (git rev-parse --short HEAD 2>&1).ToString().Trim()
$msg  = (git log -1 --pretty=%s 2>&1).ToString().Trim()

if (-not $hash -or $hash -match "fatal") {
    Write-Host "[Obsidian] Nao e um repo git ou nao ha commits."
    exit 0
}

$content = [System.IO.File]::ReadAllText($CommitsFile, [System.Text.Encoding]::UTF8)

if ($content.Contains($hash)) {
    Write-Host "[Obsidian] Commit $hash ja registrado."
} else {
    $newRow = "| `` $hash `` | $msg | - |"

    $sep     = "|------|----------|-------------|"
    $header  = "| Hash | Mensagem | Significado |"
    $section = "`n`n## $Today`n`n$header`n$sep`n$newRow"

    $marker = "## Template para novos commits importantes"
    if ($content.Contains($marker)) {
        $content = $content.Replace($marker, "$section`n`n$marker")
    } else {
        $content = $content.TrimEnd() + $section
    }

    [System.IO.File]::WriteAllText($CommitsFile, $content, [System.Text.Encoding]::UTF8)
    Write-Host "[Obsidian] Commit $hash adicionado."
}

if ($Full) {
    $estado = [System.IO.File]::ReadAllText($EstadoFile, [System.Text.Encoding]::UTF8)
    $estado = [regex]::Replace($estado, "Data da ultima atualizacao: \d{4}-\d{2}-\d{2}", "Data da ultima atualizacao: $Today")
    [System.IO.File]::WriteAllText($EstadoFile, $estado, [System.Text.Encoding]::UTF8)
    Write-Host "[Obsidian] Estado Atual.md atualizado para $Today"
}
