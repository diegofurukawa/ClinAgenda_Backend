# Documentação do Banco de Dados ClinAgenda

## Visão Geral

O banco de dados `db_clinagenda` foi projetado para dar suporte a um sistema de agendamento de consultas para clínicas médicas. Ele armazena informações sobre médicos, pacientes, especialidades, agendamentos e controle de acesso/autenticação de usuários.

## Estrutura do Banco de Dados

```sql
-- Criação da base de dados
CREATE DATABASE IF NOT EXISTS `db_clinagenda` 
/*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ 
/*!80016 DEFAULT ENCRYPTION='N' */;

-- Selecionando a base de dados
USE `db_clinagenda`;
```

### Tabelas Principais

#### Status

Armazena os diferentes status para médicos, pacientes e agendamentos.

```sql
CREATE TABLE IF NOT EXISTS `status` (
  `statusId` int NOT NULL AUTO_INCREMENT,
  `statusName` varchar(255) NOT NULL,
  `statusType` varchar(50) NOT NULL COMMENT 'Tipo de status (doctor, patient, appointment, etc.)',
  `dCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `dLastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `lActive` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`statusId`),
  KEY `idx_status_type` (`statusType`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

| Coluna       | Tipo         | Descrição                                         |
| ------------ | ------------ | ------------------------------------------------- |
| statusId     | int          | Identificador único (PK)                          |
| statusName   | varchar(255) | Nome do status                                    |
| statusType   | varchar(50)  | Tipo de status (doctor, patient, appointment, etc.)|
| dCreated     | datetime     | Data de criação                                   |
| dLastUpdated | datetime     | Data da última atualização                        |
| lActive      | tinyint(1)   | Status ativo (1) ou inativo (0)                   |

#### Specialty (Especialidade)

Armazena as especialidades médicas disponíveis.

```sql
CREATE TABLE IF NOT EXISTS `specialty` (
  `specialtyId` int NOT NULL AUTO_INCREMENT,
  `specialtyName` varchar(255) NOT NULL,
  `nScheduleDuration` int NOT NULL,
  `dCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `dLastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `lActive` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`specialtyId`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

| Coluna            | Tipo         | Descrição                              |
| ----------------- | ------------ | -------------------------------------- |
| specialtyId       | int          | Identificador único (PK)               |
| specialtyName     | varchar(255) | Nome da especialidade                  |
| nScheduleDuration | int          | Duração em minutos de uma consulta     |
| dCreated          | datetime     | Data de criação                        |
| dLastUpdated      | datetime     | Data da última atualização             |
| lActive           | tinyint(1)   | Status ativo (1) ou inativo (0)        |

#### Doctor (Médico)

Armazena os dados dos médicos.

```sql
CREATE TABLE IF NOT EXISTS `doctor` (
  `doctorId` int NOT NULL AUTO_INCREMENT,
  `doctorName` varchar(255) NOT NULL,
  `statusId` int NOT NULL,
  `dCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `dLastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `lActive` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`doctorId`),
  KEY `fk_doctor_status` (`statusId`),
  CONSTRAINT `fk_doctor_status` FOREIGN KEY (`statusId`) REFERENCES `status` (`statusId`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

| Coluna       | Tipo         | Descrição                         |
| ------------ | ------------ | --------------------------------- |
| doctorId     | int          | Identificador único (PK)          |
| doctorName   | varchar(255) | Nome do médico                    |
| statusId     | int          | Status do médico (FK)             |
| dCreated     | datetime     | Data de criação                   |
| dLastUpdated | datetime     | Data da última atualização        |
| lActive      | tinyint(1)   | Status ativo (1) ou inativo (0)   |

#### Patient (Paciente)

Armazena os dados dos pacientes.

```sql
CREATE TABLE IF NOT EXISTS `patient` (
  `patientId` int NOT NULL AUTO_INCREMENT,
  `patientName` varchar(255) NOT NULL,
  `phoneNumber` varchar(20) NOT NULL,
  `documentNumber` varchar(50) NOT NULL,
  `statusId` int NOT NULL,
  `dBirthDate` date DEFAULT NULL,
  `dCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `dLastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `lActive` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`patientId`),
  KEY `statusId` (`statusId`),
  CONSTRAINT `patient_ibfk_1` FOREIGN KEY (`statusId`) REFERENCES `status` (`statusId`)
) ENGINE=InnoDB AUTO_INCREMENT=35 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

