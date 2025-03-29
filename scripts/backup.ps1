# PowerShell script para backup do ClinAgendaBootcamp
# Com todos os arquivos em uma única pasta (estrutura plana)

# Diretorios fixos
$SOURCE_DIR = "$env:USERPROFILE\GitHub\ClinAgenda"
$DEST_DIR = "$env:USERPROFILE\GitHub\BKPs\ClinAgenda"

# Verifica se o diretorio fonte existe
if (-not (Test-Path $SOURCE_DIR)) {
    Write-Host "Erro: Diretorio fonte nao encontrado: $SOURCE_DIR" -ForegroundColor Red
    exit 1
}

# Cria o diretorio de destino se nao existir
if (-not (Test-Path $DEST_DIR)) {
    New-Item -Path $DEST_DIR -ItemType Directory -Force | Out-Null
    Write-Host "Diretorio de destino criado: $DEST_DIR" -ForegroundColor Green
}

# Cria o timestamp para o nome do backup
$TIMESTAMP = Get-Date -Format "yyyyMMdd_HHmmss"
$BACKUP_DIR = "$DEST_DIR\backup_ClinAgenda_$TIMESTAMP"

# Cria o diretorio de backup
New-Item -Path $BACKUP_DIR -ItemType Directory -Force | Out-Null

# Lista de padrões para arquivos e diretorios a ignorar
$EXCLUDE_DIRS = @(
    "bin"
    "obj"
    "Properties"
    "node_modules"
    "dist"
    "build"
    ".git"
    ".DS_Store"
    "__pycache__"
    "backup"
    ".venv"
    "venv"
    "logs"
)

$EXCLUDE_FILES = @(
    "*.log"
    ".env"
)

# Funçao para realizar o backup com estrutura plana (todos os arquivos em uma pasta)
function Do-FlatBackup {
    Write-Host "Iniciando backup com estrutura plana..." -ForegroundColor Cyan
    Write-Host "Origem: $SOURCE_DIR" -ForegroundColor Cyan
    Write-Host "Destino: $BACKUP_DIR" -ForegroundColor Cyan
    
    $fileCount = 0
    $errorCount = 0
    $successFiles = @()
    $errorFiles = @()

    # Obter todos os arquivos recursivamente, excluindo diretorios e arquivos indesejados
    $files = Get-ChildItem -Path $SOURCE_DIR -File -Recurse | Where-Object {
        $file = $_
        $shouldInclude = $true
        
        # Verificar se o arquivo está em um diretorio excluido
        foreach ($dir in $EXCLUDE_DIRS) {
            if ($file.FullName -like "*\$dir\*") {
                $shouldInclude = $false
                break
            }
        }
        
        # Verificar se o arquivo corresponde a um padrao excluido
        if ($shouldInclude) {
            foreach ($pattern in $EXCLUDE_FILES) {
                if ($file.Name -like $pattern) {
                    $shouldInclude = $false
                    break
                }
            }
        }
        
        return $shouldInclude
    }

    $totalFiles = $files.Count
    Write-Host "Total de arquivos a copiar: $totalFiles" -ForegroundColor Cyan

    # Copiar cada arquivo para o diretorio de backup mantendo apenas o nome do arquivo
    foreach ($file in $files) {
        $destFile = Join-Path -Path $BACKUP_DIR -ChildPath $file.Name
        
        # Se já existe um arquivo com o mesmo nome, adicionar um sufixo numérico
        if (Test-Path $destFile) {
            $counter = 1
            $fileNameWithoutExt = [System.IO.Path]::GetFileNameWithoutExtension($file.Name)
            $fileExt = [System.IO.Path]::GetExtension($file.Name)
            
            while (Test-Path $destFile) {
                $newFileName = "${fileNameWithoutExt}_${counter}${fileExt}"
                $destFile = Join-Path -Path $BACKUP_DIR -ChildPath $newFileName
                $counter++
            }
        }
        
        try {
            Copy-Item -Path $file.FullName -Destination $destFile -Force -ErrorAction Stop
            $fileCount++
            $successFiles += $file.Name
            
            # Mostrar progresso
            if ($fileCount % 10 -eq 0 -or $fileCount -eq $totalFiles) {
                Write-Host "Progresso: $fileCount/$totalFiles arquivos copiados" -ForegroundColor Green
            }
        } catch {
            $errorCount++
            $errorFiles += "$($file.FullName): $($_.Exception.Message)"
            Write-Host "Erro ao copiar $($file.FullName): $($_.Exception.Message)" -ForegroundColor Yellow
        }
    }
    
    # Verifica se a copia foi bem sucedida
    if ($fileCount -gt 0) {
        Write-Host "`nBackup concluido!" -ForegroundColor Green
        Write-Host "Arquivos copiados: $fileCount de $totalFiles" -ForegroundColor Green
        
        if ($errorCount -gt 0) {
            Write-Host "Atençao: $errorCount arquivos nao puderam ser copiados." -ForegroundColor Yellow
        }
        
        # Cria arquivo de log
        $LOG_FILE = "$BACKUP_DIR\backup_log.txt"
        "Backup realizado em: $(Get-Date)" | Out-File -FilePath $LOG_FILE -Force
        "Diretorio fonte: $SOURCE_DIR" | Out-File -FilePath $LOG_FILE -Append
        "Diretorio destino: $BACKUP_DIR" | Out-File -FilePath $LOG_FILE -Append
        "Total de arquivos copiados: $fileCount" | Out-File -FilePath $LOG_FILE -Append
        
        # Lista os arquivos incluidos no backup
        "`nArquivos incluidos no backup:" | Out-File -FilePath $LOG_FILE -Append
        $successFiles | Sort-Object | Out-File -FilePath $LOG_FILE -Append
        
        # Lista os arquivos que falharam (se houver)
        if ($errorCount -gt 0) {
            "`nArquivos que nao puderam ser copiados:" | Out-File -FilePath $LOG_FILE -Append
            $errorFiles | Out-File -FilePath $LOG_FILE -Append
        }
        
        return $true
    } else {
        Write-Host "Erro: Nenhum arquivo foi copiado!" -ForegroundColor Red
        if (Test-Path $BACKUP_DIR) {
            Remove-Item -Path $BACKUP_DIR -Recurse -Force -ErrorAction SilentlyContinue
        }
        return $false
    }
}

# Executa o backup
$result = Do-FlatBackup

if ($result) {
    Write-Host "`nOs arquivos foram salvos em: $BACKUP_DIR" -ForegroundColor Green
    Write-Host "Um log do backup foi criado em: $BACKUP_DIR\backup_log.txt" -ForegroundColor Green
} else {
    Write-Host "`nO backup falhou." -ForegroundColor Red
    
    # Tenta limpar o diretorio de backup em caso de falha
    if (Test-Path $BACKUP_DIR) {
        Write-Host "Removendo diretorio de backup incompleto..." -ForegroundColor Yellow
        Remove-Item -Path $BACKUP_DIR -Recurse -Force -ErrorAction SilentlyContinue
    }
}