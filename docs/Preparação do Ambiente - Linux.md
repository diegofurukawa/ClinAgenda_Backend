# Preparação para o Curso - Linux

Para quem tiver disponibilidade, aqui está a lista de coisas que vocês podem ir instalando na máquina que usarão durante o curso.

Caso não consigam adiantar essa instalação, não tem problema algum, de qualquer forma vamos passar por todas configurações hoje a noite durante a aula com todo mundo ❤️

# Backend:

- Instalar o ASP.NET / Instalar .NET 8.0 Runtime
  - Debian
    - https://learn.microsoft.com/en-us/dotnet/core/install/linux-debian?tabs=dotnet8

## Debian 12

Installing with APT can be done with a few commands. Before you install .NET, run the following commands to add the Microsoft package signing key to your list of trusted keys and add the package repository.

Open a terminal and run the following commands:

```
wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
```

### Install the SDK

The .NET SDK allows you to develop apps with .NET. If you install the .NET SDK, you don't need to install the corresponding runtime. To install the .NET SDK, run the following commands:

```
sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-8.0
```

### Install the runtime

The ASP.NET Core Runtime allows you to run apps that were made with .NET that didn't provide the runtime. The following commands install the ASP.NET Core Runtime, which is the most compatible runtime for .NET. In your terminal, run the following commands:

```
sudo apt-get update && \
  sudo apt-get install -y aspnetcore-runtime-8.0
```

As an alternative to the ASP.NET Core Runtime, you can install the .NET Runtime, which doesn't include ASP.NET Core support: replace `aspnetcore-runtime-8.0` in the previous command with `dotnet-runtime-8.0`:

```
sudo apt-get install -y dotnet-runtime-8.0
```

## Use APT to update .NET

When a new patch release is available for .NET, you can simply upgrade it through APT with the following commands:

```
sudo apt-get update
sudo apt-get upgrade
```



![image-20250318103350825](/home/dfurukawa/.config/Typora/typora-user-images/image-20250318103350825.png)



- ~~Instalar o ASP.NET~~

  - ~~https://learn.microsoft.com/en-us/dotnet/core/install/linux~~
  - ~~https://learn.microsoft.com/pt-br/dotnet/core/install/linux-scripted-manual#scripted-install~~
  - ~~Debian~~
    - ~~https://learn.microsoft.com/en-us/dotnet/core/install/linux-debian?tabs=dotnet8~~

- ~~Instalar .NET 8.0 Runtime~~

  - ~~https://learn.microsoft.com/pt-br/dotnet/core/install/linux-ubuntu-install?pivots=os-linux-ubuntu-2404&tabs=dotnet8~~
  - ~~Procure na listagem abaixo da tela a versão 8.0.0~~
  - ~~Clique para baixar na versão compatível do seu computador, no bloco "Runtime do ASP.NET Core 8.0.0" (leia com atenção o título dos blocos)~~

  

  ### Gerenciador de Banco de Dados

  - ##### Server

    - MySQL 8.0.41: https://dev.mysql.com/downloads/mysql/
      - Clique em 'download' e depois procure na tela "No thanks, just start my download."
      - Durante a instalação, será necessário definir uma senha. Guarde-a, pois iremos utilizar a senha para rodar o projeto

  - ##### Client

    - MySQL Workbench
      - https://dev.mysql.com/downloads/workbench/
    - dBeaver
      - https://dbeaver.io/download/
    - Beekeeper Studio
      - https://www.beekeeperstudio.io/get

    

- Instalar VS Code: https://code.visualstudio.com/download

