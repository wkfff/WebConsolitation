/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		DataPump.sql - Таблицы для закачки данных.
	СУБД	SQL Server 2005
*/

:!!echo ================================================================================
:!!echo  Подсистема правил расщепрения данных УМНС 28н
:!!echo ================================================================================

/* правила расщепления */
create table DisintRules_KD
(
	ID					int not null,
	KD					varchar (20) not null,		/* КД */
	Name				varchar (4000) not null,	/* Наименование */
	Year				smallint not null,			/* Год */
	ByBudget			tinyint default 1 not null,	/* признак – по закону о Бюджете или дополнительное технологическое правило */
	Fed_Percent			numeric (5,2) not null,		/* % федеральный бюджет */
	Cons_Percent		numeric (5,2) not null,		/* % консолидированный бюджет субъекта РФ/города федерального значения */
	Subj_Percent		numeric (5,2) not null,		/* % бюджет субъекта РФ/города федерального */
	ConsMR_Percent		numeric (5,2) not null,		/* % консолидированный бюджет муниципального района */
	MR_Percent			numeric (5,2) not null,		/* % бюджет муниципального района */
	Stad_Percent		numeric (5,2) not null,		/* % бюджет поселения */
	OutOfBudgetFond_Percent numeric (5,2) default 0 not null, /* % внебюдж. фонды */
	SmolenskAccount_Percent numeric (5,2) default 0 not null, /* % на счет УФК Смоленск */
	TumenAccount_Percent numeric (5,2) default 0 not null,	/* % отчисления в областной бюджет Тюменской обл */
	CONSMO_Percent		numeric (5,2) default 0 not null,	/* % консолидированный бюджет муниципальных образований (Конс.бюджет МО) */
	GO_Percent			numeric (5,2) default 0 not null,	/* % бюджет городского округа (Бюджет ГО) */
	Comments			varchar (255),				/* Комментарий */
	constraint PKDisintRules_KD primary key ( ID ),
	constraint UKDisintRules_KD unique ( Year, KD )
);

create table g.DisintRules_KD ( ID int identity not null );

/* Альтернативные коды для правил расщепления */
create table DisintRules_AltKD
(
	ID					int not null,
	KD					varchar (20) not null,		/* КД */
	Name				varchar (4000) not null,	/* Наименование */
	Comments			varchar (255),				/* Комментарий */
	RefDisintRules_KD	int not null,				/* ссылка на правило */
	constraint PKDisintRules_AltKD primary key ( ID ),
	constraint FKDisintRules_AltKDRefDistKD foreign key ( RefDisintRules_KD )
		references DisintRules_KD ( ID ) on delete cascade
);

create table g.DisintRules_AltKD ( ID int identity not null );

/* Исключения */
create table DisintRules_Ex
(
	ID					int not null,
	Basic				tinyint default 1 not null,		/* Базовая запись/уточнение (корректировка) */
	Init_Date			numeric (8) default 0 not null,	/* Дата, с которой действует корректировка */
	Region				varchar (20) default 0 not null,/* Район */
	Fed_Percent			numeric (5,2) not null,			/* % федеральный бюджет */
	Cons_Percent		numeric (5,2) not null,			/* % консолидированный бюджет субъекта РФ/города федерального значения */
	Subj_Percent		numeric (5,2) not null,			/* % бюджет субъекта РФ/города федерального */
	ConsMR_Percent		numeric (5,2) not null,			/* % Консолидированный бюджет муниципального района */
	MR_Percent			numeric (5,2) not null,			/* % бюджет муниципального района */
	Stad_Percent		numeric (5,2) not null,			/* % бюджет поселения */
	OutOfBudgetFond_Percent numeric (5,2) default 0 not null, /* % внебюдж. фонды */
	SmolenskAccount_Percent numeric (5,2) default 0 not null, /* % на счет УФК Смоленск */
	TumenAccount_Percent numeric (5,2) default 0 not null,	/* % отчисления в областной бюджет Тюменской обл */
	CONSMO_Percent		numeric (5,2) default 0 not null,	/* % консолидированный бюджет муниципальных образований (Конс.бюджет МО) */
	GO_Percent			numeric (5,2) default 0 not null,	/* % бюджет городского округа (Бюджет ГО) */
	Comments			varchar (255),					/* Комментарий */
	RefDisintRules_KD	int not null,					/* ссылка на правило */
	constraint PKDisintRulesEx primary key ( ID ),
	constraint FKDisintRulesExRefDisintKD foreign key ( RefDisintRules_KD )
		references DisintRules_KD ( ID ) on delete cascade,
	constraint UKDisintRules_Ex unique ( RefDisintRules_KD, Init_Date, Region )
);

create table g.DisintRules_Ex ( ID int identity not null );

go
