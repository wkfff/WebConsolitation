/*
	��� "������ � ������������"
	������	3.1
	������
		OLAPProcessor.sql - �������������� ������ �����.
	����	SQL Server 2005
*/

:!!echo ================================================================================
:!!echo  �������������� ������ �����.
:!!echo ================================================================================


/* �������� ������ �� �������� ����������� ���� */
create table OlapObjects
(
	ID					int not null,				/* PK */
	ObjectKey			varchar (36),				/* ���������� ������������� ������� */
	ObjectType			int not null,				/* ��� �������:
														���� = 0,
														��� = 1,
														������ ��� = 2,
														������ = 3,
														��������� = 4*/
	ObjectId			varchar (132) not null,		  /* ������������� �������. */
	ObjectName			varchar (132) not null,		  /* ������������ �������. */
	ParentId			varchar (132),			  /* ������������� ��������. */
	ParentName			varchar (132),			  /* ������������ ��������. */
  	Used				numeric (1) default 0 not null,	  /* 1 - ���� ������ ������ �������������� � ���������� ���������, 0 - ���. */
	NeedProcess			numeric (1) default 0 not null,	  /* 1 - ���� ������ ��������� � �������, 0 - ���. */
	State				numeric (10) default 0 not null,  /* ��������� �������, �������� ������������ Microsoft.AnalysisServices.AnalysisState.  */
	LastProcessed		        datetime,			  /* ���� ���������� ������� */
	RefBatchId			varchar (132),			  /* ������ �� ������������� ������ */
	ProcessType			int default 0 not null,		  /* ��� ��������, �������� ������������ Microsoft.AnalysisServices.ProcessType. �� ��������� - ProcessFull */
	ProcessResult		        varchar (2000),			  /* ��������� �� ������, ���� ������� ��������� � �������� �������. */
	Synchronized		        numeric (1) default 1 not null,	  /* 1 - ���� ������ ������������ ������ � ����������� ����, 0 - ������� � ����������� ���� ���. */
	FullName			varchar (64),			  /* ������ ��� ���������� ������� */
	Revision                        varchar(10) default null,         /* � ������� ���� � SVN */
	BatchOperations                 varchar(255) default null,        /* �������� ��������������� �������� � OlapObjects */
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

/* ������. */
create table Batch
(
	ID					int not null,				/* PK */
	RefUser				int not null,				/* ������������, ���������� �����. */
	AdditionTime		datetime not null,			/* ����� �������� ������. */
	BatchState			int not null,				/* ��������� ������:
															������������� = 0,
															����������� = 1,
															�������� = 2,
															������� = 3. */
	SessionId			varchar (132),				/* ������������� ������, � ������� ���� ������. */
	BatchId				varchar (132),				/* ������������� ������. */
	Priority			numeric (5) not null,		/* ��������� */

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

/* ����������. �������� �������� ��� ���� �������� �� ������ �������� ����������� ���� */
create table Accumulator
(
	ID					int not null,				/* PK */
	ObjectType			int not null,				/* ��� �������:
																���� = 0,
																��� = 1,
																������ ��� = 2,
																������ = 3,
																��������� = 4*/
	DatabaseId			varchar (132) not null,		/* ������������� ����. */
	CubeId				varchar (132),				/* ������������� ����. ����, ���� ������ - ���������. */
	MeasureGroupId		varchar (132),				/* ������������� ������ ���. ����, ���� ������ - ���������. */
	ObjectId			varchar (132) not null,		/* ������������� �������. */
	ObjectName			varchar (132) not null,		/* ������������ �������. */
	CubeName			varchar (132),				/* ��� ����. �����, ���� ������ - ���������. */
	ProcessType			int not null,				/* ��� ��������, �������� ������������ Microsoft.AnalysisServices.ProcessType. */
	RefBatch			int,						/* ������ �� ����� */
	State				int not null,				/* ��������� �������, �������� ������������ Microsoft.AnalysisServices.AnalysisState.  */
	LastProcessed		datetime,					/* ���� ���������� ������� */
	RecordStatus		int not null,				/* ��������� ������:
																�������� = 0,
																� ������ = 1,
																� �������� ������� = 2,
																������ �������� ������� = 3,
																������ �������� � �������� = 4,
																������ ������� ������������� = 5,
																������ ������� ������������ = 6. */
	ProcessReason		int not null,				/* ������� �������:
																�������� ������ �� ����� ������������ = 0,
																������� ������������� = 1,
																�������� ������ �� ������������ = 2,
																������� ������������ = 3,
																����� �� ������� = 4. */
	RefUser				int not null,				/* ������������, ���������� ������ � ����������. */
	AdditionTime		datetime not null,			/* ����� ���������� ������ � ����������. */
	ErrorMessage		varchar (255),				/* ��������� �� ������, ���� ������� ��������� � �������� �������. */
	OptimizationMember	tinyint default 1 not null,	/* 1 - ���� ������ ������ �������������� �������������, 0 - � ��������� ������. */
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
