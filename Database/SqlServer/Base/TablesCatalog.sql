/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		TablesCatalog.sql - скрипт каталога метаданных
	СУБД	SQL Server 2005
*/

:!!echo ================================================================================
:!!echo  Подсистема метаданных
:!!echo ================================================================================

/* Пакеты */
create table MetaPackages
(
	ID					int not null,
	ObjectKey 			varchar (36) default '00000000-0000-0000-0000-000000000000' not null, /* Уникальный идентификатор объекта */
	Name				varchar (100) not null,		/* Наименование пакета */
	BuiltIn				tinyint not null,			/* Встроенный или расположен в отдельном файле */
	RefParent			int,						/* Родительский пакет */
	PrivatePath			varchar (1000),				/* Относительный путь к файлу пакета */
	OrderBy 			int default 0,				/* Задает последовательность инициализации пакетов */
	Configuration		varchar (max),				/* XML настройка элемента, часть XML=UML */
	constraint PKMetaPackages primary key ( ID ),
	constraint FKMetaPackagesRefParent foreign key ( RefParent )
		references MetaPackages ( ID ) /*on delete cascade*/
);

create table g.MetaPackages ( ID int identity (2, 1) not null );

go

CREATE TRIGGER t_MetaPackages_d ON MetaPackages INSTEAD OF DELETE
AS
BEGIN
	SET NOCOUNT ON;
	-- Каскадное удаление
	delete MetaObjects where RefPackages in (select ID from deleted)
	delete MetaDocuments where RefPackages in (select ID from deleted)
        -- удаляем объекты у вложенных пакетов
	delete MetaObjects where RefPackages in (select ID from MetaPackages where RefParent in (select ID from deleted))
	delete MetaDocuments where RefPackages in (select ID from MetaPackages where RefParent in (select ID from deleted))
	-- удаляем вложенные пакеты
	delete MetaPackages where RefParent in (select ID from deleted)
	delete MetaPackages where ID in (select ID from deleted)
END
GO

insert into MetaPackages (ID, ObjectKey, Name, BuiltIn, RefParent, Configuration) values (1, 'c7a4196e-568e-482f-8219-921423b8a77a', 'Системные объекты', 1, null, null);

/* Объекты */
create table MetaObjects
(
	ID					int not null,
	ObjectKey 			varchar (36) default '00000000-0000-0000-0000-000000000000' not null, /* Уникальный идентификатор объекта */
	Semantic			varchar (26) not null,		/* Английнкое семантическое наименование объекта в БД */
	Name				varchar (26) not null,		/* Английнкое наименование объекта в БД */
	Class				numeric (2) default 0 not null,/* Класс объекта:
														0 - сопоставимый классификатор */
	SubClass			numeric (2) default 0 not null,/* Подкласс объекта:
														0 - */
	Configuration		varchar (max),				/* XML настройка элемента, часть XML=UML */
	RefPackages			int not null,				/* Пакет, в котором определен объект */
	constraint PKMetaObjects primary key ( ID ),
	constraint FKMetaObjectsRefPackages foreign key ( RefPackages )
		references MetaPackages ( ID ) /*on delete cascade*/
);

create table g.MetaObjects ( ID int identity (4, 1) not null );

go

CREATE TRIGGER t_MetaObjects_d ON MetaObjects INSTEAD OF DELETE
AS
BEGIN
	SET NOCOUNT ON;
	-- Каскадное удаление
	delete MetaLinks where RefParent in (select ID from deleted)
	delete MetaLinks where RefChild in (select ID from deleted)
	delete MetaObjects where ID in (select ID from deleted)
END
GO

/* Описание связеи сопоставления сопоставимых классификаторов данных с сопоставимыми классификаторами данных */
create table MetaLinks
(
	ID					int not null,
	ObjectKey 			varchar (36) default '00000000-0000-0000-0000-000000000000' not null, /* Уникальный идентификатор объекта */
	Semantic			varchar (26) not null,		/* Английнкое семантическое наименование объекта в БД */
	Name				varchar (60) not null,		/* Наименование ассоциации */
	Class				numeric (2) default 0 not null,/* Класс объекта */
	RefParent			int not null,				/* Роль A. Ссылка на идентификатор сопоставимого классификатора (сопоставимый) */
	RefChild			int not null,				/* Роль B. Ссылка на идентификатор сопоставимого классификатора (сопоставляемый) */
	Configuration		varchar (max),				/* XML настройка элемента, часть XML=UML */
	RefPackages			int not null,				/* Пакет, в котором определен объект */
	DefaultAssociateRule varchar (255), /* Уникальный идентификатор правила сопоставления по умолчанию */
	constraint PKMetaLinks primary key ( ID ),
	constraint FKMetaLinksRefParent foreign key ( RefParent )
		references MetaObjects ( ID ) /*on delete cascade*/,
	constraint FKMetaLinksRefChild foreign key ( RefChild )
		references MetaObjects ( ID ) /*on delete cascade*/,
	constraint FKMetaLinksRefPackages foreign key ( RefPackages )
		references MetaPackages ( ID ) on delete cascade
);

