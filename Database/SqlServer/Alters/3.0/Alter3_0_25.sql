/********************************************************************
	Переводит базу MS SQL из версии 3.0 в следующую версию 3.1 
********************************************************************/

/* Шаблон оформления альтера: */ 
/* Start - <Номер задачи в Reamine> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */ 
	/* Ваш SQL-скрипт */
/* End - <Номер задачи в Reamine> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */ 


/* Start - Стандартная часть */ 

/* Выходим по любой ошибке */ 
:on error exit 
/* End   - Стандартная часть */ 

/* Скрипт будет выполнять каждый год, поэтому при выпуске версии его надо будет добаить в MetadataDependent */

print N'Start - Feature #20206 - Скрипт на заполнение классификатора «Период.Рабочие дни». Включить в Seconds альтеры.  - tsvetkov - 10.02.2012'
GO

DECLARE @BinVar varbinary(128)
SET @BinVar = CAST('0' as varbinary(1)) +
                    CAST((CAST('000' AS char(24))) AS varbinary(24) ) +
                    CAST((CAST('ALTERUSER' AS char(64))) AS varbinary(64))
SET CONTEXT_INFO @BinVar
go

delete from d_Date_WorkingDays;
go

IF OBJECT_ID ('t_Date_WorkingDays_BI', 'TR') IS NOT NULL
   DROP TRIGGER t_Date_WorkingDays_BI;
GO

create trigger t_Date_WorkingDays_BI on d_Date_WorkingDays instead of insert as
begin
	declare @nullsCount int;
	set nocount on;
	select @nullsCount = count(*) from inserted where ID is null;
	if @nullsCount = 0 insert into d_Date_WorkingDays (ID, DaysWorking)
		select ID, DaysWorking from inserted;
	else if @nullsCount = 1
	begin
		insert into g.d_Date_WorkingDays default values;
		delete from g.d_Date_WorkingDays where ID = @@IDENTITY;
		insert into d_Date_WorkingDays (ID, DaysWorking)
			select @@IDENTITY, DaysWorking from inserted;
	end else
		RAISERROR (5000001, 1, 1);
end;
go

declare @mx varchar(8)
declare @mn varchar(8)

select @mn = convert(varchar(8), min(DayoffDate), 112) from d_Fact_Holidays
select @mx = convert(varchar(8), max(DayoffDate), 112) from d_Fact_Holidays

declare @v_RefFODate int;
DECLARE ConversionFKCursor CURSOR FOR 
SELECT RefFODate
from d_Date_ConversionFK con
where not exists 
			(select null from d_Fact_Holidays 
				where convert(varchar(8), DayoffDate, 112) = cast(con.RefFODate as varchar(8))
				and DayOff = 1)
		and convert(varchar(8), RefFODate, 112) >= @mn 
		and convert(varchar(8), RefFODate, 112) <= @mx

OPEN ConversionFKCursor;

FETCH NEXT FROM ConversionFKCursor INTO @v_RefFODate

WHILE @@FETCH_STATUS = 0
BEGIN
	insert into d_Date_WorkingDays (DaysWorking) values(@v_RefFODate)

	FETCH NEXT FROM ConversionFKCursor INTO @v_RefFODate
end;

CLOSE ConversionFKCursor;
DEALLOCATE ConversionFKCursor;

------------------------------------------------------------

declare @v_DaysWorking int;
DECLARE DaysWorkingCursor CURSOR FOR 
	select convert(int, convert(varchar, DayoffDate, 112)) from d_Fact_Holidays
		where DayOff = 0

OPEN DaysWorkingCursor;

FETCH NEXT FROM DaysWorkingCursor INTO @v_DaysWorking

WHILE @@FETCH_STATUS = 0
BEGIN
	insert into d_Date_WorkingDays (DaysWorking) values(@v_DaysWorking)

	FETCH NEXT FROM DaysWorkingCursor INTO @v_DaysWorking
end;

CLOSE DaysWorkingCursor;
DEALLOCATE DaysWorkingCursor;

go

------------------------------------------------------------

-- удалим дубликаты
delete from d_Date_WorkingDays
	where (id not in (select max(t2.id) from d_Date_WorkingDays t2 
		group by t2.DaysWorking )) AND (id not in (select max(t2.id) from d_Date_WorkingDays t2 
			group by t2.DaysWorking ))
go


print N'End - Feature #20206 - Скрипт на заполнение классификатора «Период.Рабочие дни» - tsvetkov - 10.02.2012'
GO

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (120, '3.0.0.25', CONVERT(datetime, '2012.02.10', 102), GETDATE(), 'Заполнение классификатора «Период.Рабочие дни»"', 0);
go