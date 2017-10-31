/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		DataPump.sql - Таблицы для закачки данных.
	СУБД	Oracle 9.2
*/

pro ================================================================================
pro Правила расщепрения данных УМНС 28н
pro ================================================================================

/* =================================================================================
	Правила расщепрения данных УМНС 28н
   ================================================================================= */

/* правила расщепления */
create table DisintRules_KD
(
	ID					number (10) not null,
	KD					varchar2 (20) not null,		/* КД */
	Name				varchar2 (4000) not null,	/* Наименование */
	Year				number (4) not null,		/* Год */
	ByBudget			number (1) default 1 not null,	/* признак – по закону о Бюджете или дополнительное технологическое правило */
	Fed_Percent			number (5,2) not null,		/* % федеральный бюджет */
	Cons_Percent		number (5,2) not null,		/* % консолидированный бюджет субъекта РФ/города федерального значения */
	Subj_Percent		number (5,2) not null,		/* % бюджет субъекта РФ/города федерального */
	ConsMR_Percent		number (5,2) not null,		/* % консолидированный бюджет муниципального района */
	MR_Percent			number (5,2) not null,		/* % бюджет муниципального района */
	Stad_Percent		number (5,2) not null,		/* % бюджет поселения */
	OutOfBudgetFond_Percent number (5,2) default 0 not null, /* % внебюдж. фонды */
	SmolenskAccount_Percent number (5,2) default 0 not null, /* % на счет УФК Смоленск */
	TumenAccount_Percent number (5,2) default 0 not null,	/* % отчисления в областной бюджет Тюменской обл */
	CONSMO_Percent		number (5,2) default 0 not null,	/* % консолидированный бюджет муниципальных образований (Конс.бюджет МО) */
	GO_Percent			number (5,2) default 0 not null,	/* % бюджет городского округа (Бюджет ГО) */
	Comments			varchar2 (255),				/* Комментарий */
	constraint PKDisintRules_KD primary key ( ID ),
	constraint UKDisintRules_KD unique ( Year, KD )
);

create sequence g_DisintRules_KD;


/* Альтернативные коды для правил расщепления */
create table DisintRules_AltKD
(
	ID					number (10) not null,
	KD					varchar2 (20) not null,		/* КД */
	Name				varchar2 (4000) not null,	/* Наименование */
	Comments			varchar2 (255),				/* Комментарий */
	RefDisintRules_KD	number (10) not null,		/* ссылка на правило */
	constraint PKDisintRules_AltKD primary key ( ID ),
	constraint FKDisintRules_AltKDRefDistKD foreign key ( RefDisintRules_KD )
		references DisintRules_KD ( ID ) on delete cascade
);

create sequence g_DisintRules_AltKD;


/* Исключения */
create table DisintRules_Ex
(
	ID					number (10) not null,
	Basic				number (1) default 1 not null,	/* Базовая запись/уточнение (корректировка) */
	Init_Date			number (8) default 0 not null,	/* Дата, с которой действует корректировка */
	Region				varchar2 (20) default 0 not null,/* Район */
	Fed_Percent			number (5,2) not null,		/* % федеральный бюджет */
	Cons_Percent		number (5,2) not null,		/* % консолидированный бюджет субъекта РФ/города федерального значения */
	Subj_Percent		number (5,2) not null,		/* % бюджет субъекта РФ/города федерального */
	ConsMR_Percent		number (5,2) not null,		/* % Консолидированный бюджет муниципального района */
	MR_Percent			number (5,2) not null,		/* % бюджет муниципального района */
	Stad_Percent		number (5,2) not null,		/* % бюджет поселения */
	OutOfBudgetFond_Percent number (5,2) default 0 not null, /* % внебюдж. фонды */
	SmolenskAccount_Percent number (5,2) default 0 not null, /* % на счет УФК Смоленск */
	TumenAccount_Percent number (5,2) default 0 not null,	/* % отчисления в областной бюджет Тюменской обл */
	CONSMO_Percent		number (5,2) default 0 not null,	/* % консолидированный бюджет муниципальных образований (Конс.бюджет МО) */
	GO_Percent			number (5,2) default 0 not null,	/* % бюджет городского округа (Бюджет ГО) */
	Comments			varchar2 (255),				/* Комментарий */
	RefDisintRules_KD	number (10) not null,		/* ссылка на правило */
	constraint PKDisintRulesEx primary key ( ID ),
	constraint FKDisintRulesExRefDisintKD foreign key ( RefDisintRules_KD )
		references DisintRules_KD ( ID ) on delete cascade,
	constraint UKDisintRules_Ex unique ( RefDisintRules_KD, Init_Date, Region )
);

create sequence g_DisintRules_Ex;

commit work;
