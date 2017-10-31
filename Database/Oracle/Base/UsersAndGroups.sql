/*
	��� "������ � ������������"
	������	3.1
	������
		UsersAndGroups.sql - ������ �������� ��������� ������ ��� �������������, ������, ����� � ��.
	����	Oracle 9.2
*/

pro ================================================================================
pro ������� ��� �������������, ������, ����� � ��.
pro ================================================================================

create table Organizations
(
	ID					number (10) not null,		/* PK */
	Name				varchar2 (500) not null,	/* �������� ����������� */
	Description			varchar2 (2048),			/* �������� ����������� */
	constraint PKOrganizations primary key ( ID )
);

create sequence g_Organizations;

create or replace trigger t_Organizations_bi before insert on Organizations for each row
begin
	if :new.ID is null then select g_Organizations.NextVal into :new.ID from Dual; end if;
end t_Organizations_bi;
/

create table TasksTypes
(
	ID					number (10) not null,		/* PK */
	Code				number (10) not null,		/* ��� */
	Name				varchar2 (500) not null,	/* �������� ���� ������ */
	Description			varchar2 (2048),			/* �������� ���� ������ */
	TaskType			number (10) not null,		/* ��� ������ */
	constraint PKTasksTypes primary key ( ID )
);

create sequence g_TasksTypes;

create or replace trigger t_TasksTypes_bi before insert on TasksTypes for each row
begin
	if :new.ID is null then select g_TasksTypes.NextVal into :new.ID from Dual; end if;
end t_TasksTypes_bi;
/

-- ��������� � ���������� ����� ��������� ��������� ������
insert into TasksTypes (ID, Code, Name, TaskType) values (0, 0, '��� ������ �� ������', 0);

commit;

create table Departments
(
	ID					number (10) not null,		/* PK */
	Name				varchar2 (500) not null,	/* �������� ������ */
	Description			varchar2 (2048),			/* �������� ������ */
	constraint PKDepartments primary key ( ID )
);

create sequence g_Departments;

create or replace trigger t_Departments_bi before insert on Departments for each row
begin
	if :new.ID is null then select g_Departments.NextVal into :new.ID from Dual; end if;
end t_Departments_bi;
/

/* ������������ ������� */
create table Users
(
	ID					number (10) not null,		/* PK */
	Name				varchar2 (255),				/* ����� ������������ */
	Description			varchar2 (2048),			/* �������� ������������ */
	UserType			number (10) not null,		/* ��� ������������. ������������� ������. */
	Blocked				number (1) default 0 not null,	/* ������� ����������: 0 - ��������, 1 - ��������������� */
	DNSName				varchar2 (255),				/* DNS ��� */
	LastLogin			date,						/* ����� ���������� ����������� */
	FirstName 			varchar2 (100),				/* ��� */
	LastName			varchar2 (100),				/* ������� */
	Patronymic			varchar2 (100),				/* �������� */
	JobTitle			varchar2 (255),				/* ��������� */
	Photo				blob,						/* ���������� */
	PwdHashSHA			varchar2 (255),				/* ��� ������ */
	AllowDomainAuth		number (1) default 0 not null,	/* ��������� �������� �������������� */
  AllowPwdAuth		number (1) default 0 not null,	/* ��������� �������������� �� ������ */
	RefDepartments		number(10),					/* ����� */
	RefOrganizations	number(10),					/* ����������� */	
	RefRegion number (10), /*������ � ������� ������������� �� ������������� "������.������*/
	constraint PKUsers primary key ( ID ),
	constraint FKUsersRefDepartments foreign key ( RefDepartments )
		references Departments ( ID ) on delete set null,
	constraint FKUsersRefOrganizations foreign key ( RefOrganizations )
		references Organizations ( ID ) on delete set null
);

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

create sequence g_Users start with 100;

create or replace trigger t_Users_bi before insert on Users for each row
begin
	if :new.ID is null then select g_Users.NextVal into :new.ID from Dual; end if;
end t_Users_bi;
/

/* ������ ������������� */
create table Groups
(
	ID					number (10) not null,		/*  PK */
	Name				varchar2 (255) not null,	/* ���������� ��� ������ */
	Description			varchar2 (2048),			/* �������� ������ */
	Blocked				number (1) default 0 not null,	/* ������� ����������: 0 - ��������, 1 - ��������������� */
	DNSName				varchar2 (255),				/* DNS ��� */
	constraint PKGroups primary key ( ID )
);

create sequence g_Groups;

create or replace trigger t_Groups_bi before insert on Groups for each row
begin
	if :new.ID is null then select g_Groups.NextVal into :new.ID from Dual; end if;
end t_Groups_bi;
/

/* �������� �������� */
create table Objects
(
	ID					number (10) not null,		/* PK */
	ObjectKey			varchar2 (255),				/* ���������� ������������� ������� ����� */
	Name				varchar2 (100) not null,	/* ���������� ��� */
	Caption				varchar2 (255) not null,	/* ������� ������������ */
	Description			varchar2 (2048),			/* �������� */
	ObjectType			number (10) not null,		/* ��� �������. ������������� ������. */
	constraint PKObjects primary key ( ID )
);

create sequence g_Objects;

create or replace trigger t_Objects_bi before insert on Objects for each row
begin
	if :new.ID is null then select g_Objects.NextVal into :new.ID from Dual; end if;
end t_Objects_bi;
/

create or replace trigger t_RegisteredUIModules_d before delete on RegisteredUIModules for each row
begin
	delete from Objects obj where obj.Name = :old.Name;
end t_RegisteredUIModules_d;
/

-- ������������ � �������� ������� �������
insert into Objects (Name, Caption, Description, ObjectType) values ('0_TaskType', '��� ������ �� ������', '��� ������ �� ������', 19000);
insert into Objects (Name, Caption, ObjectType, ObjectKey) values ('WebReports', 'Web-���������', 20000, 'WebReports');
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) 
values ('Forecast_Scenario', 'Forecast_Scenario', '��������', '�������� ��� �������� �������� �������', 25000);
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) 
values ('Forecast_Valuation', 'Forecast_Valuation', '������� �������', '������� ������� ��� �������� �������� �������', 25000);

commit;


/* �������� */
create table Memberships
(
	ID					number (10) not null,		/* PK */
	RefUsers			number (10) not null,
	RefGroups			number (10) not null,
	constraint PKMemberships primary key ( ID ),
	constraint FKMembershipsRefUsers foreign key ( RefUsers )
		references Users ( ID ) on delete cascade,
	constraint FKMembershipsRefGroups foreign key ( RefGroups )
		references Groups ( ID ) on delete cascade
);

create sequence g_Memberships;

create or replace trigger t_Memberships_bi before insert on Memberships for each row
begin
	if :new.ID is null then select g_Memberships.NextVal into :new.ID from Dual; end if;
end t_Memberships_bi;
/

/* ���������� */
create table Permissions
(
	ID					number (10) not null,		/* PK */
	RefObjects			number (10) not null,
	RefGroups			number (10),
	RefUsers			number (10),
	AllowedAction		number (10) not null,
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

create sequence g_Permissions;

create or replace trigger t_Permissions_bi before insert on Permissions for each row
begin
	if :new.ID is null then select g_Permissions.NextVal into :new.ID from Dual; end if;
end t_Permissions_bi;
/

commit work;
