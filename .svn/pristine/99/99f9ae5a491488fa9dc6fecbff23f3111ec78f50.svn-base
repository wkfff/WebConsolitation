/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		UsersAndGroups.sql - скрипт создания системных таблиц под пользователей, группы, права и пр.
	СУБД	SQL Server 2005
*/

:!!echo ================================================================================
:!!echo  Подсистема пользователей, групп, прав и пр.
:!!echo ================================================================================

create table Organizations
(
	ID					int not null,			/* PK */
	Name				varchar (500) not null,	/* Название организации */
	Description			varchar (2048),			/* Описание организации */
	constraint PKOrganizations primary key ( ID )
);

create table g.Organizations ( ID int identity not null );

create table TasksTypes
(
	ID					int not null,			/* PK */
	Code				int not null,			/* Код */
	Name				varchar (500) not null,	/* Название типа задачи */
	Description			varchar (2048),			/* Описание типа задачи */
	TaskType			int not null,			/* Тип задачи */
	constraint PKTasksTypes primary key ( ID )
);

create table g.TasksTypes ( ID int identity not null );

-- вставляем в справочник задач временную фиктивную запись
insert into TasksTypes (ID, Code, Name, TaskType) values (0, 0, 'Вид задачи не указан', 0);

create table Departments
(
	ID					int not null,			/* PK */
	Name				varchar (500) not null,	/* Название отдела */
	Description			varchar (2048),			/* Описание отдела */
	constraint PKDepartments primary key ( ID )
);

create table g.Departments ( ID int identity not null );

/* Пользователи системы */
create table Users
(
	ID					int not null,			/* PK */
	Name				varchar (255),			/* Логин пользователя */
	Description			varchar (2048),			/* Описание пользователя */
	UserType			int not null,			/* Тип пользователя. Фиксированный список. */
	Blocked				tinyint default 0 not null,	/* Признак активности: 0 - активный, 1 - заблокированный */
	DNSName				varchar (255),			/* DNS имя */
	LastLogin			datetime,				/* Время последнего подключения */
	FirstName 			varchar (100),			/* Имя */
	LastName			varchar (100),			/* Фамилия */
	Patronymic			varchar (100),			/* Отчество */
	JobTitle			varchar (255),			/* Должность */
	Photo				varbinary (max),		/* Фотография */
	PwdHashSHA			varchar (255),			/* Хэш пароля */
	AllowDomainAuth		tinyint default 0 not null,	/* Разрешена доменная аутентификация */
	AllowPwdAuth		tinyint default 0 not null,	/* Разрешена аутентификация по паролю */
	RefDepartments		int,					/* Отдел */
	RefOrganizations	int,					/* Организация */
	RefRegion int,                  /* Ссылка в таблице пользователей на классификатор "Районы.Анализ */
	constraint PKUsers primary key ( ID ),
	constraint FKUsersRefDepartments foreign key ( RefDepartments )
		references Departments ( ID ) on delete set null,
	constraint FKUsersRefOrganizations foreign key ( RefOrganizations )
		references Organizations ( ID ) on delete set null
);

create table g.Users ( ID int identity(100, 1) not null );

/* Создаем фиксированных пользователей */
-- Пользователь не определен
insert into Users (ID, Name, Description, UserType, Blocked)
values (0, 'Пользователь не определен', '', 0, 1);

-- Фиксированный администратор
insert into Users (ID, Name, Description, UserType, Blocked, AllowPwdAuth, PwdHashSha)
values (1, 'FMADMIN', 'Администратор по умолчанию', 0, 0, 1, 'LHP69eGnG4GqrXmVPUsBDPc90eNW/KA9zz/0we8babte9c9DMo+mY1NM1XwkmM67fUUZASVCywIC3Q8hEps+kg==');

-- System (для закачек по расписанию)
insert into Users (ID, Name, Description, UserType, Blocked)
values (2, 'SYSTEM', '', 0, 0);

-- Пользователь для закачек
insert into Users (ID, Name, UserType, Blocked, AllowPwdAuth, PwdHashSha)
values (3, 'Закачка данных', 0, 1, 1, 'Tf9Oo0DwqCPxXT9PAati6uDl2lecy4Ufjbnf6ExYsrN7iZA6dA4e4XLaeTpuedVg5ff5vQWKEqKAQz7W+kZRCg==');

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

/* Группы пользователей */
create table Groups
(
	ID					int not null,			/* PK */
	Name				varchar (255) not null,	/* Английское имя группы */
	Description			varchar (2048),			/* Описание группы */
	Blocked				tinyint default 0 not null,	/* Признак активности: 0 - активный, 1 - заблокированный */
	DNSName				varchar (255),			/* DNS имя */
	constraint PKGroups primary key ( ID )
);

create table g.Groups ( ID int identity not null );

/* Описания объектов */
create table Objects
(
	ID					int not null,			/* PK */
	ObjectKey			varchar (255),			/* Уникальный идентификатор объекта */
	Name				varchar (100) not null,	/* Английское имя */
	Caption				varchar (255) not null,	/* Русское наименование */
	Description			varchar (2048),			/* Описание */
	ObjectType			int not null,			/* Тип объекта. Фиксированный список. */
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

-- регистрируем в качестве объекта системы
insert into Objects (Name, Caption, Description, ObjectType) values ('0_TaskType', 'Вид задачи не указан', 'Вид задачи не указан', 19000);
insert into Objects (Name, Caption, ObjectType, ObjectKey) values ('WebReports', 'Web-интерфейс', 20000, 'WebReports');
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) 
values ('Forecast_Scenario', 'Forecast_Scenario', 'Сценарий', 'Сценарий для прогноза развития региона', 25000);
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) 
values ('Forecast_Valuation', 'Forecast_Valuation', 'Вариант расчета', 'Вариант расчета для прогноза развития региона', 25000);

go

-- регистрируем в качестве объекта системы
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) values ('1_TemplateType', '1_TemplateType', 'Шаблоны отчетов', 'Шаблоны отчетов MDX Эксперта', 22000);
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) values ('2_TemplateType', '2_TemplateType', 'Отчеты системы', 'Отчеты блоков: Источники финансирования', 22000);
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) values ('3_TemplateType', '3_TemplateType', 'Веб-отчеты', 'Отчеты веб-сайта и дашборды', 22000);
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) values ('4_TemplateType', '4_TemplateType', 'iPhone-отчеты', 'Отчеты для отображения на iPhone', 22000);

go


/* Членство */
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

/* Разрешения */
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

