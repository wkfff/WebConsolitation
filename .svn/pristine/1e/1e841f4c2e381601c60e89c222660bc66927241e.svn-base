/********************************************************************
    Переводит базу SQL Server из версии 3.0 в следующую версию 3.x
********************************************************************/

/* Start - #18999 - Добавление нового интерфейса "Послание ПРФ" - barhonina - 08.11.2011 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (251, 'RIA.EO10MessagePRF', 'Послание ПРФ', 'Krista.FM.RIA.Extensions.EO10MissivePRF.EO10ExtensionInstaller, Krista.FM.RIA.Extensions.EO10MissivePRF');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (108, '3.0.0.12', CONVERT(datetime, '2011.11.08', 102), GETDATE(), 'Зарегистрирован web-интерфейс пользователя "Послание ПРФ"', 0);

go

/* End - #18999 - Добавление нового интерфейса "Послание ПРФ" - barhonina - 08.11.2011 */

