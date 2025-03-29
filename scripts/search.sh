#!/bin/bash

# search.sh
# Uso: ./scripts/search.sh "texto_procurado" ["extensão"] ["diretório"]
# Exemplos:
#   ./scripts/search.sh "DataBaseWS"
#   ./scripts/search.sh "create_logger" "py" "src"
#   ./scripts/search.sh "API_URL" "env"

SEARCH_TEXT="$1"
FILE_EXT="${2:-*}"  # Se não especificado, procura em todos os arquivos
SEARCH_DIR="${3:-.}" # Se não especificado, procura no diretório atual

# Cores para melhor visualização
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
NC='\033[0m' # No Color

# Função para mostrar o uso do script
show_help() {
    echo -e "${BLUE}Busca em Arquivos - NetScout ArborWS${NC}"
    echo -e "${YELLOW}Uso:${NC}"
    echo "  ./scripts/search.sh \"texto_procurado\" [\"extensão\"] [\"diretório\"]"
    echo -e "${YELLOW}Exemplos:${NC}"
    echo "  ./scripts/search.sh \"DataBaseWS\"          # Busca em todos os arquivos"
    echo "  ./scripts/search.sh \"create_logger\" \"py\" # Busca apenas em arquivos .py"
    echo "  ./scripts/search.sh \"API_URL\" \"env\"      # Busca em arquivos .env"
    echo -e "${YELLOW}Extensões comuns:${NC}"
    echo "  py    - Arquivos Python"
    echo "  env   - Arquivos de ambiente"
    echo "  md    - Arquivos Markdown"
    echo "  sh    - Scripts Shell"
    echo "  sql   - Arquivos SQL"
    echo "  json  - Arquivos JSON"
    echo "  *     - Todos os arquivos"
}

# Se nenhum argumento foi fornecido ou -h/--help foi usado
if [ -z "$1" ] || [ "$1" = "-h" ] || [ "$1" = "--help" ]; then
    show_help
    exit 0
fi

echo -e "${YELLOW}Procurando por '${SEARCH_TEXT}' em arquivos *.$FILE_EXT no diretório $SEARCH_DIR${NC}"
echo -e "${PURPLE}----------------------------------------${NC}"

# Diretórios para ignorar
IGNORE_DIRS="venv|__pycache__|.git|.pytest_cache|*.pyc|*.pyo|*.pyd|logs|data/import"

if [ "$FILE_EXT" = "*" ]; then
    # Procura em todos os arquivos, excluindo diretórios especificados
    find "$SEARCH_DIR" -type f \
        ! -path "*/$IGNORE_DIRS/*" \
        -exec grep -l "$SEARCH_TEXT" {} \; | \
    while read -r file; do
        echo -e "${GREEN}Arquivo:${NC} $file"
        echo -e "${BLUE}Linha(s):${NC}"
        grep -n --color=always "$SEARCH_TEXT" "$file"
        echo -e "${PURPLE}----------------------------------------${NC}"
    done
else
    # Procura apenas em arquivos com a extensão especificada
    find "$SEARCH_DIR" -type f -name "*.$FILE_EXT" \
        ! -path "*/$IGNORE_DIRS/*" \
        -exec grep -l "$SEARCH_TEXT" {} \; | \
    while read -r file; do
        echo -e "${GREEN}Arquivo:${NC} $file"
        echo -e "${BLUE}Linha(s):${NC}"
        grep -n --color=always "$SEARCH_TEXT" "$file"
        echo -e "${PURPLE}----------------------------------------${NC}"
    done
fi

# Contagem total de ocorrências
TOTAL_FILES=$(find "$SEARCH_DIR" -type f -name "*.$FILE_EXT" ! -path "*/$IGNORE_DIRS/*" -exec grep -l "$SEARCH_TEXT" {} \; | wc -l)
TOTAL_OCCURRENCES=$(find "$SEARCH_DIR" -type f -name "*.$FILE_EXT" ! -path "*/$IGNORE_DIRS/*" -exec grep -o "$SEARCH_TEXT" {} \; | wc -l)

echo -e "${YELLOW}Resumo da busca:${NC}"
echo -e "  Arquivos encontrados: ${GREEN}$TOTAL_FILES${NC}"
echo -e "  Total de ocorrências: ${GREEN}$TOTAL_OCCURRENCES${NC}"