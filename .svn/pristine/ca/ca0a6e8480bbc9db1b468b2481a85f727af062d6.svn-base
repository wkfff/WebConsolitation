/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		TablesCatalog.sql - скрипт каталога метаданных
	СУБД	Oracle 9.2
*/

pro ================================================================================
pro каталог метаданных
pro ================================================================================

/* Пакеты */
create table MetaPackages
(
	ID					number (10) not null,
	ObjectKey			varchar2 (36) default '00000000-0000-0000-0000-000000000000' not null, /* Уникальный идентификатор объекта */
	Name				varchar2 (100) not null,	/* Наименование пакета */
	BuiltIn				number (1) not null,		/* Встроенный или расположен в отдельном файле */
	RefParent			number (10),				/* Родительский пакет */
	PrivatePath			varchar2 (1000),			/* Относительный путь к файлу пакета */
	OrderBy 			number (10) default 0,		/* Задает последовательность инициализации пакетов */
	Configuration		CLOB,						/* XML настройка элемента, часть XML=UML */
	constraint PKMetaPackages primary key ( ID ),
	constraint FKMetaPackagesRefParent foreign key ( RefParent )
		references MetaPackages ( ID ) on delete cascade
);

create sequence g_MetaPackages start with 2;

insert into MetaPackages (ID, ObjectKey, Name, BuiltIn, RefParent, Configuration) values (1, 'c7a4196e-568e-482f-8219-921423b8a77a', 'Системные объекты', 1, null, null);

/* Объекты */
create table MetaObjects
(
	ID					number (10) not null,
	ObjectKey			varchar2 (36) default '00000000-0000-0000-0000-000000000000' not null, /* Уникальный идентификатор объекта */
	Semantic			varchar2 (26) not null,		/* Английнкое семантическое наименование объекта в БД */
	Name				varchar2 (26) not null,		/* Английнкое наименование объекта в БД */
	Class				number (2) default 0 not null,/* Класс объекта:
														0 - сопоставимый классификатор */
	SubClass			number (2) default 0 not null,/* Подкласс объекта:
														0 - */
	Configuration		CLOB,						/* XML настройка элемента, часть XML=UML */
	RefPackages			number (10) not null,		/* Пакет, в котором определен объект */
	constraint PKMetaObjects primary key ( ID ),
	constraint FKMetaObjectsRefPackages foreign key ( RefPackages )
		references MetaPackages ( ID ) on delete cascade
);

create sequence g_MetaObjects start with 4;

/* Описание связеи сопоставления сопоставимых классификаторов данных с сопоставимыми классификаторами данных */
create table MetaLinks
(
	ID					number (10) not null,
	ObjectKey			varchar2 (36) default '00000000-0000-0000-0000-000000000000' not null, /* Уникальный идентификатор объекта */
	Semantic			varchar2 (26) not null,		/* Английнкое семантическое наименование объекта в БД */
	Name				varchar2 (60) not null,		/* Наименование ассоциации */
	Class				number (2) default 0 not null,/* Класс объекта */
	RefParent			number (10) not null,		/* Роль A. Ссылка на идентификатор сопоставимого классификатора (сопоставимый) */
	RefChild			number (10) not null,		/* Роль B. Ссылка на идентификатор сопоставимого классификатора (сопоставляемый) */
	Configuration		CLOB,						/* XML настройка элемента, часть XML=UML */
	RefPackages			number (10) not null,		/* Пакет, в котором определен объект */
	DefaultAssociateRule varchar2 (255),    /* Уникальный идентификатор правила сопоставления по умолчанию */
	constraint PKMetaLinks primary key ( ID ),
	constraint FKMetaLinksRefParent foreign key ( RefParent )
		references MetaObjects ( ID ) on delete cascade,
	constraint FKMetaLinksRefChild foreign key ( RefChild )
		references MetaObjects ( ID ) on delete cascade,
	constraint FKMetaLinksRefPackages foreign key ( RefPackages )
		references MetaPackages ( ID ) on delete cascade
);

create sequence g_MetaLinks;

insert into MetaObjects (ID, ObjectKey, Semantic, Name, Class, SubClass, RefPackages) values (0, 'b4612528-0e51-4e6b-8891-64c22611816b', 'Date', 'YearDayUNV', 2, 4, 1);
insert into MetaObjects (ID, ObjectKey, Semantic, Name, Class, SubClass, RefPackages) values (1, '675ede52-a0b4-423a-b0f6-365ad02d0f6f', 'Date', 'YearDay', 2, 4, 1);
insert into MetaObjects (ID, ObjectKey, Semantic, Name, Class, SubClass, RefPackages) values (2, '30fd54c6-de78-4664-afb2-d5fcadc10e9c', 'Date', 'YearMonth', 2, 4, 1);
insert into MetaObjects (ID, ObjectKey, Semantic, Name, Class, SubClass, RefPackages) values (3, 'c66d6056-7282-4ab0-ab0b-ea43ad68cb4c', 'Date', 'Year', 2, 4, 1);


