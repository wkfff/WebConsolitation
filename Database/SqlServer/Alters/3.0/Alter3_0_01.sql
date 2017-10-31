/********************************************************************
	Переводит базу SQL Server из версии 3.0 в следующую версию 3.x
********************************************************************/

/* Start - #15909 - Добавление нового интерфейса "Отчеты системы" - paluh - 14.04.2011 */

/* Регистрация интерфейса пользоватебя "Отчеты системы" */
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (210, 'ReportsUI', 'Отчеты системы', 'Krista.FM.Client.ViewObjects.ReportsUI.Gui.ReportsNavigation, Krista.FM.Client.ViewObjects.ReportsUI');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (106, '3.0.0.1', CONVERT(datetime, '2011.04.14', 102), GETDATE(), 'Зарегистрирован интерфейс пользователя "Отчеты системы"', 0);

go

/* End - #15909 - Добавление нового интерфейса "Отчеты системы" - paluh - 14.04.2011 */