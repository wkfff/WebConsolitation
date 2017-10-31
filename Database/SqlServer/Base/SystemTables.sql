/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		SystemTables.sql - скрипт создания системных таблиц хранилища данных
	СУБД	SQL Server 2005
*/

:!!echo ================================================================================
:!!echo  Системные таблицы
:!!echo ================================================================================
go

create procedure usp_Generator @generator varchar (100) as
begin
	-- set nocount on added to prevent extra result sets from
	-- interfering with select statements.
	set nocount on;

	exec ('insert into g.' + @generator + ' with(tablock) default values;');
	exec ('delete from g.' + @generator + ' where ID = @@IDENTITY;');
	return @@IDENTITY;
end;

go

/* Версии базы данных */
create table DatabaseVersions
(
	ID					int not null,
	Name				varchar (50),
	Released			datetime,
	Updated				datetime,
	[Comments]			varchar (255),
	NeedUpdate			tinyint default 0 not null,
	constraint PK_DatabaseVersions primary key ( ID )
);

--insert into DatabaseVersions (ID, Name, Released, Updated, [Comments]) values (20, '2.3.0', GETDATE(), GETDATE(), '');
--insert into DatabaseVersions (ID, Name, Released, Updated, [Comments]) values (21, '2.3.1.0', GETDATE(), GETDATE(), '');
--insert into DatabaseVersions (ID, Name, Released, Updated, [Comments], NeedUpdate) values (48, '2.4.1.0', CONVERT(datetime, '2008.05.29', 102), GETDATE(), '', 1);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (50, '2.4.1.2', CONVERT(datetime, '2008.07.02', 102), GETDATE(), 'Изменения в структуре регистрации подключаемых модулей к воркплейсу.', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
--values (52, '2.4.1.4', CONVERT(datetime, '2008.07.23', 102), GETDATE(), 'админ 0003 - на этапе обработки формируется расходный классификатор', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
--values (53, '2.4.1.5', CONVERT(datetime, '2008.07.28', 102), GETDATE(), 'фнс 0004 - закачка ФНС_0004_5 НИО', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (54, '2.4.1.6', CONVERT(datetime, '2008.10.08', 102), GETDATE(), 'Добавлен тип объекта web интерфейс.', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
--values (55, '2.4.1.7', CONVERT(datetime, '2008.10.30', 102), GETDATE(), 'фк 1, фо 2 - добавление этапа проверки данных', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (57, '2.5.0.1', CONVERT(datetime, '2008.12.02', 102), GETDATE(), 'Изменения в структуре хранения отчетов репозитория', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (58, '2.5.0.2', CONVERT(datetime, '2008.12.02', 102), GETDATE(), 'Добавлен модуль для планирования источников финансирования.', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (59, '2.5.0.3', CONVERT(datetime, '2008.12.08', 102), GETDATE(), 'Добавлен модуль "Классификаторы и таблицы" взамен все модулей  работы с классификаторами.', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (60, '2.5.0.4',CONVERT(datetime, '2009.02.25', 102), GETDATE(), 'Добавлена таблица для работы с версиями классификаторов', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (61, '2.5.0.5', CONVERT(datetime, '2008.12.17', 102), GETDATE(), 'Добавлен уникальный идентификатор правила сопоставления по умолчанию.', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (62, '2.5.0.6', CONVERT(datetime, '2008.12.19', 102), GETDATE(), 'Добавлен модуль для прогноза развития региона.', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
--values (63, '2.5.0.7', CONVERT(datetime, '2009.01.26', 102), GETDATE(), 'фнс рф - 4 нм - изменение комментария к этапу обработки', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (64, '2.5.0.8', CONVERT(datetime, '2009.02.25', 102), GETDATE(), 'Исправление ошибки в триггере t_Batch_bi', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (65, '2.5.0.9', CONVERT(datetime, '2009.02.25', 102), GETDATE(), 'Изменения в структуре хранения отчетов репозитория.', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
--values (66, '2.5.0.10', CONVERT(datetime, '2009.05.15', 102), GETDATE(), 'уфк 18 - добавление этапа обработки', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
--values (67, '2.5.0.11', CONVERT(datetime, '2009.05.15', 102), GETDATE(), 'уфк 19 - добавление этапа обработки', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
--values (68, '2.5.0.12', CONVERT(datetime, '2009.05.19', 102), GETDATE(), 'Добавление индексов для поля ParentID.', 1);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
--values (69, '2.5.0.13', CONVERT(datetime, '2009.06.03', 102), GETDATE(), 'фнс1 - добавление этапа проверки данных', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) --
--values (80, '2.7.0.0', CONVERT(datetime, '2010.03.01', 102), GETDATE(), '', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
--values (95, '3.0.0.0', CONVERT(datetime, '2011.04.01', 102), GETDATE(), '', 0);

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (122, '3.1.0.0', CONVERT(datetime, '2012.04.18', 102), GETDATE(), '', 0);

go

/* Хранение списка объектов просмотра в базе */
create table RegisteredUIModules
(
	ID					int not null,				/* PK */
	Name				varchar (255) not null,		/* Имя объекта просмотра */
	Description			varchar (500),				/* Описание объекта просмотра */
	FullName			varchar (255),				/* Полное наименование  класса */
	constraint PKRegisteredUIModules primary key ( ID ),/* ID Должно быть уникальным */
	constraint UKRegisteredUIModules unique ( Name )	/* Имя должно быть уникальным */
);

/*create trigger t_RegisteredUIModules_d on RegisteredUIModules for delete as
begin
	delete from Objects obj where obj.Name = :old.Name;
end;

go*/

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (0,   'AdministrationUI', 'Интерфейс администрирования', 'Krista.FM.Client.ViewObjects.AdministrationUI.AdministrationNavigation, Krista.FM.Client.ViewObjects.AdministrationUI');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (5,   'MDObjectsManagementUI', 'Управление многомерными моделями', 'Krista.FM.Client.ViewObjects.MDObjectsManagementUI.MDObjectsManagementNavigation, Krista.FM.Client.ViewObjects.MDObjectsManagementUI');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (10,  'ProtocolsViewObject', 'Протоколы', 'Krista.FM.Client.ViewObjects.ProtocolsUI.ProtocolsNavigation, Krista.FM.Client.ViewObjects.ProtocolsUI');
--insert into RegisteredUIModules (ID, Name, Description, FullName) 
--values (20,  'FixedClsUI', 'Интерфейс фиксированных классификаторов', 'Krista.FM.Client.ViewObjects.AssociatedCLSUI.FixedClsNavigation, Krista.FM.Client.ViewObjects.AssociatedCLSUI');
--insert into RegisteredUIModules (ID, Name, Description, FullName) 
--values (30,  'DataClsUI', 'Интерфейс классификаторов данных', 'Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataClsNavigation, Krista.FM.Client.ViewObjects.AssociatedCLSUI');
--insert into RegisteredUIModules (ID, Name, Description, FullName) 
--values (40,  'AssociatedClsUI', 'Интерфейс сопоставимых классификаторов', 'Krista.FM.Client.ViewObjects.AssociatedCLSUI.AssociatedClsNavigation, Krista.FM.Client.ViewObjects.AssociatedCLSUI');
--insert into RegisteredUIModules (ID, Name, Description, FullName) 
--values (50,  'AssociationUI', 'Интерфейс сопоставления классификаторов', 'Krista.FM.Client.ViewObjects.AssociatedCLSUI.Association.AssociationNavigation, Krista.FM.Client.ViewObjects.AssociatedCLSUI');
--insert into RegisteredUIModules (ID, Name, Description, FullName) 
--values (60,  'TranslationsTablesUI', 'Интерфейс таблиц перекодировок', 'Krista.FM.Client.ViewObjects.AssociatedCLSUI.TranslationsTables.TranslationTablesNavigation, Krista.FM.Client.ViewObjects.AssociatedCLSUI');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (70,  'DataPumpUI', 'Интерфейс закачки данных', 'Krista.FM.Client.ViewObjects.DataPumpUI.DataPumpNavigation, Krista.FM.Client.ViewObjects.DataPumpUI');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (80,  'DataSourcesUI', 'Интерфейс источников данных', 'Krista.FM.Client.ViewObjects.DataSourcesUI.DataSourcesNavigation, Krista.FM.Client.ViewObjects.DataSourcesUI');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (90,  'DisintRulesUI', 'Интерфейс нормативов отчисления доходов', 'Krista.FM.Client.ViewObjects.DisintRulesUI.DisintRulesNavigation, Krista.FM.Client.ViewObjects.DisintRulesUI');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (100, 'TasksViewObj', 'Интерфейс задач', 'Krista.FM.Client.ViewObjects.TasksUI.TasksNavigation, Krista.FM.Client.ViewObjects.TasksUI');
--insert into RegisteredUIModules (ID, Name, Description, FullName) 
--values (110, 'FactTablesUI', 'Интерфейс таблиц фактов', 'Krista.FM.Client.ViewObjects.AssociatedCLSUI.FactTablesNavigation, Krista.FM.Client.ViewObjects.AssociatedCLSUI');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (120, 'GlobalConstsViewObj', 'Константы', 'Krista.FM.Client.ViewObjects.GlobalConstsUI.GlobalConstsNavigation, Krista.FM.Client.ViewObjects.GlobalConstsUI');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (130, 'TemplatesViewObj', 'Репозиторий эксперта', 'Krista.FM.Client.ViewObjects.TemplatesUI.TemplatesNavigation, Krista.FM.Client.ViewObjects.TemplatesUI');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (140, 'FinSourcePlanningUI', 'Источники финансирования', 'Krista.FM.Client.ViewObjects.FinSourcePlanningUI.FinSourcePlanningNavigation, Krista.FM.Client.ViewObjects.FinSourcePlanningUI');
insert into RegisteredUIModules (ID, Name, FullName, Description)
values (150, 'EntityNavigationListUI', 'Krista.FM.Client.ViewObjects.AssociatedCLSUI.Navigation.EntityNavigationControl, Krista.FM.Client.ViewObjects.AssociatedCLSUI', 'Классификаторы и таблицы');
insert into RegisteredUIModules (ID, Name, FullName, Description)
values (160, 'ForecastUI', 'Krista.FM.Client.ViewObjects.ForecastUI.ForecastNavigation, Krista.FM.Client.ViewObjects.ForecastUI', 'Прогноз развития региона');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (170, 'FinSourceDebtorBookUI', 'Долговая книга', 'Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.DebtBookNavigation, Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (180, 'ConsBudgetForecastUI', 'Планирование доходов', 'Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.ConsBudgetForecastNavigation, Krista.FM.Client.ViewObjects.ConsBudgetForecastUI');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (190, 'RIA.MarksOMSU', 'Оценка ОМСУ', 'Krista.FM.RIA.Extensions.MarksOMSU');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (200, 'RIA.MOFOWebForms', 'Сбор данных с МО', 'Krista.FM.RIA.Extensions.MOFOWebForms');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (210, 'ReportsUI', 'Отчеты системы', 'Krista.FM.Client.ViewObjects.ReportsUI.Gui.ReportsNavigation, Krista.FM.Client.ViewObjects.ReportsUI');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (220, 'RIA.Consolidation', 'Сбор данных', 'Krista.FM.RIA.Extensions.Consolidation.ConsolidationExtensionInstaller, Krista.FM.RIA.Extensions.Consolidation');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (221, 'RIA.Consolidation.Admin', 'Администратор сбора данных', 'Krista.FM.RIA.Extensions.Consolidation.ConsolidationAdminExtensionInstaller, Krista.FM.RIA.Extensions.Consolidation');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (240, 'RIA.EO12InvestProjects', 'Инвестиционные проекты', 'Krista.FM.RIA.Extensions.EO12InvestProjects.InvestProjectsExtensionInstaller, Krista.FM.RIA.Extensions.EO12InvestProjects');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (241, 'RIA.OrgGKH', 'Сбор по ЖКХ', 'Krista.FM.RIA.Extensions.OrgGKH.OrgGKHExtensionInstaller, Krista.FM.RIA.Extensions.OrgGKH');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (250, 'RIA.EO14InvestAreas', 'Инвестиционные площадки', 'Krista.FM.RIA.Extensions.EO14InvestAreas.InvestAreasExtensionInstaller, Krista.FM.RIA.Extensions.EO14InvestAreas');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (251, 'RIA.EO10MessagePRF', 'Послание ПРФ', 'Krista.FM.RIA.Extensions.EO10MissivePRF.EO10ExtensionInstaller, Krista.FM.RIA.Extensions.EO10MissivePRF');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (252, 'RIA.FO51PassportMO', 'ФО_0051_Паспорт МО','Krista.FM.RIA.Extensions.FO51PassportMO.FO51ExtensionInstaller, Krista.FM.RIA.Extensions.FO51PassportMO');
insert into DV.RegisteredUIModules (ID, Name, Description, FullName) 
values (253, 'RIA.EO15TargetPrograms', 'Целевые программы', 'Krista.FM.RIA.Extensions.EO15TargetPrograms.TargetProgramsExtensionInstaller, Krista.FM.RIA.Extensions.EO15TargetPrograms');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (260, 'RIA.MinSport', 'Спортивная карта России', 'Krista.FM.RIA.Extensions.MinSport.ExtensionInstaller, Krista.FM.RIA.Extensions.MinSport');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (261, 'RIA.FO41HMAO', 'ФО_0041_Оценка эффективности льгот ХМАО','Krista.FM.RIA.Extensions.FO41HMAO.FO41HMAOExtensionInstaller, Krista.FM.RIA.Extensions.FO41HMAO');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (262, 'RIA.FO41', 'Оценка эффективности льгот', 'Krista.FM.RIA.Extensions.FO41.FO41ExtensionInstaller, Krista.FM.RIA.Extensions.FO41');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (263, 'RIA.E86N', 'Сбор информации для официального сайта ГМУ', 'Krista.FM.RIA.Extensions.E86N.E86NExtensionInstaller, Krista.FM.RIA.Extensions.E86N');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (264, 'RIA.EO15ExcCostsAIP', 'ЭО15 Иcполнение расходов АИП', 'Krista.FM.RIA.Extensions.EO15ExcCostsAIP.EO15ExcCostsAIPExtensionInstaller, Krista.FM.RIA.Extensions.EO15ExcCostsAIP');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (270, 'MessagesUI', 'Подсистема сообщений', 'Krista.FM.Client.ViewObjects.MessagesUI.MessageManagementNavigation, Krista.FM.Client.ViewObjects.MessagesUI');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (280, 'RIA.Region10MarksOIV', 'Оценка ОИВ', 'Krista.FM.RIA.Extensions.Region10MarksOIV.Region10MarksOivExtensionInstaller, Krista.FM.RIA.Extensions.Region10MarksOIV');


go

create table GlobalConsts
(
	ID					int not null,				/* PK */
	Name				varchar (255) not null,		/* английское имя константы */
	Caption				varchar (1000) not null,	/* русское наименование */
	Description			varchar (2000) not null,	/* описание константы */
	Value				varchar (4000) not null,	/* значение константы */
	ConstValueType		int not null,				/* тип значения константы */
	ConstCategory		int not null,				/* категория константы */
	ConstType 			int not null,				/* тип константы */
	constraint PKGlobalConsts primary key ( ID ),
	constraint UKGlobalConsts_Name unique ( Name )
);

create table g.GlobalConsts ( ID int identity not null );

go

create trigger t_GlobalConsts_bi on GlobalConsts instead of insert as
begin
-- begin автоматическая генерация ID
	declare @nullsCount int;
	set nocount on;
	select @nullsCount = count(*) from inserted where ID is null;
	if @nullsCount = 0 insert into GlobalConsts (ID, Name, Caption, Description, Value, ConstValueType, ConstCategory, ConstType) select ID, Name, Caption, Description, Value, ConstValueType, ConstCategory, ConstType from inserted;
	else if @nullsCount = 1
	begin
		insert into g.GlobalConsts default values;
		delete from g.GlobalConsts where ID = @@IDENTITY;
		insert into GlobalConsts (ID, Name, Caption, Description, Value, ConstValueType, ConstCategory, ConstType) select @@IDENTITY, Name, Caption, Description, Value, ConstValueType, ConstCategory, ConstType from inserted;
	end else
		RAISERROR (500001, 1, 1);
-- end автоматическая генерация ID
end;

go

-- Таблица для хранения настроек web parts
CREATE TABLE Personalization 
(
	UserName        varchar (255),
	Application     varchar (645),
	pagesettings    varchar (max)
	constraint UK_Pers UNIQUE (UserName, Application)
);

go


/* Start - 12575 - Процедура удаления констрейнов, перед удалением самой таблицы - tsvetkov - 18.01.2010 */

create  procedure utility$removeRelationships
(
 @table_schema  sysname = 'DV',
 @parent_table_name sysname = '%',
 @child_table_name sysname = '%',
      --to another
 @constraint_name sysname = '%'
) as
 begin
 set nocount on
 declare @statements cursor
 set @statements = cursor static for
 select  'alter table ' + quotename(ctu.table_schema) + '.' + quotename(ctu.table_name) +
         ' drop constraint ' + quotename(cc.constraint_name)
 from  information_schema.referential_constraints as cc
          join information_schema.constraint_table_usage as ctu
           on cc.constraint_catalog = ctu.constraint_catalog
              and cc.constraint_schema = ctu.constraint_schema
              and cc.constraint_name = ctu.constraint_name
 where   ctu.table_schema = @table_schema 
   and ctu.table_name like @child_table_name
   and cc.constraint_name like @constraint_name
   and   exists (select *
   from information_schema.constraint_table_usage ctu2
   where cc.unique_constraint_catalog = ctu2.constraint_catalog
      and  cc.unique_constraint_schema = ctu2.constraint_schema
      and  cc.unique_constraint_name = ctu2.constraint_name
      and ctu2.table_schema = @table_schema
      and ctu2.table_name like @parent_table_name)
 open @statements
 declare @statement nvarchar(1000)
 While  (1=1)
  begin
         fetch from @statements into @statement
                if @@fetch_status <> 0
                 break
                exec (@statement)
         end
 end
go

/* End - 12575 - Процедура удаления констрейнов, перед удалением самой таблицы - tsvetkov - 18.01.2010 */

/* Start - - Для SqlServer добавлена таблица DUAL для совместимости с Oracle - gbelov - 21.01.2011 */

create table DUAL (DUMMY varchar(1) not null check (DUMMY='X') primary key);
insert into DUAL (DUMMY) values ('X');

go

/* End - - Для SqlServer добавлена таблица DUAL для совместимости с Oracle - gbelov - 21.01.2011 */


/* Start - - Добавлена таблица для хранения соответствий длинных и сокращенных идентификаторов - gbelov - 22.12.2011 */

create table HashObjectsNames
(
     HashName varchar(30) not null, 	-- Хешированное имя 
     LongName varchar(2048) not null, 	-- Полное имя 
     ObjectType numeric(10) not null, 	-- Тип объекта (перечисление Krista.FM.Server.Scheme.ScriptingEngine.ObjectTypes)
     constraint PKHashObjectsNames primary key (HashName, ObjectType)
)
go

/* End - - Добавлена таблица для хранения соответствий длинных и сокращенных идентификаторов - gbelov - 22.12.2011 */
