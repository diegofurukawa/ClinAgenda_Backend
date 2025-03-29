#!/bin/bash

# Diretórios fixos
SOURCE_DIR="$HOME/GitHub/ClinAgendaBootcamp"
DEST_DIR="$HOME/GitHub/ClinAgendaBootcamp/backup"


# Verifica se o diretório fonte existe
if [ ! -d "$SOURCE_DIR" ]; then
    echo "Erro: Diretório fonte não encontrado: $SOURCE_DIR"
    exit 1
fi

# Cria o diretório de destino se não existir
mkdir -p "$DEST_DIR"

# Cria o timestamp para o nome do backup
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
BACKUP_DIR="$DEST_DIR/backup_ClinAgenda_$TIMESTAMP"

# Cria o diretório de backup
mkdir -p "$BACKUP_DIR"

# Lista de arquivos e diretórios para ignorar
EXCLUDE=(
    "bin"
    "obj"
    "Properties"
    "node_modules"
    "dist"
    "build"
    ".git"
    ".DS_Store"
    "__pycache__"
    "*.log"
    ".env"
    "backup"
    ".venv"
    "venv"
    "logs"
)

# Função para realizar o backup
do_backup() {
    echo "Iniciando backup..."
    echo "Origem: $SOURCE_DIR"
    echo "Destino: $BACKUP_DIR"
    
    # Padrão de exclusão para o find
    EXCLUDE_PATTERN=""
    for item in "${EXCLUDE[@]}"; do
        EXCLUDE_PATTERN="$EXCLUDE_PATTERN -not -path '*/$item/*' -not -name '$item'"
    done

    # Encontra todos os arquivos e copia para o diretório de backup
    eval "find '$SOURCE_DIR' -type f $EXCLUDE_PATTERN -exec cp {} '$BACKUP_DIR/' \;"
    
    # Verifica se a cópia foi bem sucedida
    if [ $? -eq 0 ]; then
        echo -e "\nBackup concluído com sucesso!"
        
        # Cria arquivo de log
        LOG_FILE="$BACKUP_DIR/backup_log.txt"
        echo "Backup realizado em: $(date)" > "$LOG_FILE"
        echo "Diretório fonte: $SOURCE_DIR" >> "$LOG_FILE"
        echo "Diretório destino: $BACKUP_DIR" >> "$LOG_FILE"
        
        # Lista os arquivos incluídos no backup
        echo -e "\nArquivos incluídos no backup:" >> "$LOG_FILE"
        ls -1 "$BACKUP_DIR" >> "$LOG_FILE"
    else
        echo "Erro durante o backup!"
        rm -rf "$BACKUP_DIR"
        exit 1
    fi
}

# Executa o backup
do_backup

echo -e "\nOs arquivos foram salvos em: $BACKUP_DIR"
echo "Um log do backup foi criado em: $BACKUP_DIR/backup_log.txt"