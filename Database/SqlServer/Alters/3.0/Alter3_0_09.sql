/********************************************************************
    Переводит базу SQL Server из версии 3.0 в следующую версию 3.x
********************************************************************/

/* Start - #17838 - Добавление нового интерфейса "Сбор по ЖКХ" - barhonina - 07.10.2011 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (241, 'RIA.OrgGKH', 'Сбор по ЖКХ', 'Krista.FM.RIA.Extensions.OrgGKH.OrgGKHExtensionInstaller, Krista.FM.RIA.Extensions.OrgGKH');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (104, '3.0.0.9', CONVERT(datetime, '2011.10.07', 102), GETDATE(), 'Зарегистрирован web-интерфейс пользователя "Сбор по ЖКХ"', 0);

if (not exists (select null from Groups where [Name] = 'МО сбор ЖКХ'))
begin
  insert into g.Groups default values;
  delete from g.Groups where ID = @@IDENTITY;
  INSERT INTO DV.Groups
    (ID, [Name], Description, Blocked, DNSName) 
    VALUES (@@IDENTITY, 'МО сбор ЖКХ', 'МО сбор ЖКХ', 0, '')
end;

if (not exists (select null from Groups where [Name] = 'Аудит сбор ЖКХ'))
begin
  insert into g.Groups default values;
  delete from g.Groups where ID = @@IDENTITY;
  INSERT INTO DV.Groups
    (ID, [Name], Description, Blocked, DNSName) 
    VALUES (@@IDENTITY, 'Аудит сбор ЖКХ', 'Аудит сбор ЖКХ', 0, '')
end;

if (not exists (select null from Groups where [Name] = 'Организации сбор ЖКХ'))
begin
  insert into g.Groups default values;
  delete from g.Groups where ID = @@IDENTITY;
  INSERT INTO DV.Groups
    (ID, [Name], Description, Blocked, DNSName) 
    VALUES (@@IDENTITY, 'Организации сбор ЖКХ', 'Организации сбор ЖКХ', 0, '')
end;

if (not exists (select null from Groups where [Name] = 'ИОГВ сбор ЖКХ'))
begin
  insert into g.Groups default values;
  delete from g.Groups where ID = @@IDENTITY;
  INSERT INTO DV.Groups
    (ID, [Name], Description, Blocked, DNSName) 
    VALUES (@@IDENTITY, 'ИОГВ сбор ЖКХ', 'ИОГВ сбор ЖКХ', 0, '')
end;

go

/* End - #17838 - Добавление нового интерфейса "Сбор по ЖКХ" - barhonina - 07.10.2011 */

