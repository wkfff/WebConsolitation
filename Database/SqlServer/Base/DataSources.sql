/*
	��� "������ � ������������"
	������	3.1
	������
		DataSources.sql - ���������� ����� ���������� ������.
	����	SQL Server 2005
*/

:!!echo ================================================================================
:!!echo  ���������� ����� ���������� ������
:!!echo ================================================================================

/* ������ ������� ������ */
create table PumpRegistry
(
	ID					int not null,			/* PK */
	SupplierCode		varchar (11) not null,	/* ��� ����������. ��������� ������������� */
	DataCode			smallint not null,		/* ���������� ����� ����������� ���������� */
	ProgramIdentifier	varchar (255) not null,	/* ��������� ������� */
	Name				varchar (2000),			/* ������������ */
	ProgramConfig		varchar (max),			/* ���������������� ��������� ��� ������� (� ������� XML) */
	StagesParams		varchar (max),			/* ��������� ������ ������� */
	Schedule			varchar (max),			/* ��������� ������� ��� ������� �� ���������� */
	PumpProgram			varchar (255),			/* �������� ����� �������� */
	Comments			varchar (2048),			/* ����������� */
	constraint PKPumpRegistry primary key ( ID ),
	constraint UKPumpRegistry unique (ProgramIdentifier)
);

create table g.PumpRegistry ( ID int identity not null );

go

create trigger t_PumpRegistry_bi on PumpRegistry instead of insert as
begin
-- begin �������������� ��������� ID
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
-- end �������������� ��������� ID
end;

go

/* �������� ������ */
create table HUB_DataSources
(
	ID					int not null,		/* PK */
	SupplierCode		varchar (11) not null,	/* ��� ����������. ��������� ������������� */
	DataCode			smallint not null,		/* ���������� ����� ����������� ���������� */
	DataName			varchar (255) not null,	/* ������������ ����������� ���������� */
	KindsOfParams		smallint not null,		/* ��� ����������:
														0 - ������, ���,
														1 - ���,
														2 - ���, �����,
														3 - ���, �����, �������,
														4 - ���, �������,
														5 - ���, �������
														6 - ���, ����������
													*/
	Year				smallint,					/* ��� */
	Name				varchar (255),				/* �������� */
	Month				smallint,					/* ����� */
	Variant				varchar (255),				/* ������� */
	Quarter				tinyint,					/* ������� */
	Territory			varchar (255),				/* ���������� */
	Locked				numeric (1) default 0 not null,	/* ������� ���������� */
	Deleted				numeric (1) default 0 not null, /* ������� �������� */

	constraint PKHUB_DataSources primary key ( ID )
);

create table g.DataSources ( ID int identity not null );

/* ����������� �������� ������� */
create table PumpHistory
(
	ID					int not null,			/* PK */
	ProgramIdentifier	varchar (38) not null,	/* ��������� ������� */
	ProgramConfig		varchar (max),			/* ���������������� ��������� ����������� ������� (� ������� XML) */
	SystemVersion		varchar (12) not null,	/* ������ ������� � ������� XXX.XXX.XXX.XXX */
	ProgramVersion		varchar (20) not null,	/* ������ ���������(������) ������� � ������� XXX.XXX.XXX.XXX */
	PumpDate			datetime not null,		/* ���� � ����� ����� ���� �������� ������� */
	StartedBy			tinyint not null,		/* �������� ������������� ��� ������������� �� ����������
														0 - ������������� �� ����������
														1 - ������������� */
	BatchId				varchar (132),			/* �������������� ������ ���������� ������� ����������� �������� */
	UserName			varchar (255),			/* ��� ������������ ������������ ������� */
	UserHost			varchar (255),			/* ��� ������ � ������� ������������ �������� ������� */
	SessionID			varchar (24),			/* ������ ������������ ������������ ������� */
	RefPumpRegistry		int not null,			/* FK - ������ ������� */
	Comments			varchar (2048),			/* ����������� */

	constraint PKPumpHistory primary key ( ID ),
	constraint FKPumpHistoryRefPumpRegistry foreign key ( RefPumpRegistry )
		references PumpRegistry ( ID ) on delete cascade
);

create index idx_PumpHistoryRefPumpRegistry on PumpHistory (RefPumpRegistry);

create table g.PumpHistory ( ID int identity not null );

/* ����� ���������� ������ � ������������ ���������� ������� */
create table DataSources2PumpHistory
(
	RefDataSources		int not null,			/* FK - �������� ������ */
	RefPumpHistory		int not null,			/* FK - �������� ������� */
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
