/*
	��� "������ � ������������"
	������	3.1
	������
		Tasks.sql - ������ �������� ������ ��� ������
	����	SQL Server 2005
*/

:!!echo ================================================================================
:!!echo  ���������� �����
:!!echo ================================================================================

/* ������ */
create table Tasks
(
	ID					int not null,			/* PK */
	State				varchar (50) not null,	/* ��������� ������ */
	FromDate			datetime,				/* ���� ���������� - "�" */
	ToDate				datetime,				/* ���� ���������� - "��" */
	Doer				int not null,			/* ����������� (��� �������� ����� ������ ������������� ����������� �� ���� "��������")*/
	Owner				int not null,			/* �������� */
	Headline			varchar (255) not null,	/* ���� ������ */
	Description			varchar (4000),			/* �������� (�����������) */
	Job					varchar (4000) not null,/* ������� */
	Curator				int default 0 not null,	/* ������� */
	LockByUser			int,					/* ������� ����������������� ������������� */
	RefTasks			int,					/* ������������ ������ */
	RefTasksTypes		int not null,			/* ��� ������ */
	constraint PKTasks primary key ( ID ),
	constraint FKTasksRefDoer foreign key ( Doer )
		references Users ( ID ),
	constraint FKTasksRefOwner foreign key ( Owner )
		references Users ( ID ),
	constraint FKTasksRefTasks foreign key ( RefTasks )
		references Tasks ( ID ),
	constraint FKTasksRefCurator foreign key ( Curator )
		references Users ( ID ),
	constraint FKTasksRefLockByUser foreign key ( LockByUser )
		references Users ( ID ),
	constraint FKTasksRefTasksTypes foreign key ( RefTasksTypes )
		references TasksTypes ( ID )
);

create table g.Tasks ( ID int identity not null );

insert into Tasks (ID, State, Doer, Owner, Headline, Job, Curator, RefTasksTypes)
	values (-1, '�������', 3, 3, '������� ������', '������� ������', 3, 0);

go

/* ������� ��� ������������� ��������� ����� */
create table TasksTemp
(
	ID					int not null,		/* PK  - ��������� � ID �������� ������� (Tasks) */
	State				varchar (50) not null,		/* ��������� ������ */
	FromDate			datetime,						/* ���� ���������� - "�" */
	ToDate				datetime,						/* ���� ���������� - "��" */
	Doer				int not null,		/* ����������� (��� �������� ����� ������ ������������� ����������� �� ���� "��������")*/
	Owner				int not null,		/* �������� */
	Headline			varchar (255) not null,	/* ���� ������ */
	Description			varchar (4000),			/* �������� (�����������) */
	Job 				varchar (4000) not null,	/* ������� */
	Curator 			int not null,		/* ������� */
	LockByUser			int,				/* ������� ����������������� ������������� */
	CAction				varchar (250),				/* ����������� �������� */
	CState				varchar (250),				/* ���������� ��������� */
	RefTasks			int,				/* ������������ ������ */
	RefTasksTypes		int not null,		/* ��� ������ */
	constraint PKTasksTemp primary key ( ID ),
	constraint FKTasksTempRefDoer foreign key ( Doer )
		references Users ( ID ),
	constraint FKTasksTempRefOwner foreign key ( Owner )
		references Users ( ID ),
	constraint FKTasksTempRefCurator foreign key ( Curator )
		references Users ( ID ),
	constraint FKTasksTempRefTasks foreign key ( RefTasks )
		references Tasks ( ID ),
	constraint FKTasksTempRefTasksTypes foreign key ( RefTasksTypes )
		references TasksTypes ( ID )
);

/* ��������� ����� */
create table Documents
(
	ID					int not null,			/* PK */
	RefTasks			int not null,			/* ������ �� ������ */
	Name				varchar (255) not null,	/* �������� ��������� */
	SourceFileName		varchar (255) not null,	/* �������� ��������������� ����� */
	DocumentType		tinyint not null,		/* ��� ���������:
														1 - ������������ ��������
														2 - ����� �����
														3 - ��������� ����
														4 - �����
														5 - ����� ����� ������ */
	IsPublic			tinyint default 1 not null,/* ������� "���������/���������" */
	Version				int not null,			/* ������ ��������� */
	Document			varbinary (max),		/* ��� �������� */
	Description			varchar (4000),			/* �������� */
	Ownership			int default 0 not null,	/* �������������� ��������� */
	constraint PKDocuments primary key ( ID ),
	constraint FKDocumentsRefTasks foreign key ( RefTasks )
		references Tasks ( ID ) on delete cascade
);

