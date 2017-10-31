/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		Audit.sql - Аудит изменений данных.
	СУБД	Oracle 9.2
*/

pro ================================================================================
pro Аудит изменений данных
pro ================================================================================

connect DVAudit/DVAudit@&DatabaseName;

/* Операции по изменению данных */
create table DataOperations
(
	ID number(10) not null ,
	ChangeTime date default SYSDATE not null,
	KindOfOperation number(1) not null,			/* Операция: 0 = insert, 1 = update, 2 = delete */
	ObjectName varchar2 (31) not null,			/* Объект БД */
	ObjectType number (10) default 0 not null,	/* Тип объекта БД. Перечисление Krista.FM.ServerLibrary.DataOperationsObjectTypes */
	UserName varchar2 (64) default USER not null,/* Пользователь */
	SessionID varchar2 (24) not null,			/* ID сессии */
	RecordID number(10) not null,				/* ID записи объекта */
	TaskID number(10),							/* ID задачи (только если с обратной записи; или в интерфейсе таблицы фактов с выбранной задачей) */
	PumpID number(10),							/* ID закачки (заполняется только при закачке) */
	constraint PKDataOperationsID primary key ( ID )
);

create sequence g_DataOperations;

create or replace trigger t_DataOperations_bi before insert on DataOperations for each row
begin
	if :new.ID is null then select g_DataOperations.NextVal into :new.ID from Dual; end if;
end t_DataOperations_bi;
/

connect &UserName/&UserPassword@&DatabaseName;

grant ALL PRIVILEGES on DVAudit.Dataoperations to PUBLIC;

/* Глобальный контекст для хранения параметров текущей сессии */
create or replace context DVContext using DVContext accessed globally;

/* Пакет для доступа к контексту параметров текущей сессии */
create or replace package DVContext as
   procedure SetValue(attribute varchar2, value varchar2, username varchar2, client_id varchar2);
end;
/

create or replace package body DVContext as
	/* Установка значения параметра сессии */
	procedure SetValue(attribute varchar2, value varchar2, username varchar2, client_id varchar2) as
	begin
		DBMS_SESSION.SET_CONTEXT ('DVContext', attribute, value, username, client_id);
	end;
end;
/

