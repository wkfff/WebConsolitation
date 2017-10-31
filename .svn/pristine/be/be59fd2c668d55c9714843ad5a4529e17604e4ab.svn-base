/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		UsersAndGroups.sql - скрипт создания системных таблиц под пользователей, группы, права и пр.
	СУБД	Oracle 9.2
*/

pro ================================================================================
pro Таблицы под пользователей, группы, права и пр.
pro ================================================================================

create table Organizations
(
	ID					number (10) not null,		/* PK */
	Name				varchar2 (500) not null,	/* Название организации */
	Description			varchar2 (2048),			/* Описание организации */
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
	Code				number (10) not null,		/* Код */
	Name				varchar2 (500) not null,	/* Название типа задачи */
	Description			varchar2 (2048),			/* Описание типа задачи */
	TaskType			number (10) not null,		/* Тип задачи */
	constraint PKTasksTypes primary key ( ID )
);

create sequence g_TasksTypes;

create or replace trigger t_TasksTypes_bi before insert on TasksTypes for each row
begin
	if :new.ID is null then select g_TasksTypes.NextVal into :new.ID from Dual; end if;
end t_TasksTypes_bi;
/

-- вставляем в справочник задач временную фиктивную запись
insert into TasksTypes (ID, Code, Name, TaskType) values (0, 0, 'Вид задачи не указан', 0);

commit;

create table Departments
(
	ID					number (10) not null,		/* PK */
	Name				varchar2 (500) not null,	/* Название отдела */
	Description			varchar2 (2048),			/* Описание отдела */
	constraint PKDepartments primary key ( ID )
);

create sequence g_Departments;

create or replace trigger t_Departments_bi before insert on Departments for each row
begin
	if :new.ID is null then select g_Departments.NextVal into :new.ID from Dual; end if;
end t_Departments_bi;
/

/* Пользователи системы */
create table Users
(
	ID					number (10) not null,		/* PK */
	Name				varchar2 (255),				/* Логин пользователя */
	Description			varchar2 (2048),			/* Описание пользователя */
	UserType			number (10) not null,		/* Тип пользователя. Фиксированный список. */
	Blocked				number (1) default 0 not null,	/* Признак активности: 0 - активный, 1 - заблокированный */
	DNSName				varchar2 (255),				/* DNS имя */
	LastLogin			date,						/* Время последнего подключения */
	FirstName 			varchar2 (100),				/* Имя */
	LastName			varchar2 (100),				/* Фамилия */
	Patronymic			varchar2 (100),				/* Отчество */
	JobTitle			varchar2 (255),				/* Должность */
	Photo				blob,						/* Фотография */
	PwdHashSHA			varchar2 (255),				/* Хэш пароля */
	AllowDomainAuth		number (1) default 0 not null,	/* Разрешена доменная аутентификация */
  AllowPwdAuth		number (1) default 0 not null,	/* Разрешена аутентификация по паролю */
	RefDepartments		number(10),					/* Отдел */
	RefOrganizations	number(10),					/* Организация */	
	RefRegion number (10), /*ссылка в таблице пользователей на классификатор "Районы.Анализ*/
	constraint PKUsers primary key ( ID ),
	constraint FKUsersRefDepartments foreign key ( RefDepartments )
		references Departments ( ID ) on delete set null,
	constraint FKUsersRefOrganizations foreign key ( RefOrganizations )
		references Organizations ( ID ) on delete set null
);

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

create sequence g_Users start with 100;

create or replace trigger t_Users_bi before insert on Users for each row
begin
	if :new.ID is null then select g_Users.NextVal into :new.ID from Dual; end if;
end t_Users_bi;
/

/* Группы пользователей */
create table Groups
(
	ID					number (10) not null,		/*  PK */
	Name				varchar2 (255) not null,	/* Английское имя группы */
	Description			varchar2 (2048),			/* Описание группы */
	Blocked				number (1) default 0 not null,	/* Признак активности: 0 - активный, 1 - заблокированный */
	DNSName				varchar2 (255),				/* DNS имя */
	constraint PKGroups primary key ( ID )
);

create sequence g_Groups;

create or replace trigger t_Groups_bi before insert on Groups for each row
begin
	if :new.ID is null then select g_Groups.NextVal into :new.ID from Dual; end if;
end t_Groups_bi;
/

/* Описания объектов */
create table Objects
(
	ID					number (10) not null,		/* PK */
	ObjectKey			varchar2 (255),				/* Уникальный идентификатор объекта схемы */
	Name				varchar2 (100) not null,	/* Английское имя */
	Caption				varchar2 (255) not null,	/* Русское наименование */
	Description			varchar2 (2048),			/* Описание */
	ObjectType			number (10) not null,		/* Тип объекта. Фиксированный список. */
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

-- регистрируем в качестве объекта системы
insert into Objects (Name, Caption, Description, ObjectType) values ('0_TaskType', 'Вид задачи не указан', 'Вид задачи не указан', 19000);
insert into Objects (Name, Caption, ObjectType, ObjectKey) values ('WebReports', 'Web-интерфейс', 20000, 'WebReports');
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) 
values ('Forecast_Scenario', 'Forecast_Scenario', 'Сценарий', 'Сценарий для прогноза развития региона', 25000);
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) 
values ('Forecast_Valuation', 'Forecast_Valuation', 'Вариант расчета', 'Вариант расчета для прогноза развития региона', 25000);

commit;


/* Членство */
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

/* Разрешения */
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
