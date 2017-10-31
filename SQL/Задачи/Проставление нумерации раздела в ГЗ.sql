/* Start - #4788 - Нумерация раздела в ГЗ - shelpanov - 15.11.2012 */

print N'Отключение триггера'
GO

disable trigger t_f_F_GosZadanie_aa on [DV].[f_F_GosZadanie];

print N'Нумеруем....'
GO

create procedure Set_Numbering as
DECLARE  @RowID  int 
DECLARE  @RefParametr  int 
DECLARE  @Type  int 
DECLARE  @RefParametrCurr  int 
DECLARE  @WorkCounter  int
DECLARE  @ServiceCounter  int 

/*  Услуги и работы в ГЗ*/
declare Docs cursor for
SELECT g.[ID], g.[RefParametr], s.[RefTipY]
FROM [DV].[f_F_GosZadanie] g join [DV].[d_Services_VedPer] s on(g.RefVedPch = s.ID)
order by g.[RefParametr], s.[RefTipY]

begin
    set @RefParametrCurr = -1;

	open Docs
	fetch next from Docs into @RowID, @RefParametr, @Type;
	while @@FETCH_STATUS <> -1 begin
		
		if (@RefParametrCurr <> @RefParametr)
		begin
		  set @RefParametrCurr  = @RefParametr;
		  set @WorkCounter = 1;
		  set @ServiceCounter = 1;
		end
		
		if (@Type = 1)
		begin
		  update [DV].[f_F_GosZadanie]
	      set RazdelN = @ServiceCounter
		  where ID = @RowID;
		  
		  set @ServiceCounter = @ServiceCounter + 1;
		end
		else
		begin
		  update [DV].[f_F_GosZadanie]
	      set RazdelN = @WorkCounter
		  where ID = @RowID;
		  
		  set @WorkCounter = @WorkCounter + 1;
		end

	  fetch next from Docs into @RowID, @RefParametr, @Type
	end
	close Docs;
	deallocate Docs;
end	
go

Exec Set_Numbering
go

drop procedure Set_Numbering
go

print N'Включение триггера'
GO

enable trigger t_f_F_GosZadanie_aa on [DV].[f_F_GosZadanie];
 
GO

/* End - #4788 - Нумерация раздела в ГЗ - shelpanov - 15.11.2012 */