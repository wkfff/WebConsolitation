/********************************************************************
	Переводит базу SQL Server из версии 3.0 в следующую версию 3.X 
********************************************************************/

/* Start - Стандартная часть */
/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */

/* Start - - Добавление нового интерфейса "Спортивная карта России" - nikonenko - 03.12.2011 */

/* Регистрация интерфейса пользователя "Спортивная карта России" */
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (260, 'RIA.MinSport', 'Спортивная карта России', 'Krista.FM.RIA.Extensions.MinSport.ExtensionInstaller, Krista.FM.RIA.Extensions.MinSport');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (112, '3.0.0.16', To_Date('03.12.2011', 'dd.mm.yyyy'), SYSDATE, 'Зарегистрирован интерфейс пользователя "Спортивная карта России"', 0);

commit;

whenever SQLError exit rollback;

/* End - - Добавление нового интерфейса "Спортивная карта России" - nikonenko - 03.12.2011 */
