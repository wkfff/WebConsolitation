/*
	��� "������ � ������������"
	������	3.1
	������
		UsersAndGroups.sql - ������ �������� ��������� ������ ��� �������������, ������, ����� � ��.
	����	SQL Server 2005
*/

:!!echo ================================================================================
:!!echo  ���������� �������������, �����, ���� � ��.
:!!echo ================================================================================

create table Organizations
(
	ID					int not null,			/* PK */
	Name				varchar (500) not null,	/* �������� ����������� */
	Description			varchar (2048),			/* �������� ����������� */
	constraint PKOrganizations primary key ( ID )
);

create table g.Organizations ( ID int identity not null );

create table TasksTypes
(
	ID					int not null,			/* PK */
	Code				int not null,			/* ��� */
	Name				varchar (500) not null,	/* �������� ���� ������ */
	Description			varchar (2048),			/* �������� ���� ������ */
	TaskType			int not null,			/* ��� ������ */
	constraint PKTasksTypes primary key ( ID )
);

create table g.TasksTypes ( ID int identity not null );

-- ��������� � ���������� ����� ��������� ��������� ������
insert into TasksTypes (ID, Code, Name, TaskType) values (0, 0, '��� ������ �� ������', 0);

create table Departments
(
	ID					int not null,			/* PK */
	Name				varchar (500) not null,	/* �������� ������ */
	Description			varchar (2048),			/* �������� ������ */
	constraint PKDepartments primary key ( ID )
);

create table g.Departments ( ID int identity not null );

/* ������������ ������� */
create table Users
(
	ID					int not null,			/* PK */
	Name				varchar (255),			/* ����� ������������ */
	Description			varchar (2048),			/* �������� ������������ */
	UserType			int not null,			/* ��� ������������. ������������� ������. */
	Blocked				tinyint default 0 not null,	/* ������� ����������: 0 - ��������, 1 - ��������������� */
	DNSName				varchar (255),			/* DNS ��� */
	LastLogin			datetime,				/* ����� ���������� ����������� */
	FirstName 			varchar (100),			/* ��� */
	LastName			varchar (100),			/* ������� */
	Patronymic			varchar (100),			/* �������� */
	JobTitle			varchar (255),			/* ��������� */
	Photo				varbinary (max),		/* ���������� */
	PwdHashSHA			varchar (255),			/* ��� ������ */
	AllowDomainAuth		tinyint default 0 not null,	/* ��������� �������� �������������� */
	AllowPwdAuth		tinyint default 0 not null,	/* ��������� �������������� �� ������ */
	RefDepartments		int,					/* ����� */
	RefOrganizations	int,					/* ����������� */
	RefRegion int,                  /* ������ � ������� ������������� �� ������������� "������.������ */
	constraint PKUsers primary key ( ID ),
	constraint FKUsersRefDepartments foreign key ( RefDepartments )
		references Departments ( ID ) on delete set null,
	constraint FKUsersRefOrganizations foreign key ( RefOrganizations )
		references Organizations ( ID ) on delete set null
);

create table g.Users ( ID int identity(100, 1) not null );

/* ������� ������������� ������������� */
-- ������������ �� ���������
insert into Users (ID, Name, Description, UserType, Blocked)
values (0, '������������ �� ���������', '', 0, 1);

-- ������������� �������������
insert into Users (ID, Name, Description, UserType, Blocked, AllowPwdAuth, PwdHashSha)
values (1, 'FMADMIN', '������������� �� ���������', 0, 0, 1, 'LHP69eGnG4GqrXmVPUsBDPc90eNW/KA9zz/0we8babte9c9DMo+mY1NM1XwkmM67fUUZASVCywIC3Q8hEps+kg==');

-- System (��� ������� �� ����������)
insert into Users (ID, Name, Description, UserType, Blocked)
values (2, 'SYSTEM', '', 0, 0);

-- ������������ ��� �������
insert into Users (ID, Name, UserType, Blocked, AllowPwdAuth, PwdHashSha)
values (3, '������� ������', 0, 1, 1, 'Tf9Oo0DwqCPxXT9PAati6uDl2lecy4Ufjbnf6ExYsrN7iZA6dA4e4XLaeTpuedVg5ff5vQWKEqKAQz7W+kZRCg==');

go

