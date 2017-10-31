/********************************************************************
    Переводит базу SQL Server из версии 3.0 в следующую версию 3.X 
********************************************************************/

/* Start - #20568 - Добавление нового интерфейса "Оценка ОИВ" - vorontsov - 02.03.2012 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (280, 'RIA.Region10MarksOIV', 'Оценка ОИВ', 'Krista.FM.RIA.Extensions.Region10MarksOIV.Region10MarksOivExtensionInstaller, Krista.FM.RIA.Extensions.Region10MarksOIV');

if (not exists (select null from Groups where [Name] = 'Оценка ОИВ'))
begin
  insert into g.Groups default values;
  delete from g.Groups where ID = @@IDENTITY;
  INSERT INTO DV.Groups
    (ID, [Name], Description, Blocked, DNSName) 
    VALUES (@@IDENTITY, 'Оценка ОИВ', 'Пользователи ОМСУ и ОИВ блока "Оценка ОИВ"', 0, '')
end;

if (not exists (select null from Groups where [Name] = 'Оценка ОИВ_Утверждение Доклада'))
begin
  insert into g.Groups default values;
  delete from g.Groups where ID = @@IDENTITY;
  INSERT INTO DV.Groups
    (ID, [Name], Description, Blocked, DNSName) 
    VALUES (@@IDENTITY, 'Оценка ОИВ_Утверждение Доклада', 'Пользователи УИМ блока "Оценка ОИВ"', 0, '')
end;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (121, '3.0.0.26', CONVERT(datetime, '2012.03.02', 102), GETDATE(), 'Зарегистрирован web-интерфейс пользователя "Оценка ОИВ"', 0);

go

/* End - #20568 - Добавление нового интерфейса "Оценка ОИВ" - vorontsov - 02.03.2012 */
