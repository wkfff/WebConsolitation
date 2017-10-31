/*******************************************************************
 Переводит базу Oracle из версии 3.0	 в следующую версию 3.X
*******************************************************************/

/* Start - - Добавлена таблица для хранения соответствий длинных и сокращенных идентификаторов - gbelov - 22.12.2011 */

create table HashObjectsNames
(
     HashName varchar2(30) not null, 	-- Хешированное имя 
     LongName varchar2(2048) not null, 	-- Полное имя 
     ObjectType number(10) not null, 	-- Тип объекта (перечисление Krista.FM.Server.Scheme.ScriptingEngine.ObjectTypes)
     constraint PKHashObjectsNames primary key (HashName, ObjectType)
);

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (116, '3.0.0.20', To_Date('22.12.2011', 'dd.mm.yyyy'), SYSDATE, 'Добавлена таблица для хранения соответствий длинных и сокращенных идентификаторов', 0);

commit;

whenever SQLError exit rollback;

/* End - - Добавлена таблица для хранения соответствий длинных и сокращенных идентификаторов - gbelov - 22.12.2011 */
