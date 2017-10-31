/*
	��� "������ � ������������"
	������	3.1
	������
		OLAPProcessor.sql - �������������� ������ �����.
	����	Oracle 9.2
*/

pro ================================================================================
pro �������������� ������ �����.
pro ================================================================================

/* ������. */
create table Batch
(
	ID					number (10) not null,		/* PK */
	RefUser				number (10) not null,		/* ������������, ���������� �����. */
	AdditionTime		date not null,				/* ����� �������� ������. */
	BatchState			number (10) not null,		/* ��������� ������:
															������������� = 0,
															����������� = 1,
															�������� = 2,
															������� = 3. */
	SessionId			varchar2 (132),				/* ������������� ������, � ������� ���� ������. */
	BatchId				varchar2 (132),				/* ������������� ������ */
	Priority			number (5) not null,		/* ��������� */
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

/* �������� ������ �� �������� ����������� ���� */
create table OlapObjects
(
	ID					number (10) not null,			/* PK */
	ObjectType			number (10) not null,			/* ��� �������:
																���� = 0,
																��� = 1,
																������ ��� = 2,
																������ = 3,
																��������� = 4*/
	ObjectId			varchar2 (132) not null,		/* ������������� �������. */
	ObjectName			varchar2 (132) not null,		/* ������������ �������. */
	ParentId			varchar2 (132),					/* ������������� ��������. */
	ParentName			varchar2 (132),					/* ������������ ��������. */
	Used				number (1) default 0 not null,	/* 1 - ���� ������ ������ �������������� � ���������� ���������, 0 - ���. */
	NeedProcess			number (1) default 0 not null,	/* 1 - ���� ������ ��������� � �������, 0 - ���. */
	State				number (10) default 0 not null,	/* ��������� �������, �������� ������������ Microsoft.AnalysisServices.AnalysisState.  */
	LastProcessed		date,							/* ���� ���������� ������� */
	RefBatchId			varchar2 (132),					/* ������ �� ������������� ������ */
	ProcessType			number (10) default 0 not null,	/* ��� ��������, �������� ������������ Microsoft.AnalysisServices.ProcessType. �� ��������� - ProcessFull */
	ProcessResult		varchar2 (2000),				/* ��������� �� ������, ���� ������� ��������� � �������� �������. */
	Synchronized		number (1) default 1 not null,	/* 1 - ���� ������ ������������ ������ � ����������� ����, 0 - ������� � ����������� ���� ���. */
	ObjectKey			varchar2 (36),					/* ���������� ������������� ������� ����� */
	FullName			varchar2 (64),					/* ������ ��� ���������� ������� */
	Revision            varchar2(10) default null,      /* � ������� ���� � SVN */
	BatchOperations     varchar2(255) default null,     /* �������� ��������������� �������� */
	constraint PKOlapObjects primary key ( ID )
);

create sequence g_OlapObjects;

create or replace trigger t_OlapObjects_bi before insert on OlapObjects for each row
begin
	if :new.ID is null then select g_OlapObjects.NextVal into :new.ID from Dual; end if;
end t_OlapObjects_bi;
/


commit;