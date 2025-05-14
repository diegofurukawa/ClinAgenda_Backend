
-- Criação da base de dados
CREATE DATABASE IF NOT EXISTS `db_clinagenda`;

-- Selecionando a base de dados
USE `db_clinagenda`;

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

CREATE TABLE IF NOT EXISTS `specialty` (
  `specialtyId` int NOT NULL AUTO_INCREMENT,
  `specialtyName` varchar(255) NOT NULL,
  `nScheduleDuration` int NOT NULL,
  `dCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `dLastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `lActive` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`specialtyId`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

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

-- Tabela de operações (list, insert, update, delete, etc.)
CREATE TABLE IF NOT EXISTS `operation` (
  `operationId` int NOT NULL AUTO_INCREMENT,
  `operationName` varchar(100) NOT NULL,
  `operationKey` varchar(100) NOT NULL,
  `module` varchar(50) NOT NULL,
  `description` varchar(255) DEFAULT NULL,
  `dCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `dLastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `lActive` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`operationId`),
  UNIQUE KEY `operationKey_module` (`operationKey`, `module`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Tabela de relação direta entre usuários e operações permitidas
CREATE TABLE IF NOT EXISTS `user_operation` (
  `userId` int NOT NULL,
  `operationId` int NOT NULL,
  `dCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `dLastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `lActive` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`userId`, `operationId`),
  KEY `fk_useroperation_operation` (`operationId`),
  CONSTRAINT `fk_useroperation_user` FOREIGN KEY (`userId`) REFERENCES `user` (`userId`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_useroperation_operation` FOREIGN KEY (`operationId`) REFERENCES `operation` (`operationId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Tabela para controle de senhas de atendimento
CREATE TABLE IF NOT EXISTS `queue_ticket` (
  `ticketId` int NOT NULL AUTO_INCREMENT,
  `ticketNumber` varchar(20) NOT NULL,
  `ticketType` enum('NORMAL','PRIORITY','SCHEDULED') NOT NULL,
  `patientId` int DEFAULT NULL,
  `appointmentId` int DEFAULT NULL,
  `createdBy` int NOT NULL,
  `calledBy` int DEFAULT NULL,
  `statusId` int NOT NULL,
  `dCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `dCalled` datetime DEFAULT NULL,
  `dFinished` datetime DEFAULT NULL,
  `dLastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `lActive` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`ticketId`),
  UNIQUE KEY `ticketNumber_dCreated` (`ticketNumber`, `dCreated`),
  KEY `fk_ticket_patient` (`patientId`),
  KEY `fk_ticket_appointment` (`appointmentId`),
  KEY `fk_ticket_createdBy` (`createdBy`),
  KEY `fk_ticket_calledBy` (`calledBy`),
  KEY `fk_ticket_status` (`statusId`),
  CONSTRAINT `fk_ticket_patient` FOREIGN KEY (`patientId`) REFERENCES `patient` (`patientId`),
  CONSTRAINT `fk_ticket_appointment` FOREIGN KEY (`appointmentId`) REFERENCES `appointment` (`appointmentId`),
  CONSTRAINT `fk_ticket_createdBy` FOREIGN KEY (`createdBy`) REFERENCES `user` (`userId`),
  CONSTRAINT `fk_ticket_calledBy` FOREIGN KEY (`calledBy`) REFERENCES `user` (`userId`),
  CONSTRAINT `fk_ticket_status` FOREIGN KEY (`statusId`) REFERENCES `status` (`statusId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Tabela para configuração de sequências de senhas
CREATE TABLE IF NOT EXISTS `queue_counter` (
  `counterId` int NOT NULL AUTO_INCREMENT,
  `counterType` enum('NORMAL','PRIORITY','SCHEDULED') NOT NULL,
  `prefix` varchar(5) NOT NULL,
  `currentNumber` int NOT NULL DEFAULT 1,
  `resetDaily` tinyint(1) NOT NULL DEFAULT 1,
  `lastResetDate` date DEFAULT NULL,
  `dCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `dLastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `lActive` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`counterId`),
  UNIQUE KEY `counterType` (`counterType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Tabela para registrar histórico de chamadas
CREATE TABLE IF NOT EXISTS `queue_call_history` (
  `callId` int NOT NULL AUTO_INCREMENT,
  `ticketId` int NOT NULL,
  `calledBy` int NOT NULL,
  `displayId` int DEFAULT NULL,
  `dCalled` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `callCount` int NOT NULL DEFAULT 1,
  PRIMARY KEY (`callId`),
  KEY `fk_callhistory_ticket` (`ticketId`),
  KEY `fk_callhistory_calledBy` (`calledBy`),
  CONSTRAINT `fk_callhistory_ticket` FOREIGN KEY (`ticketId`) REFERENCES `queue_ticket` (`ticketId`),
  CONSTRAINT `fk_callhistory_calledBy` FOREIGN KEY (`calledBy`) REFERENCES `user` (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Tabela opcional para configurar displays/painéis
CREATE TABLE IF NOT EXISTS `queue_display` (
  `displayId` int NOT NULL AUTO_INCREMENT,
  `displayName` varchar(100) NOT NULL,
  `location` varchar(100) DEFAULT NULL,
  `dCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `dLastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `lActive` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`displayId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


-- Inserindo contadores iniciais para as senhas
INSERT INTO `queue_counter` (`counterType`, `prefix`, `currentNumber`, `resetDaily`) VALUES
('NORMAL', 'N', 1, 1),
('PRIORITY', 'P', 1, 1),
('SCHEDULED', 'S', 1, 1);

-- Inserindo status relacionados a senhas (se ainda não existirem)
INSERT INTO `status` (`statusName`, `statusType`) VALUES
('Aguardando', 'queue'),
('Chamado', 'queue'),
('Em Atendimento', 'queue'),
('Finalizado', 'queue'),
('Ausente', 'queue');

-- Inserindo operações para cada módulo (exemplo)
INSERT INTO `operation` (`operationName`, `operationKey`, `module`, `description`) VALUES
-- Módulo Paciente
('Listar Pacientes', 'PAGELIST', 'PATIENT', 'Ver lista de pacientes'),
('Inserir Paciente', 'FORM.INSERT', 'PATIENT', 'Inserir novo paciente'),
('Atualizar Paciente', 'FORM.UPDATE', 'PATIENT', 'Atualizar dados de paciente'),
('Excluir Paciente', 'FORM.DELETE', 'PATIENT', 'Excluir paciente'),

-- Módulo Médico
('Listar Médicos', 'PAGELIST', 'DOCTOR', 'Ver lista de médicos'),
('Inserir Médico', 'FORM.INSERT', 'DOCTOR', 'Inserir novo médico'),
('Atualizar Médico', 'FORM.UPDATE', 'DOCTOR', 'Atualizar dados de médico'),
('Excluir Médico', 'FORM.DELETE', 'DOCTOR', 'Excluir médico'),

-- Módulo Especialidade
('Listar Especialidades', 'PAGELIST', 'SPECIALTY', 'Ver lista de especialidades'),
('Inserir Especialidade', 'FORM.INSERT', 'SPECIALTY', 'Inserir nova especialidade'),
('Atualizar Especialidade', 'FORM.UPDATE', 'SPECIALTY', 'Atualizar especialidade'),
('Excluir Especialidade', 'FORM.DELETE', 'SPECIALTY', 'Excluir especialidade'),

-- Módulo Agendamento
('Listar Agendamentos', 'PAGELIST', 'APPOINTMENT', 'Ver lista de agendamentos'),
('Inserir Agendamento', 'FORM.INSERT', 'APPOINTMENT', 'Inserir novo agendamento'),
('Atualizar Agendamento', 'FORM.UPDATE', 'APPOINTMENT', 'Atualizar agendamento'),
('Excluir Agendamento', 'FORM.DELETE', 'APPOINTMENT', 'Excluir agendamento');