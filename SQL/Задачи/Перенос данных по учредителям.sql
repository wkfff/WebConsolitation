/* Start - #4785 - ������� ������ �� ����������� - shelpanov - 19.11.2012 */

print N'���������� ��������'
GO

disable trigger t_f_F_Founder_aa on [DV].[f_F_Founder];

print N'���������� �����������'
GO

create procedure Replace_Data as
DECLARE  @RowID  int 
DECLARE  @RefYchred  int 

/*  ������ � ������ � ��*/
declare Pasports cursor for
SELECT [ID], [RefYchred]
FROM [DV].[f_Org_Passport]
where [RefYchred] is not null

begin
    open Pasports
	fetch next from Pasports into @RowID, @RefYchred;
	while @@FETCH_STATUS <> -1 begin
		
	  INSERT INTO [DV].[f_F_Founder]
				   ([SourceID]
				   ,[TaskID]
				   ,[RefPassport]
				   ,[RefYchred])
			 VALUES
				   (0, 0 , @RowID, @RefYchred)

	  fetch next from Pasports into @RowID, @RefYchred
	end
	close Pasports;
	deallocate Pasports;
end	
go

Exec Replace_Data
go

drop procedure Replace_Data
go

print N'��������� ��������'
GO

enable trigger t_f_F_Founder_aa on [DV].[f_F_Founder];
 
GO

/* End - #4785 - ������� ������ �� ����������� - shelpanov - 19.11.2012 */