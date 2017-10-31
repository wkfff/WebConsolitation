/********************************************************************
    Переводит базу SQL Server из версии 3.0 в следующую версию 3.x
********************************************************************/

/* Start - #19437 - Добавление нового интерфейса "ФО_0041_Оценка эффективности льгот ХМАО" - barhonina - 06.12.2011 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (261, 'RIA.FO41HMAO', 'ФО_0041_Оценка эффективности льгот ХМАО', 'Krista.FM.RIA.Extensions.FO41HMAO.FO41HMAOExtensionInstaller, Krista.FM.RIA.Extensions.FO41HMAO');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (113, '3.0.0.13', To_Date('06.12.2011', 'dd.mm.yyyy'), SYSDATE, 'Зарегистрирован web-интерфейс пользователя "ФО_0041_Оценка эффективности льгот ХМАО"', 0);

commit;

/* End - #19437 - Добавление нового интерфейса "ФО_0041_Оценка эффективности льгот ХМАО" - barhonina - 06.12.2011 */

