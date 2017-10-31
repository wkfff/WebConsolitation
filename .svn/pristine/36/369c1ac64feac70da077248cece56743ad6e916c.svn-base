/********************************************************************
    Переводит базу SQL Server из версии 3.0 в следующую версию 3.x
********************************************************************/

/* Start - #19256 - Добавление нового интерфейса "Целевые программы" - vorontsov - 23.11.2011 */

insert into DV.RegisteredUIModules (ID, Name, Description, FullName) 
values (253, 'RIA.EO15TargetPrograms', 'Целевые программы', 'Krista.FM.RIA.Extensions.EO15TargetPrograms.TargetProgramsExtensionInstaller, Krista.FM.RIA.Extensions.EO15TargetPrograms');

if (not exists (select null from Groups where [Name] = 'ЭО15_ЦП Заказчики'))
begin
   insert into g.Groups default values;
   delete from g.Groups where ID = @@IDENTITY;
   insert into DV.Groups (ID, [Name], Description, Blocked) values (@@IDENTITY, 'ЭО15_ЦП Заказчики', 'Заказчик Целевых программ', 0);
end;

if (not exists (select null from Groups where [Name] = 'ЭО15_ЦП Координаторы'))
begin
   insert into g.Groups default values;
   delete from g.Groups where ID = @@IDENTITY;
   insert into DV.Groups (ID, [Name], Description, Blocked) values (@@IDENTITY, 'ЭО15_ЦП Координаторы', 'ЭО Целевых программ', 0);
end;

insert into DV.DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (111, '3.0.0.15', CONVERT(datetime, '2011.11.23', 102), GETDATE(), 'Зарегистрирован web-интерфейс пользователя "Целевые программы"', 0);

go

/* End - #19256 - Добавление нового интерфейса "Целевые программы" - vorontsov - 23.11.2011 */
