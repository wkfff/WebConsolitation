/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		Templates.sql - Хранилище отчетов.
	СУБД	Oracle 9.2
*/

:!!echo ================================================================================
:!!echo Хранилище отчетов.
:!!echo ================================================================================

-- Виды шаблонов
create table TemplatesTypes
(
	ID					int not null,		/* PK */
	Name				varchar (500) not null,	/* Название вида шаблона */
	Description			varchar (2048),			/* Описание вида шаблона */
	constraint PKTemplatesTypes primary key ( ID )
)

go 

-- вставляем в справочник задач временную фиктивную запись
insert into TemplatesTypes (ID, Name, Description) values (1, 'Шаблоны отчетов', 'Шаблоны отчетов MDX Эксперта');
insert into TemplatesTypes (ID, Name, Description) values (2, 'Отчеты системы', 'Отчеты блоков: Источники финансирования');
insert into TemplatesTypes (ID, Name, Description) values (3, 'Веб-отчеты', 'Отчеты веб-сайта и дашборды');
insert into TemplatesTypes (ID, Name, Description) values (4, 'iPhone-отчеты', 'Отчеты для отображения на iPhone');

go

create table Templates
(
	ID			int  not null,
	ParentID		int,
	Type			int  not null,
	Name			varchar(255) not null,
	Description		varchar(510),
	Document		varbinary (max),
	DocumentFileName varchar (500),
	Editor int default -1,
	Code varchar (100),                -- Код отчета
	SortIndex int,                     -- Поле для сортировки
	Flags int default 0,               -- Поле для флажков (1 - фаворит; 2 - важный; 4 - новый)
	RefTemplatesTypes int not null,    -- Ссылка на справочник типов шаблонов
	constraint PKTemplates primary key (ID),
	constraint FKTemplatesRef foreign key (ParentID)
		references Templates (ID),
	constraint FKTemplatesRefTemplatesTypes foreign key (RefTemplatesTypes)
		references TemplatesTypes (ID)
);

create table g.Templates (ID int identity not null);

go

create trigger t_Templates_bi on Templates instead of insert as
begin
	declare @nullsCount int;
	set nocount on;
	select @nullsCount = count(*) from inserted where ID is null;
	if @nullsCount = 0 insert into Templates (ID, ParentID, Type, Name, Description, Document, RefTemplatesTypes) select ID, ParentID, Type, Name, Description, Document, RefTemplatesTypes from inserted;
	else if @nullsCount = 1
	begin
		insert into g.Templates default values;
		delete from g.Templates where ID = @@IDENTITY;
		insert into Templates (ID, ParentID, Type, Name, Description, Document, RefTemplatesTypes) select @@IDENTITY, ParentID, Type, Name, Description, Document, RefTemplatesTypes from inserted;
	end else
		RAISERROR (500001, 1, 1);
end;

go

create trigger t_Templates_d on Templates instead of delete as
begin
	SET NOCOUNT ON;
	-- Каскадное удаление
	delete Templates where ParentID in (select ID from deleted);
	delete Templates where ID in (select ID from deleted)
end;

go

insert into templates (NAME, TYPE, PARENTID, RefTemplatesTypes) values ('Шаблоны отчетов', 0, NULL, 2);
insert into templates (NAME, TYPE, PARENTID, RefTemplatesTypes) values ('Источники финансирования', 0, @@IDENTITY, 2);

go
