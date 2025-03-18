# Script SQL para Criação do Banco de Dados ClinAgenda

```sql
/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

-- Criação da base de dados
CREATE DATABASE IF NOT EXISTS `clinagenda_database` 
/*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ 
/*!80016 DEFAULT ENCRYPTION='N' */;

-- Selecionando a base de dados
USE `clinagenda_database`;

-- Criação da tabela de status
CREATE TABLE IF NOT EXISTS `status` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Criação da tabela de especialidades
CREATE TABLE IF NOT EXISTS `specialty` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `scheduleDuration` int NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Criação da tabela de médicos
CREATE TABLE IF NOT EXISTS `doctor` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `statusId` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_doctor_status` (`statusId`),
  CONSTRAINT `fk_doctor_status` FOREIGN KEY (`statusId`) REFERENCES `status` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Criação da tabela de pacientes
CREATE TABLE IF NOT EXISTS `patient` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `phoneNumber` varchar(20) NOT NULL,
  `documentNumber` varchar(50) NOT NULL,
  `statusId` int NOT NULL,
  `birthDate` date DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `statusId` (`statusId`),
  CONSTRAINT `patient_ibfk_1` FOREIGN KEY (`statusId`) REFERENCES `status` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=35 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Criação da tabela de relação médico-especialidade
CREATE TABLE IF NOT EXISTS `doctor_specialty` (
  `doctorId` int NOT NULL,
  `specialtyId` int NOT NULL,
  PRIMARY KEY (`doctorId`,`specialtyId`),
  KEY `FK_doctorspecialty_specialty` (`specialtyId`),
  CONSTRAINT `FK_doctorspecialty_doctor` FOREIGN KEY (`doctorId`) REFERENCES `doctor` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_doctorspecialty_specialty` FOREIGN KEY (`specialtyId`) REFERENCES `specialty` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Criação da tabela de agendamentos
CREATE TABLE IF NOT EXISTS `appointment` (
  `id` int NOT NULL AUTO_INCREMENT,
  `patientId` int NOT NULL,
  `doctorId` int NOT NULL,
  `specialtyId` int NOT NULL,
  `appointmentDate` datetime NOT NULL,
  `observation` text,
  PRIMARY KEY (`id`),
  KEY `fk_patient` (`patientId`),
  KEY `fk_doctor` (`doctorId`),
  KEY `fk_specialty` (`specialtyId`),
  CONSTRAINT `fk_doctor` FOREIGN KEY (`doctorId`) REFERENCES `doctor` (`Id`),
  CONSTRAINT `fk_patient` FOREIGN KEY (`patientId`) REFERENCES `patient` (`id`),
  CONSTRAINT `fk_specialty` FOREIGN KEY (`specialtyId`) REFERENCES `specialty` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
```

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