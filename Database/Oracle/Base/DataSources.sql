/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		DataSources.sql - Подсистема учета источников данных.
	СУБД	Oracle 9.2
*/

pro ================================================================================
pro Подсистема учета источников данных.
pro ================================================================================

/* Реестр закачек данных */
create table PumpRegistry
(
	ID					number (10) not null,		/* PK */
	SupplierCode		varchar2 (11) not null,		/* Код поставщика. Текстовый идентификатор */
	DataCode			number (4) not null,		/* Порядковый номер поступающей информации */
	ProgramIdentifier	varchar2 (255) not null,	/* Программа закачки */
	Name				varchar2 (2000),			/* Наименование */
	ProgramConfig		CLOB,						/* Конфигурационные параметры для закачки (в формате XML) */
	StagesParams		CLOB,						/* Состояния этапов закачек */
	Schedule			CLOB,						/* Настройки закачки для запуска по расписанию */
	PumpProgram			varchar2 (255),				/* Название файла настроек */
	Comments			varchar2 (2048),			/* Комментарий */
	constraint PKPumpRegistry primary key ( ID ),
	constraint UKPumpRegistry unique (ProgramIdentifier)
);

create sequence g_PumpRegistry;

create or replace trigger t_PumpRegistry_bi before insert on PumpRegistry for each row
begin
	if :new.ID is null then select g_PumpRegistry.NextVal into :new.ID from Dual; end if;
end t_PumpRegistry_bi;
/

/* Источник данных */
create table HUB_DataSources
(
	ID					number (10) not null,		/* PK */
	SupplierCode		varchar2 (11) not null,		/* Код поставщика. Текстовый идентификатор */
	DataCode			number (4) not null,		/* Порядковый номер поступающей информации */
	DataName			varchar2 (255) not null,	/* Наименование поступающей информации */
	KindsOfParams		number (4) not null,		/* Вид параметров:
														0 - Бюджет, год,
														1 - Год,
														2 - Год, месяц,
														3 - Год, месяц, вариант,
														4 - Год, вариант,
														5 - Год, квартал
														6 - Год, Территория
													*/
	Year				number (4),					/* Год */
	Name				varchar2 (255),				/* Финорган */
	Month				number (2),					/* Месяц */
	Variant				varchar2 (255),				/* Вариант */
	Quarter				number (1),					/* Квартал */
	Territory			varchar2 (255),				/* Территория */
	Locked				number (1) default 0 not null,	/* Признак блокировки */
	Deleted				number (1) default 0 not null,	/* Признак того, что источник удален */
	constraint PKHUB_DataSources primary key ( ID )
);

create sequence g_DataSources;


/* Выполненные операции закачки */
create table PumpHistory
(
	ID					number (10) not null,		/* PK */
	ProgramIdentifier	varchar2 (38) not null,		/* Программа закачки */
	ProgramConfig		CLOB,						/* Конфигурационные параметры выполненной закачки (в формате XML) */
	SystemVersion		varchar2 (12) not null,		/* Версия системы в формате XXX.XXX.XXX.XXX */
	ProgramVersion		varchar2 (20) not null,		/* Версия программы(модуля) закачки в формате XXX.XXX.XXX.XXX */
	PumpDate			DATE not null,				/* Дата и время когда была запущена закачка */
	StartedBy			number (1) not null,		/* Запущена пользователем или планировщиком по расписанию
														0 - планировщиком по расписанию
														1 - пользователем */
	RefPumpRegistry		number (10) not null,		/* FK - Реестр закачек */
	Comments			varchar2 (2048),			/* Комментарий */
	BatchId				varchar2 (132),				/* Идентификатор пакета обработки кубов */
	UserName			varchar2 (255),				/* Логин пользователя запустившего закачку */
	UserHost			varchar2 (255),				/* Имя машины с которой пользователь запустил закачку */
	SessionID			varchar2 (24),				/* Идентификатор сессии пользователя запустившего закачку */
	constraint PKPumpHistory primary key ( ID ),
	constraint FKPumpHistoryRefPumpRegistry foreign key ( RefPumpRegistry )
		references PumpRegistry ( ID ) on delete cascade
);

create index idx_PumpHistoryRefPumpRegistry on PumpHistory (RefPumpRegistry) tablespace DVINDX compute statistics;

create sequence g_PumpHistory;


/* Связь источников данных с выполненными операциями закачки */
create table DataSources2PumpHistory
(
	RefDataSources		number (10) not null,		/* FK - Источник данных */
	RefPumpHistory		number (10) not null,		/* FK - Операция закачки */
	constraint PKDataSources2PumpHistory primary key ( RefDataSources, RefPumpHistory ),
	constraint FKDS2PHRefDataSources foreign key ( RefDataSources )
		references HUB_DataSources ( ID ) on delete cascade,
	constraint FKDS2PHRefPumpHistory foreign key ( RefPumpHistory )
		references PumpHistory ( ID ) on delete cascade
);


/* Вид параметров:
														0 - Бюджет, год,
														1 - Год,
														2 - Год, месяц,
														3 - Год, месяц, вариант,
														4 - Год, вариант,
														5 - Год, квартал
														6 - Год, Территория
														7 - Год, квартал, месяц
														8 - Без параметров
														9 - Вариант
*/

create or replace view DataSources (
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory, Locked, Deleted, DataSourceName
) as
select
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory, Locked, Deleted,
	SupplierCode ||' '|| DataName ||
		CASE KindsOfParams
			WHEN 8 THEN ''
			ELSE ' - ' ||
				CASE KindsOfParams
					WHEN 0 THEN Name || ' ' || Year
					WHEN 1 THEN cast(Year as varchar(4))
					WHEN 2 THEN Year || ' ' || Month
					WHEN 3 THEN Year || ' ' || Month || ' ' || Variant
					WHEN 4 THEN Year || ' ' || Variant
					WHEN 5 THEN Year || ' ' || Quarter
					WHEN 6 THEN Year || ' ' || Territory
					WHEN 7 THEN Year || ' ' || Quarter || ' ' || Month
					WHEN 9 THEN Variant
				END
		END
from HUB_DataSources;

commit work;
