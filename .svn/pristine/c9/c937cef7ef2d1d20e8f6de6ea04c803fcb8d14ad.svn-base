/********************************************************************
    Переводит базу SQL Server из версии 3.0 в следующую версию 3.x
********************************************************************/

/* Start - #19087 - Добавление нового интерфейса "ФО_0051_Паспорт МО" - barhonina - 12.11.2011 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (252, 'RIA.FO51PassportMO', 'ФО_0051_Паспорт МО','Krista.FM.RIA.Extensions.FO51PassportMO.FO51ExtensionInstaller, Krista.FM.RIA.Extensions.FO51PassportMO');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (109, '3.0.0.13', CONVERT(datetime, '2011.11.12', 102), GETDATE(), 'Зарегистрирован web-интерфейс пользователя "ФО_0051_Паспорт МО"', 0);

go

/* End - #19087 - Добавление нового интерфейса "ФО_0051_Паспорт МО" - barhonina - 12.11.2011 */

