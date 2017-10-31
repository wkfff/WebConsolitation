/********************************************************************
	Переводит базу SQL Server из версии 2.7 в следующую версию 2.8 
********************************************************************/

/* Start - - Для SqlServer добавлена таблица DUAL для совместимости с Oracle - gbelov - 21.01.2011 */

create table DUAL (DUMMY varchar(1) not null check (DUMMY='X') primary key);
insert into DUAL (DUMMY) values ('X');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (91, '2.7.0.12', CONVERT(datetime, '2011.01.21', 102), GETDATE(), 'Создана наблица DUAL', 0);

go

/* End - - Для SqlServer добавлена таблица DUAL для совместимости с Oracle - gbelov - 21.01.2011 */
