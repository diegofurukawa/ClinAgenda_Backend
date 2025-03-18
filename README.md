# ClinAgenda - Sistema de Agendamento para Clínicas

ClinAgenda é um sistema de agendamento para clínicas médicas desenvolvido com ASP.NET Core no backend e Vue.js no frontend. O projeto segue a arquitetura Clean Architecture e utiliza MySQL como banco de dados.

## 🔍 Visão Geral

O sistema permite o gerenciamento completo de agendamentos médicos, incluindo:
- Cadastro de médicos e suas especialidades
- Cadastro de pacientes
- Agendamento de consultas
- Gestão de disponibilidade

## 👥 Atores do Sistema

- **Secretário**: Responsável pelo gerenciamento de agendamentos
- **Paciente**: Usuário final que agenda consultas
- **Médico**: Profissional que atende os pacientes

## 🛠️ Tecnologias Utilizadas

### Backend
- ASP.NET Core 8.0
- C# como linguagem de programação
- MySQL 8.0.41 como banco de dados

### Frontend
- Vue.js
- Node.js
- Yarn como gerenciador de pacotes

## 📊 Estrutura do Banco de Dados

O sistema utiliza um banco de dados MySQL com as seguintes tabelas principais:

- **Status**: Armazena estados para médicos e pacientes
- **Specialty**: Armazena as especialidades médicas disponíveis
- **Doctor**: Armazena os dados dos médicos
- **Patient**: Armazena os dados dos pacientes
- **Doctor_Specialty**: Relaciona médicos com suas especialidades
- **Appointment**: Armazena os agendamentos de consultas

## 🏗️ Arquitetura do Projeto (Clean Architecture)

O projeto segue a arquitetura limpa com as seguintes camadas:

- **Core**: Contém a lógica de negócio, entidades e interfaces
- **Application**: Contém os casos de uso, serviços de aplicação e DTOs
- **Infrastructure**: Implementa detalhes técnicos, como repositórios e acesso a banco de dados
- **Presentation**: Contém os controllers e a configuração da WebAPI

### Estrutura de Diretórios

```
ClinAgendaAPI/
├── src/
│   ├── ClinAgendaAPI.Core/                 # Camada Core
│   │   ├── Entities/                       # Entidades do Domínio
│   │   ├── Interfaces/                     # Interfaces de Repositório e Serviços
│   ├── ClinAgendaAPI.Application/          # Camada Application (Casos de Uso)
│   │   ├── DTOs/                           # Data Transfer Objects
│   │   └── UseCases/                       # Casos de Uso
│   ├── ClinAgendaAPI.Infrastructure/       # Camada Infrastructure (Implementações Técnicas)
│   │   ├── Repositories/                   # Implementações de Repositórios
│   └── ClinAgendaAPI.WebAPI/               # Camada Presentation (WebAPI)
│       ├── Controllers/                    # Controllers da API
│       └── appsettings.json                # Configurações da Aplicação
```

## 💻 Configuração do Ambiente de Desenvolvimento

### Requisitos para Windows

1. Instalar o ASP.NET e .NET 8.0 Runtime: https://dotnet.microsoft.com/pt-br/download/dotnet/8.0
2. MySQL Workbench: https://dev.mysql.com/downloads/workbench/
3. MySQL 8.0.41: https://dev.mysql.com/downloads/mysql/
4. VS Code: https://code.visualstudio.com/download
5. Extensões do VS Code:
   - C#
   - C# Dev Kit
   - .NET Install Tool
   - C# Extensions
6. Node.js: https://nodejs.org/pt/download
7. Yarn: `npm install --global yarn`
8. Extensões para Frontend:
   - Volar (Vue Language Features)
   - ESLint
   - Prettier
   - Prettier ESLint

### Requisitos para Linux (Debian 12)

#### Instalação do .NET

```bash
wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Instalar o SDK
sudo apt-get update && sudo apt-get install -y dotnet-sdk-8.0

# Instalar o Runtime
sudo apt-get update && sudo apt-get install -y aspnetcore-runtime-8.0
```

#### Instalação do MySQL no Linux

1. Atualizar repositórios: `sudo apt update && sudo apt upgrade`
2. Instalar wget: `sudo apt install wget`
3. Baixar e adicionar repositório MySQL:
   ```bash
   wget https://dev.mysql.com/get/mysql-apt-config_0.8.33-1_all.deb
   sudo apt install ./mysql-apt-config_0.8.33-1_all.deb
   ```
4. Instalar MySQL Server:
   ```bash
   sudo apt update
   sudo apt install mysql-server
   ```

### Cliente de Banco de Dados (Opcional)

Você pode escolher entre:
- MySQL Workbench: https://dev.mysql.com/downloads/workbench/
- dBeaver: https://dbeaver.io/download/
- Beekeeper Studio: https://www.beekeeperstudio.io/get

## 🚀 Inicializando o Projeto

### Configuração do Banco de Dados

1. Execute o script SQL fornecido para criação do banco de dados:
   ```bash
   mysql -u root -p < script_criacao_banco.sql
   ```

2. Verifique se o banco `clinagenda_database` foi criado corretamente.

### Backend

1. Clone o repositório:
   ```bash
   git clone [URL-DO-REPOSITÓRIO]
   cd ClinAgenda
   ```

2. Restaure as dependências e execute o projeto:
   ```bash
   dotnet restore
   cd src/ClinAgendaAPI.WebAPI
   dotnet run
   ```

3. A API estará disponível em: `https://localhost:5001`

### Frontend

1. Navegue até o diretório do frontend:
   ```bash
   cd frontend
   ```

2. Instale as dependências:
   ```bash
   yarn install
   ```

3. Execute o projeto:
   ```bash
   yarn serve
   ```

4. O frontend estará disponível em: `http://localhost:8080`

## 📝 Licença

Este projeto está licenciado sob a [Licença MIT](LICENSE).

## 👥 Contribuição

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues e pull requests.

## 📧 Contato

Para dúvidas ou sugestões, entre em contato com a equipe de desenvolvimento.
