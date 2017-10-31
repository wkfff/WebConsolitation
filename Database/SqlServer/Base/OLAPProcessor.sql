/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		OLAPProcessor.sql - Автоматический расчет кубов.
	СУБД	SQL Server 2005
*/

:!!echo ================================================================================
:!!echo  Автоматический расчет кубов.
:!!echo ================================================================================


/* Содержит записи по объектам многомерной базы */
create table OlapObjects
(
	ID					int not null,				/* PK */
	ObjectKey			varchar (36),				/* Уникальный идентификатор объекта */
	ObjectType			int not null,				/* Тип объекта:
														база = 0,
														куб = 1,
														группа мер = 2,
														раздел = 3,
														измерение = 4*/
	ObjectId			varchar (132) not null,		  /* Идентификатор объекта. */
	ObjectName			varchar (132) not null,		  /* Наименование объекта. */
	ParentId			varchar (132),			  /* Идентификатор родителя. */
	ParentName			varchar (132),			  /* Наименование родителя. */
  	Used				numeric (1) default 0 not null,	  /* 1 - если объект должен поддерживаться в актуальном состоянии, 0 - нет. */
	NeedProcess			numeric (1) default 0 not null,	  /* 1 - если объект нуждается в расчете, 0 - нет. */
	State				numeric (10) default 0 not null,  /* Состояние объекта, согласно перечислению Microsoft.AnalysisServices.AnalysisState.  */
	LastProcessed		        datetime,			  /* Дата последнего расчета */
	RefBatchId			varchar (132),			  /* Ссылка на идентификатор пакета */
	ProcessType			int default 0 not null,		  /* Тип рассчета, согласно перечислению Microsoft.AnalysisServices.ProcessType. По умолчанию - ProcessFull */
	ProcessResult		        varchar (2000),			  /* Сообщение об ошибке, если таковая произошла в процессе расчета. */
	Synchronized		        numeric (1) default 1 not null,	  /* 1 - если записи соответсвует объект в многомерной базе, 0 - объекта в многомерной базе нет. */
	FullName			varchar (64),			  /* Полное имя серверного объекта */
	Revision                        varchar(10) default null,         /* № ревизии куба в SVN */
	BatchOperations                 varchar(255) default null,        /* Перечень пользоваельских операции в OlapObjects */
	constraint PKOlapObjects primary key ( ID )
);

create table g.OlapObjects ( ID int identity not null );

go

create trigger t_OlapObjects_bi on OlapObjects instead of insert as
begin
	declare @nullsCount int;
	set nocount on;
	select @nullsCount = count(*) from inserted where ID is null;
	if @nullsCount = 0 insert into OlapObjects (ID, ObjectType, ObjectId, ObjectName, ParentId, ParentName, Used, NeedProcess, State, LastProcessed, RefBatchId, ProcessType, ProcessResult, Synchronized)
		select ID, ObjectType, ObjectId, ObjectName, ParentId, ParentName, Used, NeedProcess, State, LastProcessed, RefBatchId, ProcessType, ProcessResult, Synchronized from inserted;
	else if @nullsCount = 1
	begin
		insert into g.Batch default values;
		delete from g.Batch where ID = @@IDENTITY;
		insert into OlapObjects (ID, ObjectType, ObjectId, ObjectName, ParentId, ParentName, Used, NeedProcess, State, LastProcessed, RefBatchId, ProcessType, ProcessResult, Synchronized)
			select @@IDENTITY, ObjectType, ObjectId, ObjectName, ParentId, ParentName, Used, NeedProcess, State, LastProcessed, RefBatchId, ProcessType, ProcessResult, Synchronized from inserted;
	end else
		RAISERROR (500001, 1, 1);
end;

go

/* Пакеты. */
create table Batch
(
	ID					int not null,				/* PK */
	RefUser				int not null,				/* Пользователь, добавивщий пакет. */
	AdditionTime		datetime not null,			/* Время создания пакета. */
	BatchState			int not null,				/* Состояние пакета:
															редактируется = 0,
															выполняется = 1,
															завершен = 2,
															отменен = 3. */
	SessionId			varchar (132),				/* Идентификатор сессии, в которой идет расчет. */
	BatchId				varchar (132),				/* Идентификатор пакета. */
	Priority			numeric (5) not null,		/* Приоритет */

	constraint PKBatch primary key ( ID ),
	constraint FKBatchRefUser foreign key ( RefUser )
		references Users ( ID ),
	constraint UKBatchId unique (BatchId)
);

create table g.Batch ( ID int identity not null );

go

