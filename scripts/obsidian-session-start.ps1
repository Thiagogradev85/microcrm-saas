# obsidian-session-start.ps1
# Hook SessionStart do Claude Code.
# Le Estado Atual e Proximos Passos do vault Obsidian e injeta como contexto
# adicional no inicio de cada sessao.

$VaultBase = "C:\Users\thiag\Documents\Obsidian Vault\Micro-CRM-SAAS"

# Busca por prefixo numerico para evitar problemas de encoding com acentos no path
function Get-NoteFile ($dirPrefix, $filePattern) {
    $dir = Get-ChildItem -Path $VaultBase -Directory -ErrorAction SilentlyContinue |
           Where-Object { $_.Name -like "$dirPrefix*" } |
           Select-Object -First 1 -ExpandProperty FullName

    if (-not $dir) { return "(pasta $dirPrefix nao encontrada em $VaultBase)" }

    $file = Get-ChildItem -Path $dir -Filter "$filePattern" -ErrorAction SilentlyContinue |
            Select-Object -First 1 -ExpandProperty FullName

    if (-not $file) { return "(arquivo $filePattern nao encontrado em $dir)" }

    try {
        return [System.IO.File]::ReadAllText($file, [System.Text.Encoding]::UTF8)
    } catch {
        return "(erro ao ler $file : $_)"
    }
}

$estado = Get-NoteFile "05" "Estado*"
$passos = Get-NoteFile "06" "*.md"

$contexto = "=== MEMORIA DO PROJETO - OBSIDIAN (sessao iniciada) ===`n`n--- ESTADO ATUAL ---`n$estado`n`n--- PROXIMOS PASSOS ---`n$passos`n`n=== FIM DA MEMORIA DO OBSIDIAN ==="

$output = [ordered]@{
    hookSpecificOutput = [ordered]@{
        hookEventName     = "SessionStart"
        additionalContext = $contexto
    }
}

$output | ConvertTo-Json -Depth 3 -Compress
