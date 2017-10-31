/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		Audit.sql - Аудит изменений данных.
	СУБД	SQL Server 2005
*/

:!!echo ================================================================================
:!!echo  Аудит изменений данных
:!!echo ================================================================================

use [$(DatabaseName)Audit]
go

/* Операции по изменению данных */
CREATE TABLE [DVAudit].[dataoperations]
   (id                             int IDENTITY(1,1) NOT NULL,
    changetime                     datetime DEFAULT GETDATE() NOT NULL,
    kindofoperation                tinyint NOT NULL,
    objectname                     VARCHAR(31) NOT NULL,
    objecttype                     int DEFAULT 0 NOT NULL,
    username                       VARCHAR(64) DEFAULT USER NOT NULL,
    sessionid                      VARCHAR(24) NOT NULL,
    recordid                       int NOT NULL,
    taskid                         int,
    pumpid                         int
    );
go

ALTER TABLE DVAudit.dataoperations ADD CONSTRAINT PKdataoperations PRIMARY KEY (id);
GO


USE [$(DatabaseName)]
GO

CREATE SYNONYM [DVAudit].[dataoperations] 
  FOR [$(DatabaseName)Audit].[DVAudit].[dataoperations];
GO


