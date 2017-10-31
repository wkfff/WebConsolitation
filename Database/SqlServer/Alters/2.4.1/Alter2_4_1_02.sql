/*******************************************************************
 Переводит базу Sql Server 2005 из версии 2.3.1 в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - 8769 - Workplace MDI - gbelov - 02.07.2008 */

alter table RegisteredUIModules
	add FullName varchar (255);
	
go

update RegisteredUIModules
	set FullName = 'Krista.FM.Client.ViewObjects.AdministrationUI.AdministrationNavigation, Krista.FM.Client.ViewObjects.AdministrationUI'
	where Name = 'AdministrationUI';

update RegisteredUIModules
	set FullName = 'Krista.FM.Client.ViewObjects.MDObjectsManagementUI.MDObjectsManagementNavigation, Krista.FM.Client.ViewObjects.MDObjectsManagementUI'
	where Name = 'MDObjectsManagementUI';

update RegisteredUIModules
	set FullName = 'Krista.FM.Client.ViewObjects.ProtocolsUI.ProtocolsNavigation, Krista.FM.Client.ViewObjects.ProtocolsUI'
	where Name = 'ProtocolsViewObject';

update RegisteredUIModules
	set FullName = 'Krista.FM.Client.ViewObjects.AssociatedCLSUI.FixedClsNavigation, Krista.FM.Client.ViewObjects.AssociatedCLSUI'
	where Name = 'FixedClsUI';

update RegisteredUIModules
	set FullName = 'Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataClsNavigation, Krista.FM.Client.ViewObjects.AssociatedCLSUI'
	where Name = 'DataClsUI';

update RegisteredUIModules
	set FullName = 'Krista.FM.Client.ViewObjects.AssociatedCLSUI.AssociatedClsNavigation, Krista.FM.Client.ViewObjects.AssociatedCLSUI'
	where Name = 'AssociatedClsUI';

update RegisteredUIModules
	set FullName = 'Krista.FM.Client.ViewObjects.AssociatedCLSUI.Association.AssociationNavigation, Krista.FM.Client.ViewObjects.AssociatedCLSUI'
	where Name = 'AssociationUI';

update RegisteredUIModules
	set FullName = 'Krista.FM.Client.ViewObjects.AssociatedCLSUI.TranslationsTables.TranslationTablesNavigation, Krista.FM.Client.ViewObjects.AssociatedCLSUI'
	where Name = 'TranslationsTablesUI';

update RegisteredUIModules
	set FullName = 'Krista.FM.Client.ViewObjects.DataPumpUI.DataPumpNavigation, Krista.FM.Client.ViewObjects.DataPumpUI'
	where Name = 'DataPumpUI';

update RegisteredUIModules
	set FullName = 'Krista.FM.Client.ViewObjects.DataSourcesUI.DataSourcesNavigation, Krista.FM.Client.ViewObjects.DataSourcesUI'
	where Name = 'DataSourcesUI';

update RegisteredUIModules
	set FullName = 'Krista.FM.Client.ViewObjects.DisintRulesUI.DisintRulesNavigation, Krista.FM.Client.ViewObjects.DisintRulesUI'
	where Name = 'DisintRulesUI';

update RegisteredUIModules
	set FullName = 'Krista.FM.Client.ViewObjects.TasksUI.TasksNavigation, Krista.FM.Client.ViewObjects.TasksUI'
	where Name = 'TasksViewObj';

update RegisteredUIModules
	set FullName = 'Krista.FM.Client.ViewObjects.AssociatedCLSUI.FactTablesNavigation, Krista.FM.Client.ViewObjects.AssociatedCLSUI'
	where Name = 'FactTablesUI';

update RegisteredUIModules
	set FullName = 'Krista.FM.Client.ViewObjects.GlobalConstsUI.GlobalConstsNavigation, Krista.FM.Client.ViewObjects.GlobalConstsUI'
	where Name = 'GlobalConstsViewObj';

update RegisteredUIModules
	set FullName = 'Krista.FM.Client.ViewObjects.TemplatesUI.TemplatesNavigation, Krista.FM.Client.ViewObjects.TemplatesUI'
	where Name = 'TemplatesViewObj';

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (50, '2.4.1.2', CONVERT(datetime, '2008.07.02', 102), GETDATE(), 'Изменения в структуре регистрации подключаемых модулей к воркплейсу.', 0);

go

/* End   - 8769 - Workplace MDI - gbelov - 02.07.2008 */
