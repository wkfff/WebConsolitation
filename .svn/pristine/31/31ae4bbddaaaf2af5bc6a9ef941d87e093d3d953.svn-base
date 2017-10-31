/********************************************************************
    Переводит базу SQL Server из версии 3.0 в следующую версию 3.X 
********************************************************************/

/* Start - Добавление аудита - vorontsov - 29.06.2011 */
/*Создается/пересоздается дополнительная БД <Database>Audit для хранения данных аудита 
и создает синоним в основной БД <Database> для доступа к таблице аудита*/

/*
:setvar DatabaseName DV
:setvar UserName DV
*/

:Connect $(SERVER_NAME)

use [master];
go

:!!echo Проверка существования основной БД $(DatabaseName)...
if DB_ID ('$(DatabaseName)') is null
begin
  RAISERROR('Отсутствует основная БД!!!', 20, 1) WITH LOG;
end;
go


:!!echo Проверка существования логина $(UserName) основной БД ...
if SUSER_ID ('$(UserName)') is null
begin
    RAISERROR('Отсутствует логин  $(UserName)!!!', 20, 1) WITH LOG;
end;
go


if DB_ID ('$(DatabaseName)Audit') is not null
begin
    print 'Удаление существующей базы данных аудита $(DatabaseName)Audit...'
    drop database [$(DatabaseName)Audit];
end;
go

:!!echo Создание новой базы данных аудита $(DatabaseName)Audit...

declare @DatabasePath varchar(max);
select @DatabasePath = LEFT(T.filename, 1 + LEN(T.filename) - PATINDEX('%[\:]%', REVERSE(T.filename)) )
  from master.sys.sysdatabases T
 where upper(T.name) = UPPER('$(DatabaseName)');
if @DatabasePath is null
begin
  RAISERROR('Ошибка при определении пути к файлам данным базы!!!', 20, 1) WITH LOG;
end;

declare @sql nvarchar(max);
set @sql = 
N'create database [$(DatabaseName)Audit] 
ON  PRIMARY ( NAME = ''DVAudit'', FILENAME = '''+@DatabasePath + N'$(DatabaseName)Audit.mdf'' , SIZE = 3072KB , FILEGROWTH = 1024KB )
LOG ON  ( NAME = ''DVAudit_log'', FILENAME = '''+@DatabasePath + N'$(DatabaseName)Audit_log.ldf'' , SIZE = 1024KB , FILEGROWTH = 10%)
collate Cyrillic_General_CI_AS';
execute sp_executesql @sql;
go


use [$(DatabaseName)Audit]
go

:!!echo Создание пользователя DVAudit для существующего логина $(UserName)
create user [DVAudit] for login [$(UserName)] with default_schema = [DVAudit];
go

:!!echo Создание схемы DVAudit
create schema [DVAudit] authorization [DVAudit];
go

:!!echo Делаем пользователя DVAudit владельцем
exec sp_addrolemember 'db_owner', 'DVAudit';
go

:!!echo Создаем таблицу аудита dataoperations
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
  print 'Создание схемы ''DVAudit'' на основной БД $(DatabaseName)...';
  execute sp_executesql N'CREATE SCHEMA DVAudit AUTHORIZATION DV';
end;
GO


if OBJECT_ID('DVAudit.dataoperations') is not null
begin
  print 'Удаление существующего синонима для таблицы аудита...';
  DROP SYNONYM [DVAudit].[dataoperations];
end;
GO

:!!echo Создание синонима для таблицы аудита...
CREATE SYNONYM [DVAudit].[dataoperations] 
  FOR [$(DatabaseName)Audit].[DVAudit].[dataoperations];
GO

if OBJECT_ID('DV.DataOperations') is not null
begin
  print 'Удаляем существующую "неправильную" таблицу аудита...';
  drop table DV.DataOperations;
end;

if OBJECT_ID('g.DataOperations') is not null
begin
  print 'Удаляем существующий генератор "неправильной" таблицы аудита...';
  drop table g.DataOperations;
end;


if not exists (select V.id from DV.DatabaseVersions V where V.ID = 99)
begin 
  insert into DV.DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
  values (99, '3.0.0.4', CONVERT(datetime, '2011.06.29', 102), GETDATE(), 'Добавление аудита', 0);
end
else
begin
  print 'Запись о применении патча в DatabaseVersions уже существует...';
end;
go

/* End - Добавление аудита - vorontsov - 29.06.2011*/
