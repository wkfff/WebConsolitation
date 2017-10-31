create procedure Translation_States as
DECLARE  @Doc  int 
DECLARE  @Note varchar(1024)

/*  Планы ПФХД завершенные и только первые года планов не заполненые */
declare TmpPFHD cursor for
  select p.RefParametr, d.Note
  from [DV].[f_Fin_finActPlan] p, [DV].[f_F_ParameterDoc] d
  where (p.RefParametr = d.ID) and (d.RefSost = 7) and (p.NumberStr = 0) and 
		(p.totnonfinAssets = 0) and
		(p.realAssets = 0) and
		(p.highValPersAssets = 0) and
		(p.finAssets = 0) and
		(p.income = 0) and
		(p.expense = 0) and
		(p.finCircum = 0) and
		(p.kreditExpir = 0) and
		(p.stateTaskGrant = 0) and
		(p.actionGrant = 0) and
		(p.budgetaryFunds = 0) and
		(p.paidServices = 0) and
		(p.planPayments = 0) and
		(p.planInpayments = 0) and
		(p.labourRemuneration = 0) and
		(p.telephoneServices = 0) and
		(p.freightServices = 0) and
		(p.publicServeces = 0) and
		(p.rental = 0) and
		(p.maintenanceCosts = 0) and
		(p.mainFunds = 0) and
		(p.fictitiousAssets = 0) and
		(p.tangibleAssets = 0) and
		(p.publish = 0)
		
/*  Целевые субсидии у которых не указаны субсидии или указана с кодом 0 */
declare TmpGrantFunds cursor for
    select p.RefParametr, d.Note
	from [DV].[f_Fin_othGrantFunds] p join [DV].[f_F_ParameterDoc] d on (p.RefParametr = d.ID) 
		 left join [DV].[d_Fin_OtherGant] g on (p.RefOtherGrant = g.ID)
	where (d.RefSost = 7) and ((p.RefOtherGrant is Null) OR (g.Code = '0'))

/*  Записи с неуказанными объектами капитального строительства*/
declare TmpCapFunds cursor for
	select p.RefParametr, d.Note
	from [DV].[f_Fin_CapFunds] p join [DV].[f_F_ParameterDoc] d on (p.RefParametr = d.ID) 
	where (d.RefSost = 7) and (p.Name = '')
	
/*  Записи с неуказанными объектами приобретаемого недвижимого имущества*/
declare TmpRealAssFunds cursor for
	select p.RefParametr, d.Note
    from [DV].[f_Fin_realAssFunds] p join [DV].[f_F_ParameterDoc] d on (p.RefParametr = d.ID) 
    where (d.RefSost = 7) and (p.Name = '')

/*  Пустые ГЗ завершенные*/
declare TmpGosZadanie cursor for
select d.ID, d.Note
from  [DV].[f_F_ParameterDoc] d
where (d.RefSost = 7) and (d.RefPartDoc = 2) and (not exists(select ID
						   from [DV].[f_F_GosZadanie]
						   where RefParametr = d.ID))

/*  ГЗ завершенные с незаполненными ПНР*/
declare TmpGosZadaniePNR cursor for
	select p.RefParametr, d.Note
    from [DV].[f_F_GosZadanie] p join [DV].[f_F_ParameterDoc] d on (p.RefParametr = d.ID) 
    where (d.RefSost = 7) and (not exists(select ID
						   from [DV].[f_F_PNRZnach]
						   where RefFactGZ = p.ID))

/*  Пустые сметы завершенные*/		
declare TmpSmeta cursor for				   
select d.ID, d.Note
from  [DV].[f_F_ParameterDoc] d
where (d.RefSost = 7) and (d.RefPartDoc = 5) and (not exists(select ID
						   from [DV].[f_Fin_Smeta]
						   where RefParametr = d.ID))
