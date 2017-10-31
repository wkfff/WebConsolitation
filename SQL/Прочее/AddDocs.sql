create procedure Add_Docs as
DECLARE  @Instit  int 

/*  АУ и БУ */
declare TmpInst1 cursor for
  select ID
  from [DV].[d_Org_Structure]
  where  (RefTipYc = 3) or (RefTipYc = 10)

/*  Казенные */
declare TmpInst2 cursor for
  select ID
  from [DV].[d_Org_Structure]
  where  (RefTipYc = 8)
  
begin
	open TmpInst1
	fetch next from TmpInst1 into @Instit;
	while @@FETCH_STATUS <> -1 begin
		
		   /* Паспорта*/
		   if not exists(select ID
						 from [DV].[f_F_ParameterDoc]
						 where (RefUchr = @Instit) and (RefPartDoc = 1)
						)
		   begin
			INSERT INTO [DV].[f_F_ParameterDoc]
				   ([SourceID]
				   ,[TaskID]
				   ,[Note]
				   ,[PlanThreeYear]
				   ,[RefPartDoc]
				   ,[RefSost]
				   ,[RefUchr]
				   ,[RefYearForm])
			 VALUES
				   (0, 0 ,'' ,0 , 1, 2 ,@Instit, 2012)
		   end
		   
		   /* ГЗ*/
		   if not exists(select ID
						 from [DV].[f_F_ParameterDoc]
						 where (RefUchr = @Instit) and (RefPartDoc = 2)
						)
		   begin
			INSERT INTO [DV].[f_F_ParameterDoc]
				   ([SourceID]
				   ,[TaskID]
				   ,[Note]
				   ,[PlanThreeYear]
				   ,[RefPartDoc]
				   ,[RefSost]
				   ,[RefUchr]
				   ,[RefYearForm])
			 VALUES
				   (0, 0 ,'' ,0 , 2, 2 ,@Instit, 2012)
		   end          
		   
		   /* ПФХД*/
		   if not exists(select ID
						 from [DV].[f_F_ParameterDoc]
						 where (RefUchr = @Instit) and (RefPartDoc = 3)
						)
		   begin
			INSERT INTO [DV].[f_F_ParameterDoc]
				   ([SourceID]
				   ,[TaskID]
				   ,[Note]
				   ,[PlanThreeYear]
				   ,[RefPartDoc]
				   ,[RefSost]
				   ,[RefUchr]
				   ,[RefYearForm])
			 VALUES
				   (0, 0 ,'' ,0 , 3, 2 ,@Instit, 2012)
		   end          
		
		fetch next from TmpInst1 into @Instit
	end
	close TmpInst1;
	deallocate TmpInst1;
  
    open TmpInst2
	fetch next from TmpInst2 into @Instit
	while @@FETCH_STATUS <> -1 begin
	   /* Смета*/
	   if not exists(select ID
					 from [DV].[f_F_ParameterDoc]
					 where (RefUchr = @Instit) and (RefPartDoc = 5)
					)
	   begin
		INSERT INTO [DV].[f_F_ParameterDoc]
			   ([SourceID]
			   ,[TaskID]
			   ,[Note]
			   ,[PlanThreeYear]
			   ,[RefPartDoc]
			   ,[RefSost]
			   ,[RefUchr]
			   ,[RefYearForm])
		 VALUES
			   (0, 0 ,'' ,0 , 5, 2 ,@Instit, 2012)
	   end
			
	  fetch next from TmpInst2 into @Instit
	end
	close TmpInst2;
	deallocate TmpInst2;
end	
go

Exec Add_Docs
go

drop procedure Add_Docs
go