/********************************************************************
	Переводит базу SQL Server из версии 3.0 в следующую версию 3.X 
********************************************************************/

/* Start - Добавление признака текущей версии классификатора - tsvetkov - 15.06.2011 */

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[DV].[FKVersionsRefSource]') AND parent_object_id = OBJECT_ID(N'[DV].[ObjectVersions]'))
ALTER TABLE [DV].[ObjectVersions] DROP CONSTRAINT [FKVersionsRefSource]
GO

print N'Добавляем в таблицу ObjectVersions поле-признак текущей версии классификатора.'

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'ObjectVersions' AND COLUMN_NAME = 'IsCurrent')
BEGIN
   print N'Поля нет...Добавляем...'
   ALTER TABLE ObjectVersions ADD IsCurrent int DEFAULT 0 WITH VALUES;
   execute sp_executesql N'ALTER trigger t_ObjectVersions_BI on ObjectVersions instead of insert as
		begin
		declare @nullsCount int;
		set nocount on;
		select @nullsCount = count(*) from inserted where ID is null;
		if @nullsCount = 0 insert into ObjectVersions (ID, SOURCEID, OBJECTKEY, PRESENTATIONKEY, NAME, ISCURRENT)
			select ID, SOURCEID, OBJECTKEY, PRESENTATIONKEY, NAME, ISCURRENT from inserted;
		else if @nullsCount = 1
		begin
			insert into g.ObjectVersions default values;
			delete from g.ObjectVersions where ID = @@IDENTITY;
			insert into ObjectVersions (ID, SOURCEID, OBJECTKEY, PRESENTATIONKEY, NAME,ISCURRENT)
				select @@IDENTITY, SOURCEID, OBJECTKEY, PRESENTATIONKEY, NAME, ISCURRENT from inserted;
		end else
			RAISERROR (500001, 1, 1);
	end;'
	  
	print N'Поле добавлено.' 
END
GO

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (98, '3.0.0.3', CONVERT(datetime, '2011.06.15', 102), GETDATE(), 'Добавление признака текущей версии классификатора', 0);

go

/* End - Добавление признака текущей версии классификатора - tsvetkov - 15.06.2011 */