create table g.Documents ( ID int identity not null );

/* �������-��� ��� ���������� */
create table DocumentsTemp
(
	ID					int not null,			/* PK */
	Name				varchar (255) not null,	/* �������� ��������� */
	SourceFileName		varchar (255) not null,	/* �������� ��������������� ����� */
	DocumentType		tinyint not null,		/* ��� ��������� */
	IsPublic			tinyint default 1 not null,/* ������� "���������/���������" */
	Version				int not null,			/* ������ ��������� */
	Document			varbinary (max),		/* ��� �������� */
	Description			varchar (4000),			/* �������� */
	Ownership			int default 0 not null,	/* �������������� ��������� */
	RefTasksTemp		int not null,			/* ������ ��������� �� ������ */
	constraint PKDocumentsTemp primary key ( ID ),
	constraint FKDocumentsTempRefTasksTemp foreign key ( RefTasksTemp )
		references TasksTemp ( ID ) on delete cascade
);

/* ������� ���������� �����*/
create table TasksParameters
(
	ID					int not null,			/* PK */
	Name				varchar (1000) not null,/* ��������*/
	Dimension			varchar (1000),			/* ��������� � ���������*/
	AllowMultiSelect	tinyint default 0 not null, /* �������� �� ��������.����� (0 - ���, 1 - ��) */
	Description			varchar (2000),			/* ����������� */
	ParamValues			varchar (max),		/* �������� ��������� */
	ParamType			tinyint default 0 not null, /* ��� ��������� (0 - ��������, 1 - ���������)*/
	RefTasks			int not null,			/* ������ �� ������ */
	constraint PKTasksParameters primary key ( ID ),
	constraint FKTasksParametersRefTasks foreign key ( RefTasks )
		references Tasks ( ID ) on delete cascade
);

/* ������������������ ��� ��������� ID ���������� */
create table g.TasksParameters ( ID int identity not null );

/* �������-��� ��� ���������� ����� */
create table TasksParametersTemp
(
	ID					int not null,			/* PK */
	Name				varchar (1000) not null,/* ��������*/
	Dimension			varchar (1000),			/* ��������� � ���������*/
	AllowMultiSelect	tinyint default 0 not null, /* �������� �� ��������.����� (0 - ���, 1 - ��) */
	Description			varchar (2000),			/* ����������� */
	ParamValues			varchar (max),		/* �������� ��������� */
	ParamType			tinyint default 0 not null, /* ��� ��������� (0 - ��������, 1 - ���������)*/
	RefTasks			int not null,			/* ������ �� ������ */
	constraint PKTasksParametersTemp primary key ( ID ),
	constraint FKTsksPrmtrsTmpRefTsksTmp foreign key ( RefTasks )
		references TasksTemp ( ID ) on delete cascade
);

go

/* ��������� ������ ��������� ������ - ������ ����� ������ � ������� ��������� �������, */
/* ������������� ������ �� ���������������� ������������ */
create procedure sp_BeginTaskUpdate
	@taskID int,
	@userID int,
	@CAct varchar (250),
	@CSt varchar (250)
as
begin
	set nocount on;

	declare
		  @lockByUser int;

	-- �������� �� ��������� �� ������ � ������ ����������
	select @lockByUser = LockByUser from Tasks where ID = @taskID;
	if not (@lockByUser is null)
		RAISERROR('������ ��� ��������� � ������ ��������������', 1, 1);

	-- ������������� �������� ���� LockByUser � �������� �������
	update Tasks set LockByUser = @userID where ID = @taskID;

	-- �������� ���������� ������ �� ��������� �������
	insert into TasksTemp
		(ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes, LockByUser)
	select
		ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes, LockByUser
	from Tasks
	where ID = @taskID;

	-- ������������� �������� �������� �������� � ��������� ������
	update TasksTemp set CAction = @CAct, CState = @CSt where ID = @taskID;

	-- �������� ��������� ������ �� ��������� �������
	insert into DocumentsTemp
		(ID, RefTasksTemp, Name, SourceFileName, DocumentType, IsPublic, Version, Document, Description, Ownership)
	select
		ID, RefTasks, Name, SourceFileName, DocumentType, IsPublic, Version, Document, Description, Ownership
	from Documents
	where RefTasks = @taskID;

	-- �������� ��������� �� ��������� �������
	insert into TasksParametersTemp
		(ID, Name, Dimension, AllowMultiSelect, Description, ParamValues, ParamType, RefTasks)
	select
		ID, Name, Dimension, AllowMultiSelect, Description, ParamValues, ParamType, RefTasks
	from TasksParameters
	where RefTasks = @taskID;

