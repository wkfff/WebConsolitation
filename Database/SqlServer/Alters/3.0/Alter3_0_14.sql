/********************************************************************
    Переводит базу SQL Server из версии 3.0 в следующую версию 3.x
********************************************************************/

/* Start - #14339 - Для процедуры генерации очередного ID добавлена блокировка на уровне таблицы" - tsvetkov - 17.11.2011 */

drop procedure usp_Generator
go

create procedure usp_Generator @generator varchar (100) as
begin
	-- set nocount on added to prevent extra result sets from
	-- interfering with select statements.
	set nocount on;

	exec ('insert into g.' + @generator + ' with(tablock) default values;');
	exec ('delete from g.' + @generator + ' where ID = @@IDENTITY;');
	return @@IDENTITY;
end;

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (110, '3.0.0.14', CONVERT(datetime, '2011.11.17', 102), GETDATE(), 'Для процедуры генерации очередного ID добавлена блокировка на уровне таблицы"', 0);

go

/* End - #14339 - Для процедуры генерации очередного ID добавлена блокировка на уровне таблицы" - tsvetkov - 17.11.2011 */

