/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.0
	МОДУЛЬ
		CreateNewBase.sql - Скрипт для создания физической схемы базы данных и главного пользователя
	СУБД	Oracle 9.2
*/

pro ================================================================================
pro Создание табличного пространства '&TableSpace' и пользователя '&UserName/&UserPassword@&DatabaseName'
pro ================================================================================

connect system/system@&DatabaseName;

whenever SQLError continue commit

drop tablespace &TableSpace including contents and datafiles cascade constraints;
drop tablespace &IndexTableSpace including contents and datafiles cascade constraints;
drop tablespace DVAudit including contents and datafiles cascade constraints;

create tablespace &TableSpace datafile '&TableSpaceFile' size 3M reuse autoextend on next 1280K maxsize unlimited extent management local segment space management auto;
create tablespace &IndexTableSpace datafile '&IndexTableSpaceFile' size 3M reuse autoextend on next 1280K maxsize unlimited extent management local segment space management auto;
create tablespace &AuditTableSpace datafile '&AuditTableSpaceFile' size 3M reuse autoextend on next 1280K maxsize unlimited extent management local segment space management auto;

drop user &UserName cascade;
drop user DVAudit cascade;

commit work;

alter profile default limit password_life_time unlimited;

create user &UserName identified by &UserPassword default tablespace &TableSpace;
grant DBA to &UserName;

create user DVAudit identified by DVAudit default tablespace DVAudit;
alter user DVAudit quota unlimited on DVAudit;
grant connect to DVAudit;
grant create table to DVAudit;
grant create any index to DVAudit;
grant create trigger to DVAudit;
grant create sequence to DVAudit;

commit work;
