/********************************************************************
    Переводит базу SQL Server из версии 3.0 в следующую версию 3.x
********************************************************************/

/* Start - #18503 - Добавление нового интерфейса "Инвестиционные площадки" - vorontsov - 24.10.2011 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (250, 'RIA.EO14InvestAreas', 'Инвестиционные площадки', 'Krista.FM.RIA.Extensions.EO14InvestAreas.InvestAreasExtensionInstaller, Krista.FM.RIA.Extensions.EO14InvestAreas');

insert into g.Groups default values;
delete from g.Groups where ID = @@IDENTITY;
insert into DV.Groups (ID, [Name], Description, Blocked) values (@@IDENTITY, 'EO14_Creator', 'Создатели ИП', 0);

insert into g.Groups default values;
delete from g.Groups where ID = @@IDENTITY;
insert into DV.Groups (ID, [Name], Description, Blocked) values (@@IDENTITY, 'EO14_Coordinator', 'Координаторы ИП', 0);

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (105, '3.0.0.10', CONVERT(datetime, '2011.10.24', 102), GETDATE(), 'Зарегистрирован web-интерфейс пользователя "Инвестиционные площадки"', 0);

go

/* End - #18503 - Добавление нового интерфейса "Инвестиционные площадки" - vorontsov - 24.10.2011 */
