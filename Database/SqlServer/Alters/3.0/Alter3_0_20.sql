/*******************************************************************
  Переводит базу Sql Server 2005 из версии 3.0 в следующую версию 3.X
*******************************************************************/

/* Start - - Добавлена таблица для хранения соответствий длинных и сокращенных идентификаторов - gbelov - 22.12.2011 */

create table HashObjectsNames
(
     HashName varchar(30) not null, 	-- Хешированное имя 
     LongName varchar(2048) not null, 	-- Полное имя 
     ObjectType numeric(10) not null, 	-- Тип объекта (перечисление Krista.FM.Server.Scheme.ScriptingEngine.ObjectTypes)
     constraint PKHashObjectsNames primary key (HashName, ObjectType)
)
go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (116, '3.0.0.20', CONVERT(datetime, '2011.12.22', 102), GETDATE(), 'Добавлена таблица для хранения соответствий длинных и сокращенных идентификаторов', 0);

go

/* End - - Добавлена таблица для хранения соответствий длинных и сокращенных идентификаторов - gbelov - 22.12.2011 */
