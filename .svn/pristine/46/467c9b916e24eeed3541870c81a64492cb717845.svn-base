/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		Templates.sql - Хранилище отчетов.
	СУБД	Oracle 9.2
*/

pro ================================================================================
pro Хранилище отчетов.
pro ================================================================================

-- Виды шаблонов
create table TemplatesTypes
(
	ID					number (10) not null,		/* PK */
	Name				varchar2 (500) not null,	/* Название вида шаблона */
	Description			varchar2 (2048),			/* Описание вида шаблона */
	constraint PKTemplatesTypes primary key ( ID )
);

create sequence g_TemplatesTypes;

create or replace trigger t_TemplatesTypes_bi before insert on TemplatesTypes for each row
begin
	if :new.ID is null then select g_TemplatesTypes.NextVal into :new.ID from Dual; end if;
end t_TemplatesTypes_bi;
/

-- вставляем в справочник задач временную фиктивную запись
insert into TemplatesTypes (ID, Name, Description) values (1, 'Шаблоны отчетов', 'Шаблоны отчетов MDX Эксперта');
insert into TemplatesTypes (ID, Name, Description) values (2, 'Отчеты системы', 'Отчеты блоков: Источники финансирования');
insert into TemplatesTypes (ID, Name, Description) values (3, 'Веб-отчеты', 'Отчеты веб-сайта и дашборды');
insert into TemplatesTypes (ID, Name, Description) values (4, 'iPhone-отчеты', 'Отчеты для отображения на iPhone');

commit;

create sequence g_Templates;

create table Templates
(
	ID				number (10)  not null,
	ParentID		number (10),
	Type			number (10)  not null,
	Name			varchar2 (255) not null,
	Description		varchar2 (510),
	Document		blob,
	DocumentFileName	varchar2 (500),
	Editor			number (10) default -1,
	Code varchar2 (100),                                -- Код отчета
	SortIndex number (10),                              -- Поле для сортировки
	Flags number (10) default 0,                        -- Поле для флажков (1 - фаворит; 2 - важный; 4 - новый)
	RefTemplatesTypes number (10) not null,
	constraint PKTemplates primary key (ID),
	constraint FKTemplatesRef foreign key (ParentID)
		references Templates (ID) on delete cascade,
	constraint FKTemplatesRefTemplatesTypes foreign key (RefTemplatesTypes)
		references TemplatesTypes (ID)
);

/* триггер на добавление новых отчетов */
create or replace trigger t_Templates_bi before insert on Templates for each row
begin
	if :new.ID is null then select g_Templates.NextVal into :new.ID from Dual;
	end if;
end t_Templates_bi;
/

-- Вставляем фиксированные записи "Шаблоны отчетов\Источники финансирования"
declare
	NewID pls_integer;
begin
	select g_Templates.NextVal into NewID from Dual;

	insert into templates (ID, NAME, TYPE, PARENTID, RefTemplatesTypes) 
		values (NewID , 'Шаблоны отчетов', 0, null, 2);
	insert into templates (NAME, TYPE, PARENTID, RefTemplatesTypes) 
		values ('Источники финансирования', 0, NewID, 2);
end;
/

commit;

create or replace trigger t_Templates_AA before insert OR update OR delete on Templates for each row
declare
	UserName varchar2(64);
	SessionID varchar2(24);
begin
	SELECT SYS_CONTEXT ('DVContext', 'USERNAME'), SYS_CONTEXT('DVContext', 'SESSIONID') into UserName, SessionID FROM dual;
	if inserting then insert into DVAudit.DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (0, 'Templates', 5, UserName, SessionID, :new.ID);
	elsif updating then insert into DVAudit.DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (1, 'Templates', 5, UserName, SessionID, :new.ID);
	elsif deleting then insert into DVAudit.DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (2, 'Templates', 5, UserName, SessionID, :old.ID); end if;
end t_Templates_AA;
/

commit;



