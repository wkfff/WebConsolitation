/********************************************************************
	Переводит базу SQL Server из версии 3.0 в следующую версию 3.X 
********************************************************************/

/* Start - - Добавление нового интерфейса "Сбор данных" - gbelov - 25.05.2011 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (220, 'RIA.Consolidation', 'Сбор данных', 'Krista.FM.RIA.Extensions.Consolidation.ConsolidationExtensionInstaller, Krista.FM.RIA.Extensions.Consolidation');

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (221, 'RIA.Consolidation.Admin', 'Администратор сбора данных', 'Krista.FM.RIA.Extensions.Consolidation.ConsolidationAdminExtensionInstaller, Krista.FM.RIA.Extensions.Consolidation');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (97, '3.0.0.2', CONVERT(datetime, '2011.05.25', 102), GETDATE(), 'Зарегистрирован web-интерфейс пользователя "Сбор данных"', 0);

go

/* End - - Добавление нового интерфейса "Сбор данных" - gbelov - 25.05.2011 */