| Coluna          | Tipo         | Descrição                         |
| --------------- | ------------ | --------------------------------- |
| patientId       | int          | Identificador único (PK)          |
| patientName     | varchar(255) | Nome do paciente                  |
| phoneNumber     | varchar(20)  | Telefone de contato               |
| documentNumber  | varchar(50)  | Número do documento               |
| statusId        | int          | Status do paciente (FK)           |
| dBirthDate      | date         | Data de nascimento                |
| dCreated        | datetime     | Data de criação                   |
| dLastUpdated    | datetime     | Data da última atualização        |
| lActive         | tinyint(1)   | Status ativo (1) ou inativo (0)   |

### Tabelas de Relacionamento

#### Doctor_Specialty

Relaciona médicos com suas especialidades (N:N).

```sql
CREATE TABLE IF NOT EXISTS `doctor_specialty` (
  `doctorId` int NOT NULL,
  `specialtyId` int NOT NULL,
  `dCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `dLastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `lActive` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`doctorId`,`specialtyId`),
  KEY `FK_doctorspecialty_specialty` (`specialtyId`),
  CONSTRAINT `FK_doctorspecialty_doctor` FOREIGN KEY (`doctorId`) REFERENCES `doctor` (`doctorId`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_doctorspecialty_specialty` FOREIGN KEY (`specialtyId`) REFERENCES `specialty` (`specialtyId`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

| Coluna       | Tipo       | Descrição                         |
| ------------ | ---------- | --------------------------------- |
| doctorId     | int        | ID do médico (PK, FK)             |
| specialtyId  | int        | ID da especialidade (PK, FK)      |
| dCreated     | datetime   | Data de criação                   |
| dLastUpdated | datetime   | Data da última atualização        |
| lActive      | tinyint(1) | Status ativo (1) ou inativo (0)   |

#### Appointment (Agendamento)

Armazena os agendamentos de consultas.

```sql
CREATE TABLE IF NOT EXISTS `appointment` (
  `appointmentId` int NOT NULL AUTO_INCREMENT,
  `patientId` int NOT NULL,
  `doctorId` int NOT NULL,
  `specialtyId` int NOT NULL,
  `statusId` int NOT NULL,
  `dAppointmentDate` datetime NOT NULL,
  `observation` text,
  `dCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `dLastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `lActive` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`appointmentId`),
  KEY `fk_patient` (`patientId`),
  KEY `fk_doctor` (`doctorId`),
  KEY `fk_specialty` (`specialtyId`),
  KEY `fk_appointment_status` (`statusId`),
  CONSTRAINT `fk_doctor` FOREIGN KEY (`doctorId`) REFERENCES `doctor` (`doctorId`),
  CONSTRAINT `fk_patient` FOREIGN KEY (`patientId`) REFERENCES `patient` (`patientId`),
  CONSTRAINT `fk_specialty` FOREIGN KEY (`specialtyId`) REFERENCES `specialty` (`specialtyId`),
  CONSTRAINT `fk_appointment_status` FOREIGN KEY (`statusId`) REFERENCES `status` (`statusId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

| Coluna           | Tipo       | Descrição                         |
| ---------------- | ---------- | --------------------------------- |
| appointmentId    | int        | Identificador único (PK)          |
| patientId        | int        | ID do paciente (FK)               |
| doctorId         | int        | ID do médico (FK)                 |
| specialtyId      | int        | ID da especialidade (FK)          |
| statusId         | int        | Status do agendamento (FK)        |
| dAppointmentDate | datetime   | Data e hora da consulta           |
| observation      | text       | Observações sobre a consulta      |
| dCreated         | datetime   | Data de criação                   |
| dLastUpdated     | datetime   | Data da última atualização        |
| lActive          | tinyint(1) | Status ativo (1) ou inativo (0)   |

### Tabelas de Autenticação e Controle de Acesso

#### User (Usuário)

Armazena dados dos usuários do sistema.

```sql
CREATE TABLE IF NOT EXISTS `user` (
  `userId` int NOT NULL AUTO_INCREMENT,
  `username` varchar(50) NOT NULL,
  `email` varchar(100) NOT NULL,
  `passwordHash` varchar(255) NOT NULL,
  `dLastLogin` datetime DEFAULT NULL,
  `nFailedLoginAttempts` int NOT NULL DEFAULT 0,
  `dCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `dLastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `lActive` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`userId`),
  UNIQUE KEY `username` (`username`),
  UNIQUE KEY `email` (`email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

| Coluna               | Tipo         | Descrição                         |
| -------------------- | ------------ | --------------------------------- |
| userId               | int          | Identificador único (PK)          |
| username             | varchar(50)  | Nome de usuário (único)           |
| email                | varchar(100) | Email do usuário (único)          |
| passwordHash         | varchar(255) | Hash da senha                     |
| dLastLogin           | datetime     | Data/hora do último login         |
| nFailedLoginAttempts | int          | Contagem de tentativas falhas     |
| dCreated             | datetime     | Data de criação                   |
| dLastUpdated         | datetime     | Data da última atualização        |
| lActive              | tinyint(1)   | Status ativo (1) ou inativo (0)   |

#### Role (Perfil de Usuário)

Armazena os diferentes perfis de acesso.

```sql
CREATE TABLE IF NOT EXISTS `role` (
  `roleId` int NOT NULL AUTO_INCREMENT,
  `roleName` varchar(50) NOT NULL,
  `description` varchar(255) DEFAULT NULL,
  `dCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `dLastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `lActive` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`roleId`),
  UNIQUE KEY `roleName` (`roleName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

| Coluna       | Tipo         | Descrição                         |
| ------------ | ------------ | --------------------------------- |
| roleId       | int          | Identificador único (PK)          |
| roleName     | varchar(50)  | Nome do perfil (único)            |
| description  | varchar(255) | Descrição do perfil               |
| dCreated     | datetime     | Data de criação                   |
| dLastUpdated | datetime     | Data da última atualização        |
| lActive      | tinyint(1)   | Status ativo (1) ou inativo (0)   |

#### User_Role (Usuário-Perfil)

Relaciona usuários com seus perfis (N:N).

```sql
CREATE TABLE IF NOT EXISTS `user_role` (
  `userId` int NOT NULL,
  `roleId` int NOT NULL,
  `dCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `dLastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `lActive` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`userId`,`roleId`),
  KEY `fk_userrole_role` (`roleId`),
  CONSTRAINT `fk_userrole_user` FOREIGN KEY (`userId`) REFERENCES `user` (`userId`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_userrole_role` FOREIGN KEY (`roleId`) REFERENCES `role` (`roleId`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

| Coluna       | Tipo       | Descrição                         |
| ------------ | ---------- | --------------------------------- |
| userId       | int        | ID do usuário (PK, FK)            |
| roleId       | int        | ID do perfil (PK, FK)             |
| dCreated     | datetime   | Data de criação                   |
| dLastUpdated | datetime   | Data da última atualização        |
| lActive      | tinyint(1) | Status ativo (1) ou inativo (0)   |

#### Permission (Permissão)

Armazena as permissões específicas disponíveis no sistema.

```sql
CREATE TABLE IF NOT EXISTS `permission` (
  `permissionId` int NOT NULL AUTO_INCREMENT,
  `permissionName` varchar(100) NOT NULL,
  `description` varchar(255) DEFAULT NULL,
  `dCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `dLastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `lActive` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`permissionId`),
  UNIQUE KEY `permissionName` (`permissionName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

| Coluna         | Tipo         | Descrição                         |
| -------------- | ------------ | --------------------------------- |
| permissionId   | int          | Identificador único (PK)          |
| permissionName | varchar(100) | Nome da permissão (único)         |
| description    | varchar(255) | Descrição da permissão            |
| dCreated       | datetime     | Data de criação                   |
| dLastUpdated   | datetime     | Data da última atualização        |
| lActive        | tinyint(1)   | Status ativo (1) ou inativo (0)   |

#### Role_Permission (Perfil-Permissão)

Relaciona perfis com suas permissões (N:N).

```sql
CREATE TABLE IF NOT EXISTS `role_permission` (
  `roleId` int NOT NULL,
  `permissionId` int NOT NULL,
  `dCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `dLastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `lActive` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`roleId`,`permissionId`),
  KEY `fk_rolepermission_permission` (`permissionId`),
  CONSTRAINT `fk_rolepermission_role` FOREIGN KEY (`roleId`) REFERENCES `role` (`roleId`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_rolepermission_permission` FOREIGN KEY (`permissionId`) REFERENCES `permission` (`permissionId`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

| Coluna        | Tipo       | Descrição                         |
| ------------- | ---------- | --------------------------------- |
| roleId        | int        | ID do perfil (PK, FK)             |
| permissionId  | int        | ID da permissão (PK, FK)          |
| dCreated      | datetime   | Data de criação                   |
| dLastUpdated  | datetime   | Data da última atualização        |
| lActive       | tinyint(1) | Status ativo (1) ou inativo (0)   |

#### Auth_Token (Token de Autenticação)

Armazena tokens de autenticação/sessão dos usuários.

```sql
CREATE TABLE IF NOT EXISTS `auth_token` (
  `tokenId` int NOT NULL AUTO_INCREMENT,
  `userId` int NOT NULL,
  `token` varchar(255) NOT NULL,
  `dExpires` datetime NOT NULL,
  `dCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `dLastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `lActive` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`tokenId`),
  UNIQUE KEY `token` (`token`),
  KEY `fk_token_user` (`userId`),
  CONSTRAINT `fk_token_user` FOREIGN KEY (`userId`) REFERENCES `user` (`userId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

| Coluna       | Tipo         | Descrição                         |
| ------------ | ------------ | --------------------------------- |
| tokenId      | int          | Identificador único (PK)          |
| userId       | int          | ID do usuário (FK)                |
| token        | varchar(255) | Token de acesso (único)           |
| dExpires     | datetime     | Data/hora de expiração            |
| dCreated     | datetime     | Data de criação                   |
| dLastUpdated | datetime     | Data da última atualização        |
| lActive      | tinyint(1)   | Status ativo (1) ou inativo (0)   |

#### Audit_Log (Log de Auditoria)

Registra ações realizadas no sistema para fins de auditoria.

```sql
CREATE TABLE IF NOT EXISTS `audit_log` (
  `logId` int NOT NULL AUTO_INCREMENT,
  `userId` int DEFAULT NULL,
  `action` varchar(100) NOT NULL,
  `description` text,
  `ipAddress` varchar(45) DEFAULT NULL,
  `userAgent` varchar(255) DEFAULT NULL,
  `dCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`logId`),
  KEY `fk_auditlog_user` (`userId`),
  CONSTRAINT `fk_auditlog_user` FOREIGN KEY (`userId`) REFERENCES `user` (`userId`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

| Coluna      | Tipo         | Descrição                       |
| ----------- | ------------ | ------------------------------- |
| logId       | int          | Identificador único (PK)        |
| userId      | int          | ID do usuário (FK, opcional)    |
| action      | varchar(100) | Ação realizada                  |
| description | text         | Descrição detalhada             |
| ipAddress   | varchar(45)  | Endereço IP                     |
| userAgent   | varchar(255) | Navegador/dispositivo           |
| dCreated    | datetime     | Data/hora do registro           |

#### User_Entity (Usuário-Entidade)

Relaciona usuários com médicos ou pacientes.

```sql
CREATE TABLE IF NOT EXISTS `user_entity` (
  `userId` int NOT NULL,
  `entityType` enum('doctor','patient') NOT NULL,
  `entityId` int NOT NULL,
  `dCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `dLastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `lActive` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`userId`),
  UNIQUE KEY `entity_unique` (`entityType`,`entityId`),
  CONSTRAINT `fk_userentity_user` FOREIGN KEY (`userId`) REFERENCES `user` (`userId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```

| Coluna       | Tipo                     | Descrição                         |
| ------------ | ------------------------ | --------------------------------- |
| userId       | int                      | ID do usuário (PK, FK)            |
| entityType   | enum('doctor','patient') | Tipo da entidade                  |
| entityId     | int                      | ID da entidade (médico/paciente)  |
| dCreated     | datetime                 | Data de criação                   |
| dLastUpdated | datetime                 | Data da última atualização        |
| lActive      | tinyint(1)               | Status ativo (1) ou inativo (0)   |

## Convenções de Nomenclatura

O banco de dados segue as seguintes convenções de nomenclatura:

- Prefixos para tipos de dados específicos:
  - Campos de data: prefixo `d` (ex: `dCreated`, `dBirthDate`)
  - Campos numéricos: prefixo `n` (ex: `nScheduleDuration`, `nFailedLoginAttempts`)
  - Campos booleanos: prefixo `l` (ex: `lActive`)

- Campos comuns em todas as tabelas:
  - `dCreated`: Data e hora de criação do registro
  - `dLastUpdated`: Data e hora da última atualização
  - `lActive`: Flag para indicar se o registro está ativo (1) ou inativo (0)

## Aspectos de Segurança

- Senhas de usuário são armazenadas como hashes (`passwordHash`)
- Sistema monitora tentativas de login falhas (`nFailedLoginAttempts`)
- Tokens de autenticação têm data de expiração (`dExpires`)
- Todas as ações são registradas no log de auditoria (`audit_log`)
- A exclusão de um usuário exclui em cascata seus tokens, roles e associações com entidades

## Relações Entre Entidades

- Um médico pode ter várias especialidades e uma especialidade pode ser de vários médicos (N:N)
- Um médico e um paciente podem ter apenas um status (1:N)
- Um agendamento envolve um médico, um paciente, uma especialidade e um status (N:1)
- Um usuário pode ter vários perfis e um perfil pode ser atribuído a vários usuários (N:N)
- Um perfil pode ter várias permissões e uma permissão pode estar em vários perfis (N:N)
- Um usuário pode ser associado a um médico ou a um paciente (1:1)