- Instalar extensões do VS Code

  - [Instalar extensão C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
  - [Instalar extensão C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
  - [.NET Install Tool](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.vscode-dotnet-runtime)
  - [C# Extensions](https://marketplace.visualstudio.com/items?itemName=kreativ-software.csharpextensions)

## Frontend:

- Node.js: https://nodejs.org/pt/download
- Instalar Yarn: `npm install --global yarn`
- Instalar extensões do VS Code:
  - [Volar (Vue Language Features)](https://marketplace.visualstudio.com/items?itemName=Vue.volar)
  - [ESLint](https://marketplace.visualstudio.com/items?itemName=dbaeumer.vscode-eslint)
  - [Prettier](https://marketplace.visualstudio.com/items?itemName=esbenp.prettier-vscode)
  - [Prettier ESLint](https://marketplace.visualstudio.com/items?itemName=rvest.vs-code-prettier-eslint)













# How to Install MySQL on Debian 12 (Bookworm) - Linux Genie

[MySQL](https://www.mysql.com/) is a popular free and open-source RDBMS (Relational Database Management System) used to store data in the form of tables. Most developers used the MySQL database to store data for online applications, content management systems, and e-commerce websites.

In this tutorial, we will go through the installation of the MySQL Server on the [Debian 12](https://linuxgenie.net/how-to-download-and-install-debian-12-on-vmware-workstation/) Bookworm distribution.

Prerequisites
-------------

*   The Debian 12 distribution should be running on your system or VirtualBox.
*   You should have sudo access or root privileges.

MySQL installation on Debian 12
-------------------------------

To install MySQL Server on Debian 12 distribution, perform the following steps:

### Step 1: Update Repositories Packages

First, open the terminal windows using application search bar:

![](http://linuxgenie.net/wp-content/uploads/2023/06/word-image-7821-1.png)

update all available packages using these commands:

```
$ sudo apt update
```


![](http://linuxgenie.net/wp-content/uploads/2023/06/word-image-7821-2.png)

```
$ sudo apt upgrade
```


### Step 2: Install Required Packages

If you are using a fresh Debian 12 distro, you will install the ‘wget’ package to download apt MySQL repository. Install the wget package using this command:

```
$ sudo apt install wget
```


![](http://linuxgenie.net/wp-content/uploads/2023/06/word-image-7821-3.png)

### Step 3: Download and add MySQL Apt Repository

MySQL is not available in the Debian 12 repository. However, you can download and add an apt MySQL repository to your Debian system. So, download the recent MySQL release from the [official MySQL download page](https://dev.mysql.com/downloads/repo/apt/) using this command:

```
$ wget https://dev.mysql.com/get/mysql-apt-config_0.8.33-1_all.deb
```


![](http://linuxgenie.net/wp-content/uploads/2023/06/word-image-7821-4.png)

Now, add this package to your system using the below-mentioned command:

```
$ sudo apt install ./mysql-apt-config_0.8.33-1_all.deb
```


### Step 4: Install MySQL on Debian 12

Once the MySQL package is added to your system, you can install MySQL on the Debian system. Update the system repositories and use the below command to install the MySQL server on Debian 12:

```
$ sudo apt update
```


```
$ sudo apt install mysql-server
```


![](http://linuxgenie.net/wp-content/uploads/2023/06/word-image-7821-5.png)

After running the above command, you are required to enter the password for root to use MySQL database login.

![](http://linuxgenie.net/wp-content/uploads/2023/06/word-image-7821-6.png)

Enter the password and re-enter again to confirm this password.

![](http://linuxgenie.net/wp-content/uploads/2023/06/word-image-7821-7.png)

Select ‘ok’ and the below output will display on the terminal:

![](http://linuxgenie.net/wp-content/uploads/2023/06/word-image-7821-8.png)

Now, check the installed MySQL version:

```
$ mysql --version
```


![](http://linuxgenie.net/wp-content/uploads/2023/06/word-image-7821-9.png)

### Step 5: Start MySQL Service

Once the MySQL installation is complete, the MySQL service will automatically start and run on your system. You can also check whether MySQL service is running on your system or not using this command:

```
$ sudo systemctl status mysql
```


![](http://linuxgenie.net/wp-content/uploads/2023/06/word-image-7821-10.png)

If you want to stop the MySQL service due to any reason, you can stop Mysql service using this command:

```
$ sudo systemctl stop mysql
```


![](http://linuxgenie.net/wp-content/uploads/2023/06/word-image-7821-11.png)

To start MySQL again, use this command to start MySQL service on Debian 12:

```
$ sudo systemctl start mysql
```


![](http://linuxgenie.net/wp-content/uploads/2023/06/word-image-7821-12.png)

### Step 6: Access MySQL Database

To access MySQL databases, log in to the MySQL server as root using this command:

```
$ mysql -u root -p
```


Enter the password that you enter as the root password during the MySQL installation. After that, the following MySQL shell will open on the terminal:

![](http://linuxgenie.net/wp-content/uploads/2023/06/word-image-7821-13.png)

Type the ‘exit’ command to leave the MySQL shell:

```
> exit
```


Uninstall MySQL from Debian 12
------------------------------

To uninstall MySQL from Debian 12, stop MySQL and use this command:

```
$ sudo apt purge mysql-server
```











# How to Reset MySQL Password on Ubuntu 22.04 - 2025

In this blog, we will learn how to reset MySQL Root Password in Ubuntu/Linux. There might be a case you have forgotten your MySQL Database Password. In these steps, we will reset root password using ubuntu command line. This process of resetting root password is without login into mysql database.

_Video Guide for Reset or Change the MySQL root Forgotten password on Ubuntu_

**_Resetting the MySQL root password on Ubuntu involves a slightly different process compared to_** [_**How to Reset MySQL Root Password on Windows**_](https://studygyaan.com/blog/how-to-reset-mysql-password-in-windows)**_. Here’s how you can do it:_**

**Step 1: Stop MySQL Service**: First, you need to stop the MySQL service to perform the password reset. You can do this using the following command:

```
sudo /etc/init.d/mysql stop
```


**Step 2: Start MySQL in Safe Mode**: Start MySQL in safe mode, which doesn’t require a password:

```
sudo mkdir /var/run/mysqld
sudo chown mysql /var/run/mysqld
sudo mysqld_safe --skip-grant-tables&
```


**Step 3: Access MySQL Prompt**: Access the MySQL prompt without a password:

```
sudo mysql --user=root mysql
```


Once you’re in the MySQL prompt, you will be connected to the `mysql` database:

**Step 4: Update Root Password**: Update the root password with the following SQL command. We will set password to null.

```
UPDATE mysql.user SET authentication_string=null WHERE User='root';
```


**Step 5: Flush Privileges:** Flush privileges to apply the changes:

```
FLUSH PRIVILEGES;
```


**Step 6: Set MySQL Root Password**: Once setting password to null, now we will reset password and set to desired value.

```
ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY 'Password@123';
```


**Step 7: Flush Privileges:** Flush privileges to apply the changes:

```
FLUSH PRIVILEGES;
```


**Step 8: Exit MySQL Prompt:** Exit the MySQL prompt:

```
exit;
```


**Step 9: Stop MySQL Safe Mode**: Stop the MySQL safe mode process:

```
sudo killall -KILL mysql mysqld_safe mysqld
```


**Step 10: Start MySQL Service**: Start the MySQL service again:

```
sudo /etc/init.d/mysql start
```


**Step 11: Verify**: Try logging in with the new password to verify that it works:

```
mysql -u root -p
```


These steps should allow you to reset the MySQL root password on Ubuntu. Make sure to replace `Password@123` with your desired strong password.

_Check our blog on [How to Connect MySQL in Visual Studio Code](https://studygyaan.com/blog/connect-mysql-database-in-visual-studio-code)_.







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