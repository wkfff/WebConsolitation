/********************************************************************
	Переводит базу SQL Server из версии 3.0 в следующую версию 3.X 
********************************************************************/

/* Start - Стандартная часть */
/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */

/* Start - - Добавление нового интерфейса "Оценка эффективности льгот" - barhonina - 22.07.2011 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (230, 'RIA.FO41', 'Оценка эффективности льгот', 'Krista.FM.RIA.Extensions.FO41.FO41ExtensionInstaller, Krista.FM.RIA.Extensions.FO41');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (99, '3.0.0.4', To_Date('22.07.2011', 'dd.mm.yyyy'), SYSDATE, 'Зарегистрирован web-интерфейс пользователя "Оценка эффективности льгот"', 0);

commit;

whenever SQLError exit rollback;

/* End - - Добавление нового интерфейса "Оценка эффективности льгот" - barhonina - 22.07.2011 */
