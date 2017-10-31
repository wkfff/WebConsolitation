/********************************************************************
    ��������� ���� SQL Server �� ������ 3.0 � ��������� ������ 3.X 
********************************************************************/

/* Start - ���������� ������ - vorontsov - 29.06.2011 */
/*���������/������������� �������������� �� <Database>Audit ��� �������� ������ ������ 
� ������� ������� � �������� �� <Database> ��� ������� � ������� ������*/

/*
:setvar DatabaseName DV
:setvar UserName DV
*/

:Connect $(SERVER_NAME)

use [master];
go

:!!echo Checking the existence of the main database $(DatabaseName)...
if DB_ID ('$(DatabaseName)') is null
begin
  RAISERROR('Missing main database!!!', 20, 1) WITH LOG;
end;
go


:!!echo Checking the existence of the login $(UserName) of the main database ...
if SUSER_ID ('$(UserName)') is null
begin
    RAISERROR('Missing username  $(UserName)!!!', 20, 1) WITH LOG;
end;
go


if DB_ID ('$(DatabaseName)Audit') is not null
begin
    print '�������� ������������ ���� ������ ������ $(DatabaseName)Audit...'
    drop database [$(DatabaseName)Audit];
end;
go

:!!echo Create a new audit database $(DatabaseName)Audit...

declare @DatabasePath varchar(max);
select @DatabasePath = LEFT(T.filename, 1 + LEN(T.filename) - PATINDEX('%[\:]%', REVERSE(T.filename)) )
  from master.sys.sysdatabases T
 where upper(T.name) = UPPER('$(DatabaseName)');
if @DatabasePath is null
begin
  RAISERROR('������ ��� ����������� ���� � ������ ������ ����!!!', 20, 1) WITH LOG;
end;

declare @sql nvarchar(max);
set @sql = 
N'create database [$(DatabaseName)Audit] 
ON  PRIMARY ( NAME = ''DVAudit'', FILENAME = '''+@DatabasePath + N'$(DatabaseName)Audit.mdf'' , SIZE = 5120KB , FILEGROWTH = 1024KB )
LOG ON  ( NAME = ''DVAudit_log'', FILENAME = '''+@DatabasePath + N'$(DatabaseName)Audit_log.ldf'' , SIZE = 1024KB , FILEGROWTH = 10%)
collate Cyrillic_General_CI_AS';
execute sp_executesql @sql;
go


use [$(DatabaseName)Audit]
go

:!!echo �������� ������������ DVAudit ��� ������������� ������ $(UserName)
create user [DVAudit] for login [$(UserName)] with default_schema = [DVAudit];
go

:!!echo �������� ����� DVAudit
create schema [DVAudit] authorization [DVAudit];
go

:!!echo ������ ������������ DVAudit ����������
exec sp_addrolemember 'db_owner', 'DVAudit';
go

:!!echo ������� ������� ������ dataoperations
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

if SCHEMA_ID ('DVAudit') is null
begin
  print 'Creating ''DVAudit'' on main $(DatabaseName)...';
  execute sp_executesql N'CREATE SCHEMA DVAudit AUTHORIZATION DV';
end;
GO


if OBJECT_ID('DVAudit.dataoperations') is not null
begin
  print '�������� ������������� �������� ��� ������� ������...';
  DROP SYNONYM [DVAudit].[dataoperations];
end;
GO

:!!echo �������� �������� ��� ������� ������...
CREATE SYNONYM [DVAudit].[dataoperations] 
  FOR [$(DatabaseName)Audit].[DVAudit].[dataoperations];
GO

if OBJECT_ID('DV.DataOperations') is not null
begin
  print '������� ������������ "������������" ������� ������...';
  drop table DV.DataOperations;
end;

if OBJECT_ID('g.DataOperations') is not null
begin
  print '������� ������������ ��������� "������������" ������� ������...';
  drop table g.DataOperations;
end;


/*insert into DV.DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (99, '3.0.0.4', CONVERT(datetime, '2011.06.29', 102), GETDATE(), '���������� ������', 0);*/

go

/* End - ���������� ������ - vorontsov - 29.06.2011*/