create table g.MetaLinks ( ID int identity not null );

insert into MetaObjects (ID, ObjectKey, Semantic, Name, Class, SubClass, RefPackages) values (0, 'b4612528-0e51-4e6b-8891-64c22611816b', 'Date', 'YearDayUNV', 2, 4, 1);
insert into MetaObjects (ID, ObjectKey, Semantic, Name, Class, SubClass, RefPackages) values (1, '675ede52-a0b4-423a-b0f6-365ad02d0f6f', 'Date', 'YearDay', 2, 4, 1);
insert into MetaObjects (ID, ObjectKey, Semantic, Name, Class, SubClass, RefPackages) values (2, '30fd54c6-de78-4664-afb2-d5fcadc10e9c', 'Date', 'YearMonth', 2, 4, 1);
insert into MetaObjects (ID, ObjectKey, Semantic, Name, Class, SubClass, RefPackages) values (3, 'c66d6056-7282-4ab0-ab0b-ea43ad68cb4c', 'Date', 'Year', 2, 4, 1);

go

create view MetaLinksWithRolesNames (ID, Semantic, Name, Class, ObjectKey, RoleAName, RoleBName, RoleAObjectKey, RoleBObjectKey, RefPackages, Configuration) as
select
	L.ID, L.Semantic, L.Name, L.Class, L.ObjectKey,
	case OP.Class when 0 then 'b' when 1 then 'd' when 2 then 'fx' when 3 then 'f' when 10 then 't' end + '.' + OP.Semantic + '.' + OP.Name,
	case OC.Class when 0 then 'b' when 1 then 'd' when 2 then 'fx' when 3 then 'f' when 10 then 't' end + '.' + OC.Semantic + '.' + OC.Name,
	OP.ObjectKey, OC.ObjectKey,
	L.RefPackages, L.Configuration
from MetaLinks L join MetaObjects OP on (OP.ID = L.RefParent) join MetaObjects OC on (OC.ID = L.RefChild);

go

/* Документы */
create table MetaDocuments
(
	ID					int not null,
	ObjectKey 			varchar (36) default '00000000-0000-0000-0000-000000000000' not null, /* Уникальный идентификатор объекта */
	Name				varchar (100) not null,		/* Наименование документа */
	DocType				int not null,				/* Тип документа */
	RefPackages			int not null,				/* родительский пакет */
	Configuration		varchar (max),				/* XML настройка элемента, часть XML=UML */
	constraint PKMetaDocuments primary key ( ID ),
	constraint FKMetaDocumentsRefPackages foreign key ( RefPackages )
		references MetaPackages ( ID ) on delete cascade
);

create table g.MetaDocuments ( ID int identity not null );

/* ================================================================================
	Таблицы перекодировок
   ================================================================================ */

/* Каталог таблиц перекодировок */
create table MetaConversionTable
(
	ID					int not null,				/* PKID */
	RefAssociation		int not null,				/* Ассоциация для которой применяется правило */
	AssociateRule		varchar (50) not null,		/* Наименование правила сопоставления */
	constraint PKMetaConversionTable primary key ( ID ),
	constraint FKMetaCTRefAssociation foreign key ( RefAssociation )
		references MetaLinks ( ID ) on delete cascade
);

create table g.MetaConversionTable ( ID int identity not null );

go

create view MetaConversionTablesCatalog (ID, AssociationKey, AssociationName, RuleName) as
select CT.ID, L.ObjectKey, 'a.' + L.Semantic + '.' + L.Name, CT.AssociateRule from MetaConversionTable CT join MetaLinks L on (CT.RefAssociation = L.ID);

go

/* Таблица перекодировок */
create table ConversionTable
(
	TypeID				int not null,
	ID					int not null,
	constraint PKConversionTable primary key ( ID ),
	constraint FKConversionTableTypeID foreign key ( TypeID )
		references MetaConversionTable ( ID ) on delete cascade
);

create table g.ConversionTable ( ID int identity not null );

/* Входные параметры репекодировки */
create table ConversionInputAttributes
(
	TypeID				int not null,
	ID					int not null,
	Name				varchar (26) not null,
	ValueNum			numeric (20),
	ValueStr			varchar (4000),
	constraint PKConversionInputAttributes primary key ( TypeID, ID, Name ),
	constraint FKConversionInputAttributesID foreign key ( ID )
		references ConversionTable ( ID ) on delete cascade
);

/* Выходные параметры репекодировки */
create table ConversionOutAttributes
(
	TypeID				int not null,
	ID					int not null,
	Name				varchar (26) not null,
	ValueNum			numeric (20),
	ValueStr			varchar (4000),
	constraint PKConversionOutAttributes primary key ( TypeID, ID, Name ),
	constraint FKConversionOutAttributesID foreign key ( ID )
		references ConversionTable ( ID ) on delete cascade
);

go

/* Представление для поиска объектов в базе данных. */
create view DVDB_Objects (Name, Type, Schema_id) as
	select name, type, schema_id from sys.objects
	union
	select I.name, 'INDEX', O.schema_id from sys.indexes I join sys.objects O on (I.object_id = O.object_id and I.type = 2);

go

