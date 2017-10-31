/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		OLAPProcessor.sql - Автоматический расчет кубов.
	СУБД	Oracle 9.2
*/

pro ================================================================================
pro Автоматический расчет кубов.
pro ================================================================================

/* Пакеты. */
create table Batch
(
	ID					number (10) not null,		/* PK */
	RefUser				number (10) not null,		/* Пользователь, добавивщий пакет. */
	AdditionTime		date not null,				/* Время создания пакета. */
	BatchState			number (10) not null,		/* Состояние пакета:
															редактируется = 0,
															выполняется = 1,
															завершен = 2,
															отменен = 3. */
	SessionId			varchar2 (132),				/* Идентификатор сессии, в которой идет расчет. */
	BatchId				varchar2 (132),				/* Идентификатор пакета */
	Priority			number (5) not null,		/* Приоритет */
	constraint PKBatch primary key ( ID ),
	constraint FKBatchRefUser foreign key ( RefUser )
		references Users ( ID )
);

alter table Batch add constraint UKBatchId unique (BatchId);

create sequence g_Batch;

create or replace trigger t_Batch_bi before insert on Batch for each row
begin
	if :new.ID is null then select g_Batch.NextVal into :new.ID from Dual; end if;
end t_Batch_bi;
/

/* Содержит записи по объектам многомерной базы */
create table OlapObjects
(
	ID					number (10) not null,			/* PK */
	ObjectType			number (10) not null,			/* Тип объекта:
																база = 0,
																куб = 1,
																группа мер = 2,
																раздел = 3,
																измерение = 4*/
	ObjectId			varchar2 (132) not null,		/* Идентификатор объекта. */
	ObjectName			varchar2 (132) not null,		/* Наименование объекта. */
	ParentId			varchar2 (132),					/* Идентификатор родителя. */
	ParentName			varchar2 (132),					/* Наименование родителя. */
	Used				number (1) default 0 not null,	/* 1 - если объект должен поддерживаться в актуальном состоянии, 0 - нет. */
	NeedProcess			number (1) default 0 not null,	/* 1 - если объект нуждается в расчете, 0 - нет. */
	State				number (10) default 0 not null,	/* Состояние объекта, согласно перечислению Microsoft.AnalysisServices.AnalysisState.  */
	LastProcessed		date,							/* Дата последнего расчета */
	RefBatchId			varchar2 (132),					/* Ссылка на идентификатор пакета */
	ProcessType			number (10) default 0 not null,	/* Тип рассчета, согласно перечислению Microsoft.AnalysisServices.ProcessType. По умолчанию - ProcessFull */
	ProcessResult		varchar2 (2000),				/* Сообщение об ошибке, если таковая произошла в процессе расчета. */
	Synchronized		number (1) default 1 not null,	/* 1 - если записи соответсвует объект в многомерной базе, 0 - объекта в многомерной базе нет. */
	ObjectKey			varchar2 (36),					/* Уникальный идентификатор объекта схемы */
	FullName			varchar2 (64),					/* Полное имя серверного объекта */
	Revision            varchar2(10) default null,      /* № ревизии куба в SVN */
	BatchOperations     varchar2(255) default null,     /* Перечень пользоваельских операции */
	constraint PKOlapObjects primary key ( ID )
);

create sequence g_OlapObjects;

create or replace trigger t_OlapObjects_bi before insert on OlapObjects for each row
begin
	if :new.ID is null then select g_OlapObjects.NextVal into :new.ID from Dual; end if;
end t_OlapObjects_bi;
/


commit;