/* Документы */
create table MetaDocuments
(
	ID					number (10) not null,
	ObjectKey			varchar2 (36) default '00000000-0000-0000-0000-000000000000' not null, /* Уникальный идентификатор объекта */
	Name				varchar2 (100) not null,	/* Наименование документа */
	DocType				number (10) not null,		/* Тип документа */
	RefPackages			number (10) not null,		/* родительский пакет */
	Configuration		CLOB,						/* XML настройка элемента, часть XML=UML */
	constraint PKMetaDocuments primary key ( ID ),
	constraint FKMetaDocumentsRefPackages foreign key ( RefPackages )
		references MetaPackages ( ID ) on delete cascade
);

create sequence g_MetaDocuments;

/* Представление для получения имен ролей */
create or replace view MetaLinksWithRolesNames (ID, Semantic, Name, Class, ObjectKey, RoleAName, RoleBName, RoleAObjectKey, RoleBObjectKey, RefPackages, Configuration) as
select
	L.ID, L.Semantic, L.Name, L.Class, L.ObjectKey,
	case OP.Class when 0 then 'b' when 1 then 'd' when 2 then 'fx' when 3 then 'f' when 10 then 't' end || '.' || OP.Semantic || '.' || OP.Name,
	case OC.Class when 0 then 'b' when 1 then 'd' when 2 then 'fx' when 3 then 'f' when 10 then 't' end || '.' || OC.Semantic || '.' || OC.Name,
	OP.ObjectKey, OC.ObjectKey,
	L.RefPackages, L.Configuration
from MetaLinks L join MetaObjects OP on (OP.ID = L.RefParent) join MetaObjects OC on (OC.ID = L.RefChild);


/* ================================================================================
	Таблицы перекодировок
   ================================================================================ */

/* Каталог таблиц перекодировок */
create table MetaConversionTable
(
	ID					number (10) not null,		/* PKID */
	RefAssociation		number (10) not null,		/* Ассоциация для которой применяется правило */
	AssociateRule		varchar2 (50) not null,		/* Наименование правила сопоставления */
	constraint PKMetaConversionTable primary key ( ID ),
	constraint FKMetaCTRefAssociation foreign key ( RefAssociation )
		references MetaLinks ( ID ) on delete cascade
);

create sequence g_MetaConversionTable;

/* Представление для получения списка таблиц перекодировок */
create or replace view MetaConversionTablesCatalog (ID, AssociationKey, AssociationName, RuleName) as
select CT.ID, L.ObjectKey, 'a.' || L.Semantic || '.' || L.Name, CT.AssociateRule from MetaConversionTable CT join MetaLinks L on (CT.RefAssociation = L.ID);

/* Таблица перекодировок */
create table ConversionTable
(
	TypeID				number (10) not null,
	ID					number (10) not null,
	constraint PKConversionTable primary key ( ID ),
	constraint FKConversionTableTypeID foreign key ( TypeID )
		references MetaConversionTable ( ID ) on delete cascade
);

create sequence g_ConversionTable;

/* Входные параметры репекодировки */
create table ConversionInputAttributes
(
	TypeID				number (10) not null,
	ID					number (10) not null,
	Name				varchar2 (26) not null,
	ValueNum			number (20),
	ValueStr			varchar2 (4000),
	constraint PKConversionInputAttributes primary key ( TypeID, ID, Name ),
	constraint FKConversionInputAttributesID foreign key ( ID )
		references ConversionTable ( ID ) on delete cascade
);

/* Выходные параметры репекодировки */
create table ConversionOutAttributes
(
	TypeID				number (10) not null,
	ID					number (10) not null,
	Name				varchar2 (26) not null,
	ValueNum			number (20),
	ValueStr			varchar2 (4000),
	constraint PKConversionOutAttributes primary key ( TypeID, ID, Name ),
	constraint FKConversionOutAttributesID foreign key ( ID )
		references ConversionTable ( ID ) on delete cascade
);

/* Представление для поиска объектов в базе данных. */
create or replace view DVDB_Objects (Name, Type) as
	select object_name, object_type from user_objects
	union
	select constraint_name, constraint_type from user_constraints;

commit work;
