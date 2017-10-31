/*******************************************************************
 Переводит базу Sql Server 2005 из версии 2.3.1 в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - 10111 - Исправление ошибки в триггере t_Batch_bi - tsvetkov - 25.02.2009 */

alter trigger t_Batch_bi on Batch instead of insert as
begin
	declare @nullsCount int;
	set nocount on;
	select @nullsCount = count(*) from inserted where ID is null;
	if @nullsCount = 0 insert into Batch (ID, RefUser, AdditionTime, BatchState, SessionId, BatchId, Priority) select ID, RefUser, AdditionTime, BatchState, SessionId, BatchId, Priority from inserted;
	else if @nullsCount = 1
	begin
		insert into g.Batch default values;
		delete from g.Batch where ID = @@IDENTITY;
		insert into Batch (ID, RefUser, AdditionTime, BatchState, SessionId, BatchId, Priority) select @@IDENTITY, RefUser, AdditionTime, BatchState, BatchId, SessionId, Priority from inserted;
	end else
		RAISERROR (500001, 1, 1);
end;

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (64, '2.5.0.8', CONVERT(datetime, '2009.02.25', 102), GETDATE(), 'Исправление ошибки в триггере t_Batch_bi', 0);

go

/* End - 10111 - Исправление ошибки в триггере t_Batch_bi - tsvetkov - 25.02.2009 */