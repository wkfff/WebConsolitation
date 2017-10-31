/*
:setvar ServerName MRRSERV\SQLSERVER2005
:setvar UserName DV
:setvar DatabaseName DV
*/

:setvar DatabasePath D:\MSSQL\DATA

use [master];

if SUSER_ID ('$(UserName)') is not null
begin
	:!!echo Удаление логина $(UserName)...
	drop login $(UserName)
end
go

if DB_ID ('$(DatabaseName)') is not null
begin
	:!!echo Удаление базы данных $(DatabaseName)...
	drop database [$(DatabaseName)];
end
go


:!!echo Создание базы данных $(DatabaseName)...
create database $(DatabaseName)
ON  PRIMARY ( NAME = N'DV', FILENAME = N'$(DatabasePath)\$(DatabaseName)\$(DatabaseName).mdf' , SIZE = 5072KB , FILEGROWTH = 1024KB )
LOG ON  ( NAME = N'DV_log', FILENAME = N'$(DatabasePath)\$(DatabaseName)\$(DatabaseName)_log.ldf' , SIZE = 1024KB , FILEGROWTH = 10%)
collate Cyrillic_General_CI_AS
go

use $(DatabaseName)
go

:!!echo Создание логина $(UserName)...
create login $(UserName)
	with password = '$(UserName)',
	default_database = $(DatabaseName),
	default_language = [русский],
	check_policy=off
go

:!!echo Создание пользователя DV с логином $(UserName)
create user DV for login $(UserName) with default_schema = DV
go

:!!echo Создание схемы 'DV'
create schema DV authorization DV
go

:!!echo Создание схемы 'g'
create schema g authorization DV
go

:!!echo Создание схемы 'DVAudit'
create schema DVAudit authorization DV
go

:!!echo Делаем пользователя DV владельцем
exec sp_addrolemember 'db_owner', 'DV'
go


use [master];
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
  RAISERROR('Ошибка при определении пути к файлам данных базы!!!', 20, 1) WITH LOG;
end;

declare @sql nvarchar(max);
set @sql = 
N'create database [$(DatabaseName)Audit] 
ON  PRIMARY ( NAME = ''DVAudit'', FILENAME = '''+@DatabasePath + N'$(DatabaseName)Audit.mdf'' , SIZE = 5072KB , FILEGROWTH = 1024KB )
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


--exec dbo.sp_changedbowner @loginame = 'BUILTIN\Administrators', @map = false
--go
