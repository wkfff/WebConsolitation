/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		fx_Date.sql - Генерация дат
	СУБД	SQL Server 2005
*/

:!!echo ================================================================================
:!!echo  Классификаторы дат
:!!echo ================================================================================

create table fx_Date_YearDay
(
	ID					int not null,		/* PKID */
	ROWTYPE				int default 0 not null,
	DateYear			numeric (4) not null,
	DateHalfYear		varchar (255) not null,
	DateQuarter			varchar (255) not null,
	DateMonthID			numeric (2) not null,
	DateMonth			varchar (255) not null,
	DateDay				int not null,
	constraint PKDateYearDay primary key ( ID )
);

create table fx_Date_YearMonth
(
	ID					int not null,		/* PKID */
	ROWTYPE				int default 0 not null,
	DateYear			numeric (4) not null,
	DateHalfYear		varchar (255) not null,
	DateQuarter			varchar (255) not null,
	DateMonthID			numeric (2) not null,
	DateMonth			varchar (255) not null,
	constraint PKDateYearMonth primary key ( ID )
);

create table fx_Date_Year
(
	ID					int not null,		/* PKID */
	ROWTYPE				int default 0 not null,
	DateYear			numeric (4) not null,
	constraint PKfx_Date_Year primary key ( ID )
);

create table fx_Date_YearDayUNV
(
	ID					int not null,		/* PKID */
	ROWTYPE				int default 0 not null,
	DateYear			varchar (255) not null,
	DateYearID			int not null,
	DateHalfYear		varchar (255) not null,
	DateHalfYearID		int not null,
	DateQuarter			varchar (255) not null,
	DateQuarterID		int not null,
	DateMonth			varchar (255) not null,
	DateMonthID			int not null,
	DateDay				varchar (255) not null,
	DateDayID			int not null,
	Name				varchar (255) not null,
	ParentID			int,
	OrderByDefault		int,
	constraint PKDateYearDayUNV primary key ( ID ),
	constraint FKDateYearDayUNVParentID foreign key ( ParentID )
		references fx_Date_YearDayUNV ( ID )
);

go