create trigger t_Users_bi on Users instead of insert as
begin
	declare @nullsCount int;
	set nocount on;
	select @nullsCount = count(*) from inserted where ID is null;
	if @nullsCount = 0 insert into Users (ID, Name, Description, UserType, Blocked, DNSName, LastLogin, FirstName, LastName, Patronymic, JobTitle, Photo, PwdHashSHA, RefDepartments, RefOrganizations) select ID, Name, Description, UserType, Blocked, DNSName, LastLogin, FirstName, LastName, Patronymic, JobTitle, Photo, PwdHashSHA, RefDepartments, RefOrganizations from inserted;
	else if @nullsCount = 1
	begin
		insert into g.Users default values;
		delete from g.Users where ID = @@IDENTITY;
		insert into Users (ID, Name, Description, UserType, Blocked, DNSName, LastLogin, FirstName, LastName, Patronymic, JobTitle, Photo, PwdHashSHA, RefDepartments, RefOrganizations) select @@IDENTITY, Name, Description, UserType, Blocked, DNSName, LastLogin, FirstName, LastName, Patronymic, JobTitle, Photo, PwdHashSHA, RefDepartments, RefOrganizations from inserted;
	end else
		RAISERROR (500001, 1, 1);
end;

go

/* ������ ������������� */
create table Groups
(
	ID					int not null,			/* PK */
	Name				varchar (255) not null,	/* ���������� ��� ������ */
	Description			varchar (2048),			/* �������� ������ */
	Blocked				tinyint default 0 not null,	/* ������� ����������: 0 - ��������, 1 - ��������������� */
	DNSName				varchar (255),			/* DNS ��� */
	constraint PKGroups primary key ( ID )
);

create table g.Groups ( ID int identity not null );

/* �������� �������� */
create table Objects
(
	ID					int not null,			/* PK */
	ObjectKey			varchar (255),			/* ���������� ������������� ������� */
	Name				varchar (100) not null,	/* ���������� ��� */
	Caption				varchar (255) not null,	/* ������� ������������ */
	Description			varchar (2048),			/* �������� */
	ObjectType			int not null,			/* ��� �������. ������������� ������. */
	constraint PKObjects primary key ( ID )
);

create table g.Objects ( ID int identity (3, 1) not null );

go

create trigger t_Objects_bi on Objects instead of insert as
begin
	declare @nullsCount int;
	set nocount on;
	select @nullsCount = count(*) from inserted where ID is null;
	if @nullsCount = 0 insert into Objects (ID, ObjectKey, Name, Caption, Description, ObjectType) select ID, ObjectKey, Name, Caption, Description, ObjectType from inserted;
	else if @nullsCount = 1
	begin
		insert into g.Objects default values;
		delete from g.Objects where ID = @@IDENTITY;
		insert into Objects (ID, ObjectKey, Name, Caption, Description, ObjectType) select @@IDENTITY, ObjectKey, Name, Caption, Description, ObjectType from inserted;
	end else
		RAISERROR (500001, 1, 1);
end;

go

-- ������������ � �������� ������� �������
insert into Objects (Name, Caption, Description, ObjectType) values ('0_TaskType', '��� ������ �� ������', '��� ������ �� ������', 19000);
insert into Objects (Name, Caption, ObjectType, ObjectKey) values ('WebReports', 'Web-���������', 20000, 'WebReports');
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) 
values ('Forecast_Scenario', 'Forecast_Scenario', '��������', '�������� ��� �������� �������� �������', 25000);
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) 
values ('Forecast_Valuation', 'Forecast_Valuation', '������� �������', '������� ������� ��� �������� �������� �������', 25000);

go

-- ������������ � �������� ������� �������
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) values ('1_TemplateType', '1_TemplateType', '������� �������', '������� ������� MDX ��������', 22000);
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) values ('2_TemplateType', '2_TemplateType', '������ �������', '������ ������: ��������� ��������������', 22000);
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) values ('3_TemplateType', '3_TemplateType', '���-������', '������ ���-����� � ��������', 22000);
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) values ('4_TemplateType', '4_TemplateType', 'iPhone-������', '������ ��� ����������� �� iPhone', 22000);

go


/* �������� */
create table Memberships
(
	ID					int not null,			/* PK */
	RefUsers			int not null,
	RefGroups			int not null,
	constraint PKMemberships primary key ( ID ),
	constraint FKMembershipsRefUsers foreign key ( RefUsers )
		references Users ( ID ) on delete cascade,
	constraint FKMembershipsRefGroups foreign key ( RefGroups )
		references Groups ( ID ) on delete cascade
);

create table g.Memberships ( ID int identity not null );

/* ���������� */
create table Permissions
(
	ID					int not null,			/* PK */
	RefObjects			int not null,
	RefGroups			int,
	RefUsers			int,
	AllowedAction		int not null,
	constraint PKPermissions primary key ( ID ),
	constraint FKPermissionsRefGroups foreign key ( RefGroups )
		references Groups ( ID ) on delete cascade,
	constraint FKPermissionsRefUsers foreign key ( RefUsers )
		references Users ( ID ) on delete cascade,
	constraint FKPermissionsRefObjects foreign key ( RefObjects )
		references  Objects ( ID ) on delete cascade,
	constraint CKPermissions check (
		(RefUsers is     null and RefGroups is not null) or
		(RefUsers is not null and RefGroups is null))
);

create table g.Permissions ( ID int identity not null );

go

