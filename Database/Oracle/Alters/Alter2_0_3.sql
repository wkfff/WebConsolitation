/*******************************************************************
 Переводит DV-базу Oracle из версии 2.0.3 в следующую версию 2.1.0
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */



--insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (5, 'X.X.X', SYSDATE, SYSDATE, '');

commit;

/* Start - Нет - Изменения для таблиц с правами/пользователями - borisov - 21.11.05 17:00 */

/* Делаем ID в таблице Memberships - без него не удобно работать с DataUpdater'ом */
/* Таблица гарантированно пустая, можно удолять */
drop table memberships;

create table Memberships
(
	ID					number (10) not null,		/* PK */
	RefUsers			number (10) not null,
	RefGroups			number (10) not null,
	constraint PKMemberships primary key ( ID ),
	constraint FKMembershipsRefUsers foreign key ( RefUsers )
		references Users ( ID ),
	constraint FKMembershipsRefGroups foreign key ( RefGroups )
		references Groups ( ID )
);

create sequence g_Memberships;

create or replace trigger t_Memberships_bi before insert on Memberships for each row
begin
	if :new.ID is null then select g_Memberships.NextVal into :new.ID from Dual; end if;
end t_Memberships_bi;
/

/* Вместо четырех полей под типы операций в таблице Permissions делаем одно */
/* Таблица гарантированно пустая, можно удолять */
drop table permissions;

create table Permissions
(
	ID					number (10) not null,		/* PK */
	RefObjects			number (10) not null,
	RefGroups			number (10),
	RefUsers			number (10),
	AllowedAction		number (10) not null,
	constraint PKPermissions primary key ( ID ),
	constraint FKPermissionsRefGroups foreign key ( RefGroups )
		references Groups ( ID ),
	constraint FKPermissionsRefUsers foreign key ( RefUsers )
		references Users ( ID ),
	constraint FKPermissionsRefObjects foreign key ( RefObjects )
		references  Objects ( ID ),
	constraint CKPermissions check (
		(RefUsers is     null and RefGroups is not null) or
		(RefUsers is not null and RefGroups is null))
);

create or replace trigger t_Permissions_bi before insert on Permissions for each row
begin
	if :new.ID is null then select g_Permissions.NextVal into :new.ID from Dual; end if;
end t_Permissions_bi;
/

commit;
/* End - Нет - Изменения для таблиц с правами/пользователями - borisov - 21.11.05 17:00 */

/* Start - Нет - Каскадное удаление в триггерах таблиц пользователей/прав - borisov - 05.12.05 15:00 */
alter table Memberships drop constraint FKMembershipsRefUsers;
alter table Memberships add constraint FKMembershipsRefUsers foreign key ( RefUsers )
		references Users ( ID ) on delete cascade;

alter table	Memberships drop constraint FKMembershipsRefGroups;
alter table	Memberships add constraint FKMembershipsRefGroups foreign key ( RefGroups )
		references Groups ( ID )  on delete cascade;

alter table Permissions drop constraint FKPermissionsRefGroups;
alter table Permissions add	constraint FKPermissionsRefGroups foreign key ( RefGroups )
		references Groups ( ID ) on delete cascade;

alter table Permissions drop constraint FKPermissionsRefUsers;
alter table Permissions add constraint FKPermissionsRefUsers foreign key ( RefUsers )
		references Users ( ID ) on delete cascade;

alter table Permissions drop constraint FKPermissionsRefObjects;
alter table Permissions add constraint FKPermissionsRefObjects foreign key ( RefObjects )
		references  Objects ( ID ) on delete cascade;

commit;
/* End - Нет - Каскадное удаление в триггерах таблиц пользователей/прав - borisov - 05.12.05 15:00 */

/* Start - Нет - Справочники для пользователей - borisov - 05.12.05 16:00 */
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

commit;
/* End - Нет - Справочники для пользователей - borisov - 05.12.05 16:00 */

/* Start - Нет - Новые поля в таблице пользователей - borisov - 05.12.05 18:00 */
alter table Users add
(
	FirstName 			varchar2 (100), /* Имя */
	LastName			varchar2 (100), /* Фамилия */
	Patronymic			varchar2 (100), /* Отчество */
	JobTitle			varchar2 (255), /* Должность */
	Photo				blob,			/* Фотография */
	RefDepartments		number(10),		/* Отдел */
	RefOrganizations	number(10),		/* Организация */
	constraint FKUsersRefDepartments foreign key ( RefDepartments )
		references Departments ( ID ) on delete set null,
	constraint FKUsersRefOrganizations foreign key ( RefOrganizations )
		references Organizations ( ID ) on delete set null
);

alter table Objects
	modify Name varchar2 (100);

commit;
/* Start - Нет - Новые поля в таблице пользователей - borisov - 05.12.05 18:00 */

/* Start - Нет - Некоторые поля в таблицах пользователей/групп не пригодились - borisov - 09.12.05 15:00 */
alter table GROUPS drop column GroupType;

alter table GROUPS drop column Caption;

alter table USERS drop column Caption;

commit;

/* End - Нет - Некоторые поля в таблице пользователей не пригодились - borisov - 09.12.05 15:00 */

insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (5, '2.1.0', SYSDATE, SYSDATE, '');

commit;