begin
/* ================================ Собираем примечания для плана===================================*/		
	open TmpPFHD
	fetch next from TmpPFHD into @Doc, @Note;
	while @@FETCH_STATUS <> -1 begin
		
		update [DV].[f_F_ParameterDoc]
		set  Note = @Note + ' Не заполнен ПФХД;'
		where ID = @Doc
		
		fetch next from TmpPFHD into @Doc, @Note
	end
	close TmpPFHD;
		
	open TmpGrantFunds
	fetch next from TmpGrantFunds into @Doc, @Note;
	while @@FETCH_STATUS <> -1 begin
		
		update [DV].[f_F_ParameterDoc]
		set  Note = @Note + ' Не заполнено наименование субсидии;'
		where ID = @Doc
		
		fetch next from TmpGrantFunds into @Doc, @Note
	end
	close TmpGrantFunds;
	
	open TmpCapFunds
	fetch next from TmpCapFunds into @Doc, @Note;
	while @@FETCH_STATUS <> -1 begin
		
		update [DV].[f_F_ParameterDoc]
		set  Note = @Note + ' Не заполнено наименование объекта капитального строительства;'
		where ID = @Doc
		
		fetch next from TmpCapFunds into @Doc, @Note
	end
	close TmpCapFunds;
	
	open TmpRealAssFunds
	fetch next from TmpRealAssFunds into @Doc, @Note;
	while @@FETCH_STATUS <> -1 begin
		
		update [DV].[f_F_ParameterDoc]
		set  Note = @Note + ' Не заполнено наименование объекта приобретаемого недвиж. имущества;'
		where ID = @Doc
		
		fetch next from TmpRealAssFunds into @Doc, @Note
	end
	close TmpRealAssFunds;
	
/* ==================================== проставляем состояния для плана=============================*/		
	open TmpPFHD
	fetch next from TmpPFHD into @Doc, @Note;
	while @@FETCH_STATUS <> -1 begin
		
		update [DV].[f_F_ParameterDoc]
		set  RefSost = 5
		where ID = @Doc
		
		fetch next from TmpPFHD into @Doc, @Note
	end
	close TmpPFHD;
	deallocate TmpPFHD;
	
	open TmpGrantFunds
	fetch next from TmpGrantFunds into @Doc, @Note;
	while @@FETCH_STATUS <> -1 begin
		
		update [DV].[f_F_ParameterDoc]
		set  RefSost = 5
		where ID = @Doc
		
		fetch next from TmpGrantFunds into @Doc, @Note
	end
	close TmpGrantFunds;
	deallocate TmpGrantFunds;
	
	open TmpCapFunds
	fetch next from TmpCapFunds into @Doc, @Note;
	while @@FETCH_STATUS <> -1 begin
		
		update [DV].[f_F_ParameterDoc]
		set  RefSost = 5
		where ID = @Doc
		
		fetch next from TmpCapFunds into @Doc, @Note
	end
	close TmpCapFunds;
	deallocate TmpCapFunds;
	
	open TmpRealAssFunds
	fetch next from TmpRealAssFunds into @Doc, @Note;
	while @@FETCH_STATUS <> -1 begin
		
		update [DV].[f_F_ParameterDoc]
		set  RefSost = 5
		where ID = @Doc
		
		fetch next from TmpRealAssFunds into @Doc, @Note
	end
	close TmpRealAssFunds;
	deallocate TmpRealAssFunds;
	
/*========================================== ГЗ и Смета=============================================*/		
	
	open TmpGosZadanie
	fetch next from TmpGosZadanie into @Doc, @Note;
	while @@FETCH_STATUS <> -1 begin
		
		update [DV].[f_F_ParameterDoc]
		set  Note = @Note + ' Не заполнено государственное задание;',
		     RefSost = 5
		where ID = @Doc
		
		fetch next from TmpGosZadanie into @Doc, @Note
	end
	close TmpGosZadanie;
	deallocate TmpGosZadanie;
	
	open TmpGosZadaniePNR
	fetch next from TmpGosZadaniePNR into @Doc, @Note;
	while @@FETCH_STATUS <> -1 begin
		
		update [DV].[f_F_ParameterDoc]
		set  Note = @Note + ' Не заполнен ПНР;',
		     RefSost = 5
		where ID = @Doc
		
		fetch next from TmpGosZadaniePNR into @Doc, @Note
	end
	close TmpGosZadaniePNR;
	deallocate TmpGosZadaniePNR;
		
	open TmpSmeta
	fetch next from TmpSmeta into @Doc, @Note;
	while @@FETCH_STATUS <> -1 begin
		
		update [DV].[f_F_ParameterDoc]
		set  Note = @Note + ' Не заполнены Бюджетные обязательства учреждения;',
		     RefSost = 5
		where ID = @Doc
		
		fetch next from TmpSmeta into @Doc, @Note
	end
	close TmpSmeta;
	deallocate TmpSmeta;
end	
go

Exec Translation_States
go

drop procedure Translation_States
go