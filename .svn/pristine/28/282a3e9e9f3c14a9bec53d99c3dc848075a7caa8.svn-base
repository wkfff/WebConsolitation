/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		DataSources.sql - Подсистема учета источников данных.
	СУБД	SQL Server 2005
*/

:!!echo ================================================================================
:!!echo  Подсистема учета источников данных
:!!echo ================================================================================

/* Реестр закачек данных */
create table PumpRegistry
(
	ID					int not null,			/* PK */
	SupplierCode		varchar (11) not null,	/* Код поставщика. Текстовый идентификатор */
	DataCode			smallint not null,		/* Порядковый номер поступающей информации */
	ProgramIdentifier	varchar (255) not null,	/* Программа закачки */
	Name				varchar (2000),			/* Наименование */
	ProgramConfig		varchar (max),			/* Конфигурационные параметры для закачки (в формате XML) */
	StagesParams		varchar (max),			/* Состояния этапов закачек */
	Schedule			varchar (max),			/* Настройки закачки для запуска по расписанию */
	PumpProgram			varchar (255),			/* Название файла настроек */
	Comments			varchar (2048),			/* Комментарий */
	constraint PKPumpRegistry primary key ( ID ),
	constraint UKPumpRegistry unique (ProgramIdentifier)
);

create table g.PumpRegistry ( ID int identity not null );

go

create trigger t_PumpRegistry_bi on PumpRegistry instead of insert as
begin
-- begin автоматическая генерация ID
	declare @nullsCount int;
	set nocount on;
	select @nullsCount = count(*) from inserted where ID is null;
	if @nullsCount = 0 insert into PumpRegistry (ID, SupplierCode, DataCode, ProgramIdentifier, Name, ProgramConfig, StagesParams, Schedule, Comments) select ID, SupplierCode, DataCode, ProgramIdentifier, Name, ProgramConfig, StagesParams, Schedule, Comments from inserted;
	else if @nullsCount = 1
	begin
		insert into g.PumpRegistry default values;
		delete from g.PumpRegistry where ID = @@IDENTITY;
		insert into PumpRegistry (ID, SupplierCode, DataCode, ProgramIdentifier, Name, ProgramConfig, StagesParams, Schedule, Comments) select @@IDENTITY, SupplierCode, DataCode, ProgramIdentifier, Name, ProgramConfig, StagesParams, Schedule, Comments from inserted;
	end else
		RAISERROR (500001, 1, 1);
-- end автоматическая генерация ID
end;

go

/* Источник данных */
create table HUB_DataSources
(
	ID					int not null,		/* PK */
	SupplierCode		varchar (11) not null,	/* Код поставщика. Текстовый идентификатор */
	DataCode			smallint not null,		/* Порядковый номер поступающей информации */
	DataName			varchar (255) not null,	/* Наименование поступающей информации */
	KindsOfParams		smallint not null,		/* Вид параметров:
														0 - Бюджет, год,
														1 - Год,
														2 - Год, месяц,
														3 - Год, месяц, вариант,
														4 - Год, вариант,
														5 - Год, квартал
														6 - Год, Территория
													*/
	Year				smallint,					/* Год */
	Name				varchar (255),				/* Финорган */
	Month				smallint,					/* Месяц */
	Variant				varchar (255),				/* Вариант */
	Quarter				tinyint,					/* Квартал */
	Territory			varchar (255),				/* Территория */
	Locked				numeric (1) default 0 not null,	/* Признак блокировки */
	Deleted				numeric (1) default 0 not null, /* Признак удаления */

	constraint PKHUB_DataSources primary key ( ID )
);

create table g.DataSources ( ID int identity not null );

/* Выполненные операции закачки */
create table PumpHistory
(
	ID					int not null,			/* PK */
	ProgramIdentifier	varchar (38) not null,	/* Программа закачки */
	ProgramConfig		varchar (max),			/* Конфигурационные параметры выполненной закачки (в формате XML) */
	SystemVersion		varchar (12) not null,	/* Версия системы в формате XXX.XXX.XXX.XXX */
	ProgramVersion		varchar (20) not null,	/* Версия программы(модуля) закачки в формате XXX.XXX.XXX.XXX */
	PumpDate			datetime not null,		/* Дата и время когда была запущена закачка */
	StartedBy			tinyint not null,		/* Запущена пользователем или планировщиком по расписанию
														0 - планировщиком по расписанию
														1 - пользователем */
	BatchId				varchar (132),			/* Идентификатора пакета диспетчера расчета многомерных объектов */
	UserName			varchar (255),			/* Имя пользователя запустившего закачку */
	UserHost			varchar (255),			/* Имя машины с которой пользователь запустил закачку */
	SessionID			varchar (24),			/* Сессия пользователя запустившего закачку */
	RefPumpRegistry		int not null,			/* FK - Реестр закачек */
	Comments			varchar (2048),			/* Комментарий */

	constraint PKPumpHistory primary key ( ID ),
	constraint FKPumpHistoryRefPumpRegistry foreign key ( RefPumpRegistry )
		references PumpRegistry ( ID ) on delete cascade
);

create index idx_PumpHistoryRefPumpRegistry on PumpHistory (RefPumpRegistry);

create table g.PumpHistory ( ID int identity not null );

/* Связь источников данных с выполненными операциями закачки */
create table DataSources2PumpHistory
(
	RefDataSources		int not null,			/* FK - Источник данных */
	RefPumpHistory		int not null,			/* FK - Операция закачки */
	constraint PKDataSources2PumpHistory primary key ( RefDataSources, RefPumpHistory ),
	constraint FKDS2PHRefDataSources foreign key ( RefDataSources )
		references HUB_DataSources ( ID ) on delete cascade,
	constraint FKDS2PHRefPumpHistory foreign key ( RefPumpHistory )
		references PumpHistory ( ID ) on delete cascade
);

go

create view DataSources (
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory, Locked, Deleted, DataSourceName
) as
select
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory, Locked, Deleted,
	SupplierCode + ' ' + DataName + cast((
		CASE KindsOfParams
			WHEN 8 THEN ''
			ELSE ' - ' +
			CASE KindsOfParams
				WHEN 0 THEN Name + ' ' + cast(Year as varchar(4))
				WHEN 1 THEN cast(Year as varchar(4))
				WHEN 2 THEN cast(Year as varchar(4)) + ' ' + cast(Month as varchar(2))
				WHEN 3 THEN cast(Year as varchar(4)) + ' ' + cast(Month as varchar(2)) + ' ' + Variant
				WHEN 4 THEN cast(Year as varchar(4)) + ' ' + Variant
				WHEN 5 THEN cast(Year as varchar(4)) + ' ' + cast(Quarter as varchar(1))
				WHEN 6 THEN cast(Year as varchar(4)) + ' ' + Territory
				WHEN 7 THEN cast(Year as varchar(4)) + ' ' + cast(Quarter as varchar(1)) + ' ' + cast(Month as varchar(2))
				WHEN 9 THEN Variant
			END
		END) as varchar(1024))
from HUB_DataSources;


go
