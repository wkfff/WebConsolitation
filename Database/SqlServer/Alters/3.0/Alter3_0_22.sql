/********************************************************************
	Переводит базу SQL Server из версии 3.0 в следующую версию 3.X 
********************************************************************/

/* Start - #20138 - Добавление нового интерфейса "Иcполнение расходов АИП" - barhonina - 29.01.2012 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (264, 'RIA.EO15ExcCostsAIP', 'ЭО15 Иcполнение расходов АИП', 'Krista.FM.RIA.Extensions.EO15ExcCostsAIP.EO15ExcCostsAIPExtensionInstaller, Krista.FM.RIA.Extensions.EO15ExcCostsAIP');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (117, '3.0.0.22', CONVERT(datetime, '2012.01.29', 102), GETDATE(), 'Зарегистрирован web-интерфейс пользователя "Иcполнение расходов АИП"', 0);

if (not exists (select null from Groups where [Name] = 'ЭО15_Строительство МО'))
begin
  insert into g.Groups default values;
  delete from g.Groups where ID = @@IDENTITY;
  INSERT INTO DV.Groups
    (ID, [Name], Description, Blocked, DNSName) 
    VALUES (@@IDENTITY, 'ЭО15_Строительство МО', 'МО Исполнение расходов АИП', 0, '')
end;

if (not exists (select null from Groups where [Name] = 'ЭО15_Строительство Заказчики'))
begin
  insert into g.Groups default values;
  delete from g.Groups where ID = @@IDENTITY;
  INSERT INTO DV.Groups
    (ID, [Name], Description, Blocked, DNSName) 
    VALUES (@@IDENTITY, 'ЭО15_Строительство Заказчики', 'Заказчики Исполнение расходов АИП', 0, '')
end;

if (not exists (select null from Groups where [Name] = 'ЭО15_Строительство Координаторы'))
begin
  insert into g.Groups default values;
  delete from g.Groups where ID = @@IDENTITY;
  INSERT INTO DV.Groups
    (ID, [Name], Description, Blocked, DNSName) 
    VALUES (@@IDENTITY, 'ЭО15_Строительство Координаторы', 'Координаторы Исполнение расходов АИП', 0, '')
end;

if (not exists (select null from Groups where [Name] = 'ЭО15_Строительство Пользователи'))
begin
  insert into g.Groups default values;
  delete from g.Groups where ID = @@IDENTITY;
  INSERT INTO DV.Groups
    (ID, [Name], Description, Blocked, DNSName) 
    VALUES (@@IDENTITY, 'ЭО15_Строительство Пользователи', 'Пользователи Исполнение расходов АИП', 0, '')
end;

go

/* End - #20138 - Добавление нового интерфейса "Иcполнение расходов АИП" - barhonina - 29.01.2012 */
