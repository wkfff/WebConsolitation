/********************************************************************
	Переводит базу SQL Server из версии 3.0 в следующую версию 3.x
********************************************************************/

/* Start - - Добавление нового интерфейса "Спортивная карта России" - nikonenko - 03.12.2011 */

/* Регистрация интерфейса пользователя "Спортивная карта России" */
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (260, 'RIA.MinSport', 'Спортивная карта России', 'Krista.FM.RIA.Extensions.MinSport.ExtensionInstaller, Krista.FM.RIA.Extensions.MinSport');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (112, '3.0.0.16', CONVERT(datetime, '2012.12.03', 102), GETDATE(), 'Зарегистрирован интерфейс пользователя "Спортивная карта России"', 0);

go

/* End - - Добавление нового интерфейса "Спортивная карта России" - nikonenko - 03.12.2011 */