/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		SystemTables.sql - скрипт создания системных таблиц хранилища данных
	СУБД	Oracle 9.2
*/

pro ================================================================================
pro Системные таблицы
pro ================================================================================

/* Версии базы данных */
create table DatabaseVersions
(
	ID					number (10) not null,
	Name				varchar2 (50),				/* Текущая версия базы (смотреть надо последнюю запись) */
	Released			Date,						/* Когда была выпущена версия */
	Updated				Date,						/* Когда было произведено фактическое обновление базы */
	Comments			varchar2 (255),				/* Коментарий */
	NeedUpdate			number (1) default 0 not null,	/* Признак указывающий серверу на необходимость произвести обновление схемы
															0 - версия обновлена,
															1 - необходимо обновление
														   после обновления сервер должен сбросить признак. */
	constraint PKDatabaseVersions primary key ( ID )
);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (1, '2.0.0', SYSDATE, SYSDATE, '');
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (2, '2.0.1', SYSDATE, SYSDATE, '');
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (3, '2.0.2', SYSDATE, SYSDATE, '');
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (4, '2.0.3', SYSDATE, SYSDATE, '');
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (5, '2.1.0', SYSDATE, SYSDATE, '');
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (6, '2.1.1', SYSDATE, SYSDATE, '');
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (7, '2.1.2', SYSDATE, SYSDATE, '');
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (8, '2.1.3', SYSDATE, SYSDATE, '');
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (9, '2.1.4', SYSDATE, SYSDATE, '');
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (10, '2.1.5', SYSDATE, SYSDATE, '');
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (11, '2.1.6', SYSDATE, SYSDATE, '');
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (12, '2.1.7', SYSDATE, SYSDATE, '');
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (13, '2.1.8', SYSDATE, SYSDATE, '');
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (14, '2.1.9', SYSDATE, SYSDATE, '');
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (15, '2.1.10', SYSDATE, SYSDATE, '');
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (16, '2.1.11', SYSDATE, SYSDATE, '');
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (17, '2.1.12', SYSDATE, SYSDATE, '');
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (18, '2.1.13', SYSDATE, SYSDATE, '');
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (19, '2.2.0', SYSDATE, SYSDATE, '');
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (20, '2.3.0', SYSDATE, SYSDATE, '');
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (21, '2.3.1.0', SYSDATE, SYSDATE, '');
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (34, '2.4.0.0', To_Date('28.11.2007', 'dd.mm.yyyy'), SYSDATE, '', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (36, '2.4.0.2', To_Date('01.02.2008', 'dd.mm.yyyy'), SYSDATE, 'УФК 11 - отказ от обработки дня ФО, изменение комментария к этапу обработки', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (37, '2.4.0.3', To_Date('06.02.2008', 'dd.mm.yyyy'), SYSDATE, 'Добавление поля - признака блокировки источника в таблицу HUB_DataSources и представление DataSources; Добавление события блокировки источника.', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (39, '2.4.0.5', To_Date('20.02.2008', 'dd.mm.yyyy'), SYSDATE, 'Расширение поля DV.OlapObjects.ProcessResult до 2000 символов', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (40, '2.4.0.6', To_Date('27.02.2008', 'dd.mm.yyyy'), SYSDATE, 'Архивирование протоколов', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (42, '2.4.0.8', To_Date('14.03.2008', 'dd.mm.yyyy'), SYSDATE, 'GUID для объектов.', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (44, '2.4.0.10', To_Date('04.04.2008', 'dd.mm.yyyy'), SYSDATE, 'Инфа о пользователе в PumpHistory', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (46, '2.4.0.12', To_Date('19.05.2008', 'dd.mm.yyyy'), SYSDATE, 'Добавление поля - признака удаления источника в таблицу HUB_DataSources и представление DataSources; добавление события удаления источника.', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
--values (47, '2.4.0.13', To_Date('29.05.2008', 'dd.mm.yyyy'), SYSDATE, 'Добавлена фиксированная запись в системный классификатор fx_Date_YearDayUNV.', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (48, '2.4.1.0', To_Date('29.05.2008', 'dd.mm.yyyy'), SYSDATE, '', 1);

-- в версию не вошло!!!
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (49, '2.4.1.1', To_Date('06.05.2008', 'dd.mm.yyyy'), SYSDATE, 'Изменения в структуре хранения отчетов репозитория', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (50, '2.4.1.2', To_Date('02.07.2008', 'dd.mm.yyyy'), SYSDATE, 'Изменения в структуре регистрации подключаемых модулей к воркплейсу.', 0);

-- в версию не вошло!!!
--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (51, '2.4.1.3', To_Date('16.07.2008', 'dd.mm.yyyy'), SYSDATE, 'Добавлен модуль для планирования источников финансирования.', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
--values (52, '2.4.1.4', To_Date('23.07.2008', 'dd.mm.yyyy'), SYSDATE, 'админ 0003 - на этапе обработки формируется расходный классификатор', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
--values (53, '2.4.1.5', To_Date('28.07.2008', 'dd.mm.yyyy'), SYSDATE, 'фнс 0004 - закачка ФНС_0004_5 НИО', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (54, '2.4.1.6', To_Date('08.10.2008', 'dd.mm.yyyy'), SYSDATE, 'Добавлен тип объекта web интерфейс.', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
--values (55, '2.4.1.7', To_Date('30.10.2008', 'dd.mm.yyyy'), SYSDATE, 'фк 1, фо 2 - добавление этапа проверки данных', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
--values (56, '2.4.1.8', To_Date('21.11.2008', 'dd.mm.yyyy'), SYSDATE, 'Добавление каталога шаблонов отчетов', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (57, '2.5.0.1', To_Date('02.12.2008', 'dd.mm.yyyy'), SYSDATE, 'Изменения в структуре хранения отчетов репозитория', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (58, '2.5.0.2', To_Date('12.08.2008', 'dd.mm.yyyy'), SYSDATE, 'Добавлен модуль для планирования источников финансирования.', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (59, '2.5.0.3', To_Date('08.12.2008', 'dd.mm.yyyy'), SYSDATE, 'Добавлен модуль "Классификаторы и таблицы" взамен все модулей  работы с классификаторами.', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (60, '2.5.0.4', To_Date('10.12.2008', 'dd.mm.yyyy'), SYSDATE, 'Добавлена таблица для работы с версиями классификаторов', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (62, '2.5.0.6', To_Date('03.02.2009', 'dd.mm.yyyy'), SYSDATE, 'Добавлен модуль для прогноза развития региона.', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
--values (63, '2.5.0.7', To_Date('26.01.2009', 'dd.mm.yyyy'), SYSDATE, 'фнс рф - 4 нм - изменение комментария к этапу обработки', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
--values (65, '2.5.0.9', To_Date('25.02.2009', 'dd.mm.yyyy'), SYSDATE, 'Изменения в структуре хранения отчетов репозитория.', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
--values (66, '2.5.0.10', To_Date('15.05.2009', 'dd.mm.yyyy'), SYSDATE, 'уфк 18 - добавление этапа обработки', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
--values (67, '2.5.0.11', To_Date('15.05.2009', 'dd.mm.yyyy'), SYSDATE, 'уфк 19 - добавление этапа обработки', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
--values (68, '2.5.0.12', To_Date('19.05.2009', 'dd.mm.yyyy'), SYSDATE, 'Добавление индексов для поля ParentID.', 1);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
--values (69, '2.5.0.13', To_Date('03.06.2009', 'dd.mm.yyyy'), SYSDATE, 'фнс1 - добавление этапа проверки данных', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
--values (70, '2.5.0.14', To_Date('22.07.2009', 'dd.mm.yyyy'), SYSDATE, 'fx.Date.YearDayUNV - закл. обороты за каждый месяц', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (80, '2.7.0.0', To_Date('01.03.2010', 'dd.mm.yyyy'), SYSDATE, '', 0);

--insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
--values (95, '3.0.0.0', To_Date('01.04.2011', 'dd.mm.yyyy'), SYSDATE, 'Базовая версия 3.0', 0);

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (122, '3.1.0.0', To_Date('18.04.2012', 'dd.mm.yyyy'), SYSDATE, 'Базовая версия 3.1', 0);

commit;


/* Хранение списка объектов просмотра в базе */
create table RegisteredUIModules
(
	ID					number (10) not null,		/* PK */
	Name				varchar2 (255) not null,	/* Имя объекта просмотра */
	Description			varchar2 (500),				/* Описание объекта просмотра */
	FullName			varchar2 (255),				/*  Полное наименование типа объекта */
	constraint PKRegisteredUIModules primary key ( ID ),/* ID Должно быть уникальным */
	constraint UKRegisteredUIModules unique ( Name )	/* Имя должно быть уникальным */
);

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
insert into RegisteredUIModules (ID, Name, FullName, Description) values (150, 'EntityNavigationListUI', 'Krista.FM.Client.ViewObjects.AssociatedCLSUI.Navigation.EntityNavigationControl, Krista.FM.Client.ViewObjects.AssociatedCLSUI', 'Классификаторы и таблицы');
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
values (230, 'RIA.FO41', 'Оценка эффективности льгот', 'Krista.FM.RIA.Extensions.FO41.FO41ExtensionInstaller, Krista.FM.RIA.Extensions.FO41');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (240, 'RIA.EO12InvestProjects', 'Инвестиционные проекты', 'Krista.FM.RIA.Extensions.EO12InvestProjects.InvestProjectsExtensionInstaller, Krista.FM.RIA.Extensions.EO12InvestProjects');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (241, 'RIA.OrgGKH', 'Сбор по ЖКХ', 'Krista.FM.RIA.Extensions.OrgGKH.OrgGKHExtensionInstaller, Krista.FM.RIA.Extensions.OrgGKH');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (250, 'RIA.EO14InvestAreas', 'Инвестиционные площадки', 'Krista.FM.RIA.Extensions.EO14InvestAreas.InvestAreasExtensionInstaller, Krista.FM.RIA.Extensions.EO14InvestAreas');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (251, 'RIA.EO10MessagePRF', 'Послание ПРФ', 'Krista.FM.RIA.Extensions.EO10MissivePRF.EO10ExtensionInstaller, Krista.FM.RIA.Extensions.EO10MissivePRF');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (252, 'RIA.FO51PassportMO', 'ФО_0051_Паспорт МО', 'Krista.FM.RIA.Extensions.FO51PassportMO.FO51ExtensionInstaller, Krista.FM.RIA.Extensions.FO51PassportMO');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (253, 'RIA.EO15TargetPrograms', 'Целевые программы', 'Krista.FM.RIA.Extensions.EO15TargetPrograms.TargetProgramsExtensionInstaller, Krista.FM.RIA.Extensions.EO15TargetPrograms');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (260, 'RIA.MinSport', 'Спортивная карта России', 'Krista.FM.RIA.Extensions.MinSport.ExtensionInstaller, Krista.FM.RIA.Extensions.MinSport');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (261, 'RIA.FO41HMAO', 'ФО_0041_Оценка эффективности льгот ХМАО', 'Krista.FM.RIA.Extensions.FO41HMAO.FO41HMAOExtensionInstaller, Krista.FM.RIA.Extensions.FO41HMAO');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (263, 'RIA.E86N', 'Сбор информации для официального сайта ГМУ', 'Krista.FM.RIA.Extensions.E86N.E86NExtensionInstaller, Krista.FM.RIA.Extensions.E86N');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (264, 'RIA.EO15ExcCostsAIP', 'ЭО15 Иcполнение расходов АИП', 'Krista.FM.RIA.Extensions.EO15ExcCostsAIP.EO15ExcCostsAIPExtensionInstaller, Krista.FM.RIA.Extensions.EO15ExcCostsAIP');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (270, 'MessagesUI', 'Подсистема сообщений', 'Krista.FM.Client.ViewObjects.MessagesUI.MessageManagementNavigation, Krista.FM.Client.ViewObjects.MessagesUI');
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (280, 'RIA.Region10MarksOIV', 'Оценка ОИВ', 'Krista.FM.RIA.Extensions.Region10MarksOIV.Region10MarksOivExtensionInstaller, Krista.FM.RIA.Extensions.Region10MarksOIV');

commit work;


/* Константы */
create table GlobalConsts
(
	ID					number (10) not null,		/* PK */
	Name				varchar2 (255) not null,	/* английское имя константы */
	Caption				varchar2 (1000) not null,	/* русское наименование */
	Description			varchar2 (2000) not null,	/* описание константы */
	Value				varchar2 (4000) not null,	/* значение константы */
	ConstValueType		number (10) not null,		/* тип значения константы */
	ConstCategory		number (10) not null,		/* категория константы */
	ConstType 			number (10) not null,		/* тип константы */
	constraint PKGlobalConsts primary key ( ID ),
	constraint UKGlobalConsts_Name unique ( Name )
);

create sequence g_GlobalConsts;

create or replace trigger t_GlobalConsts_i before insert on GlobalConsts for each row
begin
	if :new.ID is null then select g_GlobalConsts.NextVal into :new.ID from Dual; end if;
end t_GlobalConsts_i;
/

create or replace trigger t_GlobalConsts_AA after insert OR update OR delete on GlobalConsts for each row
declare
	UserName varchar2(64);
	SessionID varchar2(24);
begin
	SELECT SYS_CONTEXT ('DVContext', 'USERNAME'), SYS_CONTEXT('DVContext', 'SESSIONID') into UserName, SessionID FROM dual;
	if inserting then insert into DVAudit.DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (0, 'GlobalConsts', 5, UserName, SessionID, :new.ID);
	elsif updating then insert into DVAudit.DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (1, 'GlobalConsts', 5, UserName, SessionID, :new.ID);
	elsif deleting then insert into DVAudit.DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (2, 'GlobalConsts', 5, UserName, SessionID, :old.ID); end if;
end;
/

create table Personalization 
(
	UserName        varchar2 (255),
	Application     varchar2 (1000),
	PageSettings    blob default empty_blob(),
	constraint UK_Pers UNIQUE ( UserName, Application )
);

/* Таблица для хранения соответствий длинных и сокращенных идентификаторов */
create table HashObjectsNames
(
     HashName varchar2(30) not null, 	                               -- Хешированное имя 
     LongName varchar2(2048) not null, 	                               -- Полное имя 
     ObjectType number(10) not null, 	                               -- Тип объекта (перечисление Krista.FM.Server.Scheme.ScriptingEngine.ObjectTypes)
     constraint PKHashObjectsNames primary key (HashName, ObjectType)
);

commit;