end;

go

/* ��������� ����������/������ ��������� ������ - � ����������� �� ���������� ��������� */
/* ������ � �������� �������, ������� ����� ������ � ������� ��������� �������, */
/* �������� ������ �� ���������������� ������������ */
create procedure sp_EndTaskUpdate
	@taskID int,
	@canceled int
as
begin
	set nocount on;

	declare
	    @recCount int,
  		@lockByUser int,
		@newState varchar (50),
		@newFromDate datetime,
		@newToDate datetime,
		@newDoer int,
		@newOwner int,
		@newCurator int,
		@newHeadline varchar (255),
		@newJob varchar (4000),
		@newDescription varchar (4000),
		@newRefTasks int,
		@newRefTasksTypes int

	-- ������� ���� �� ����� ������ � �������� �������
	select @recCount = count(ID) from Tasks where ID = @taskID;

	-- ���� �������� �� �������� - ��������� ������ � �������� �������
	if @canceled = 0
	begin
		-- � ����������� �� ������� ���������/��������� ������
		if @recCount = 0
			insert into Tasks
				(ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes)
			select
				ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes
			from TasksTemp
			where ID = @taskID;
		else
		begin
    		-- �������� ��������� �����
	      	select
	       		@newState = State,
	       		@newFromDate = FromDate,
	       		@newToDate = ToDate,
	       		@newDoer = Doer,
	       		@newOwner = Owner,
	       		@newCurator = Curator,
	       		@newHeadline = Headline,
	       		@newJob = Job,
	       		@newDescription = Description,
	       		@newRefTasks = RefTasks,
	       		@newRefTasksTypes = RefTasksTypes
	       	from
		      	TasksTemp
    		where
	       		ID = @taskID;

			update Tasks
			set
				State = @newState, FromDate = @newFromDate, ToDate = @newToDate, Doer = @newDoer,
				Owner = @newOwner, Curator = @newCurator, Headline = @newHeadline, Job = @newJob,
				Description = @newDescription, RefTasks = @newRefTasks, RefTasksTypes = @newRefTasksTypes
			where
				ID = @taskID;
		end; -- if recCount = 0

		-- ��������� ��������� �� ��������� ������� � ��������
		-- ������� ��� ����
		delete from Documents where RefTasks = @taskID;

		-- ��������� ��� �����
		insert into Documents
			(ID, RefTasks, Name, SourceFileName, DocumentType, IsPublic, Version, Document, Description, Ownership)
		select
			ID, RefTasksTemp, Name, SourceFileName, DocumentType, IsPublic, Version, Document, Description, Ownership
		from DocumentsTemp
		where RefTasksTemp = @taskID;

		-- ��������� ��������� �� ��������� ������� � ��������
		-- ������� ��� ����
		delete from TasksParameters where RefTasks = @taskID;

		-- ��������� ��� �����
		insert into TasksParameters
			(ID, Name, Dimension, AllowMultiSelect, Description, ParamValues, ParamType, RefTasks)
		select
			ID, Name, Dimension, AllowMultiSelect, Description, ParamValues, ParamType, RefTasks
		from TasksParametersTemp
		where RefTasks = @taskID;

	end; -- if @canceled = 0

	-- �������� ���� LockByUser � �������� �������
	update Tasks set LockByUser = null where ID = @taskID;

	-- ������� ������ �� ��������� ������� �����
	delete from TasksTemp where ID = @taskID;

	-- ������� �������������� ���������
	delete from DocumentsTemp where RefTasksTemp = @taskID;

	-- ������� �������������� ���������
	delete from TasksParametersTemp where RefTasks = @taskID;

end;

go

/* �������� */
create table Actions
(
	ID					int not null,			/* PK */
	ActionDate			datetime not null,		/* ���� � ����� ���������� �������� */
	Action				varchar (50) not null,	/* ����������� �������� */
	RefUsers			int not null,			/* ������������ ����������� �������� */
	OldState			varchar (50) not null,	/* ������� ��������� */
	NewState			varchar (50) not null,	/* ����� ��������� */
	RefTasks			int not null,			/* ������ �� ������ */
	constraint PKActions primary key ( ID ),
	constraint FKActionsRefUsers foreign key ( RefUsers )
		references Users ( ID ),
	constraint FKActionsRefTasks foreign key ( RefTasks )
		references Tasks ( ID ) on delete cascade
);

create table g.Actions ( ID int identity not null );

go