create trigger t_Batch_bi on Batch instead of insert as
begin
	declare @nullsCount int;
	set nocount on;
	select @nullsCount = count(*) from inserted where ID is null;
	if @nullsCount = 0 insert into Batch (ID, RefUser, AdditionTime, BatchState, SessionId, BatchId, Priority) select ID, RefUser, AdditionTime, BatchState, SessionId, BatchId, Priority from inserted;
	else if @nullsCount = 1
	begin
		insert into g.Batch default values;
		delete from g.Batch where ID = @@IDENTITY;
		insert into Batch (ID, RefUser, AdditionTime, BatchState, SessionId, BatchId, Priority) select @@IDENTITY, RefUser, AdditionTime, BatchState, BatchId, SessionId, Priority from inserted;
	end else
		RAISERROR (500001, 1, 1);
end;

go

/* Накопитель. Содержит сведения обо всех запросах на расчет объектов многомерной базы */
create table Accumulator
(
	ID					int not null,				/* PK */
	ObjectType			int not null,				/* Тип объекта:
																база = 0,
																куб = 1,
																группа мер = 2,
																раздел = 3,
																измерение = 4*/
	DatabaseId			varchar (132) not null,		/* Идентификатор базы. */
	CubeId				varchar (132),				/* Идентификатор куба. Пуст, если объект - измерение. */
	MeasureGroupId		varchar (132),				/* Идентификатор группы мер. Пуст, если объект - измерение. */
	ObjectId			varchar (132) not null,		/* Идентификатор объекта. */
	ObjectName			varchar (132) not null,		/* Наименование объекта. */
	CubeName			varchar (132),				/* Имя куба. Пусто, если объект - измерение. */
	ProcessType			int not null,				/* Тип рассчета, согласно перечислению Microsoft.AnalysisServices.ProcessType. */
	RefBatch			int,						/* Ссылка на пакет */
	State				int not null,				/* Состояние объекта, согласно перечислению Microsoft.AnalysisServices.AnalysisState.  */
	LastProcessed		datetime,					/* Дата последнего расчета */
	RecordStatus		int not null,				/* Состояние записи:
																ожидание = 0,
																в пакете = 1,
																в процессе расчета = 2,
																расчет завершен успешно = 3,
																расчет завершен с ошибками = 4,
																расчет отменен пользователем = 5,
																расчет отменен оптимизацией = 6. */
	ProcessReason		int not null,				/* Причина расчета:
																обратная запись из листа планирования = 0,
																изменен классификатор = 1,
																изменена ссылка на сопоставимый = 2,
																желание пользователя = 3,
																вызов из закачки = 4. */
	RefUser				int not null,				/* Пользователь, добавивщий задачу в накопитель. */
	AdditionTime		datetime not null,			/* Время добавления записи в накопитель. */
	ErrorMessage		varchar (255),				/* Сообщение об ошибке, если таковая произошла в процессе расчета. */
	OptimizationMember	tinyint default 1 not null,	/* 1 - если запись должна обрабатываться оптимизатором, 0 - в противном случае. */
	constraint PKAccumulator primary key ( ID ),
	constraint FKAccumulatorRefUser foreign key ( RefUser )
		references Users ( ID ),
	constraint FKAccumulatorRefBatch foreign key ( RefBatch )
		references Batch ( ID )
);

create table g.Accumulator ( ID int identity not null );

go

create trigger t_Accumulator_bi on Accumulator instead of insert as
begin
	declare @nullsCount int;
	set nocount on;
	select @nullsCount = count(*) from inserted where ID is null;
	if @nullsCount = 0 insert into Accumulator (ID, ObjectType, DatabaseId, CubeId, MeasureGroupId, ObjectId, ObjectName, CubeName, ProcessType, RefBatch, State, LastProcessed, RecordStatus, ProcessReason, RefUser, AdditionTime, ErrorMessage, OptimizationMember) select ID, ObjectType, DatabaseId, CubeId, MeasureGroupId, ObjectId, ObjectName, CubeName, ProcessType, RefBatch, State, LastProcessed, RecordStatus, ProcessReason, RefUser, AdditionTime, ErrorMessage, OptimizationMember from inserted;
	else if @nullsCount = 1
	begin
		insert into g.Accumulator default values;
		delete from g.Accumulator where ID = @@IDENTITY;
		insert into Accumulator (ID, ObjectType, DatabaseId, CubeId, MeasureGroupId, ObjectId, ObjectName, CubeName, ProcessType, RefBatch, State, LastProcessed, RecordStatus, ProcessReason, RefUser, AdditionTime, ErrorMessage, OptimizationMember) select @@IDENTITY, ObjectType, DatabaseId, CubeId, MeasureGroupId, ObjectId, ObjectName, CubeName, ProcessType, RefBatch, State, LastProcessed, RecordStatus, ProcessReason, RefUser, AdditionTime, ErrorMessage, OptimizationMember from inserted;
	end else
		RAISERROR (500001, 1, 1);
end;

go
