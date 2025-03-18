# Projeto ClinAgenda - Sistema de Agendamento para Clínicas

## Visão Geral

ClinAgenda é um sistema de agendamento para clínicas médicas desenvolvido com ASP.NET Core no backend e Vue.js no frontend. O projeto segue a arquitetura Clean Architecture e utiliza MySQL como banco de dados.

## Atores do Sistema

- Secretário: Responsável pelo gerenciamento de agendamentos
- Paciente: Usuário final que agenda consultas
- Doutora/Médico: Profissional que atende os pacientes

## Tecnologias Utilizadas

### Backend

- ASP.NET Core 8.0
- C# como linguagem de programação
- MySQL 8.0.41 como banco de dados

### Frontend

- Vue.js
- Node.js
- Yarn como gerenciador de pacotes

## Estrutura do Banco de Dados

### Tabelas Principais

#### Status

Armazena os diferentes status para médicos e pacientes.

| Coluna | Tipo         | Descrição                |
| ------ | ------------ | ------------------------ |
| id     | int          | Identificador único (PK) |
| name   | varchar(255) | Nome do status           |

#### Specialty (Especialidade)

Armazena as especialidades médicas disponíveis.

| Coluna           | Tipo         | Descrição                          |
| ---------------- | ------------ | ---------------------------------- |
| id               | int          | Identificador único (PK)           |
| name             | varchar(255) | Nome da especialidade              |
| scheduleDuration | int          | Duração em minutos de uma consulta |

#### Doctor (Médico)

Armazena os dados dos médicos.

| Coluna   | Tipo         | Descrição                |
| -------- | ------------ | ------------------------ |
| id       | int          | Identificador único (PK) |
| name     | varchar(255) | Nome do médico           |
| statusId | int          | Status do médico (FK)    |

#### Patient (Paciente)

Armazena os dados dos pacientes.

| Coluna         | Tipo         | Descrição                |
| -------------- | ------------ | ------------------------ |
| id             | int          | Identificador único (PK) |
| name           | varchar(255) | Nome do paciente         |
| phoneNumber    | varchar(20)  | Telefone de contato      |
| documentNumber | varchar(50)  | Número do documento      |
| statusId       | int          | Status do paciente (FK)  |
| birthDate      | date         | Data de nascimento       |

### Tabelas de Relacionamento

#### Doctor_Specialty

Relaciona médicos com suas especialidades (N:N).

| Coluna      | Tipo | Descrição                    |
| ----------- | ---- | ---------------------------- |
| doctorId    | int  | ID do médico (PK, FK)        |
| specialtyId | int  | ID da especialidade (PK, FK) |

#### Appointment (Agendamento)

Armazena os agendamentos de consultas.

| Coluna          | Tipo     | Descrição                    |
| --------------- | -------- | ---------------------------- |
| id              | int      | Identificador único (PK)     |
| patientId       | int      | ID do paciente (FK)          |
| doctorId        | int      | ID do médico (FK)            |
| specialtyId     | int      | ID da especialidade (FK)     |
| appointmentDate | datetime | Data e hora da consulta      |
| observation     | text     | Observações sobre a consulta |

## Arquitetura do Projeto (Clean Architecture)

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

## Configuração do Ambiente de Desenvolvimento

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
   wget https://dev.mysql.com/get/mysql-apt-config_0.8.33-1_all.debsudo apt install ./mysql-apt-config_0.8.33-1_all.deb
   ```

4. Instalar MySQL Server:

   ```bash
   sudo apt updatesudo apt install mysql-server
   ```

## Inicialização do Projeto

Para criar um projeto ASP.NET Core WebAPI:

```bash
dotnet new webapi --name ClinAgenda
```

## Diferenças entre C#, .NET, ASP.NET e ASP.NET Core

| Termo        | O que é?                                                     | Relacionamento                                           |
| ------------ | ------------------------------------------------------------ | -------------------------------------------------------- |
| .NET         | Plataforma de desenvolvimento (ecossistema)                  | Base para tudo                                           |
| C#           | Linguagem de programação                                     | Usada para escrever código no .NET                       |
| ASP.NET      | Framework para criar aplicações web (versão mais antiga)     | Parte do .NET, focado em web                             |
| ASP.NET Core | Framework moderno e multiplataforma para criar aplicações web e APIs | Parte do .NET 5+, sucessor do ASP.NET                    |
| ASP.NET MVC  | Padrão de arquitetura para organizar aplicações web (Model-View-Controller) | Pode ser usado no ASP.NET tradicional ou no ASP.NET Core |