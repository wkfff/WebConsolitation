
UPDATE [DV].[f_F_NPACena]
   SET DataNPAGZ = CONVERT(datetime, '2999.12.31', 102)
      
 WHERE (DataNPAGZ is Null)
GO

UPDATE [DV].[f_Fin_othGrantFunds]
   SET RefOtherGrant = (select ID from [DV].[d_Fin_OtherGant] where Code = '0')
      
 WHERE (RefOtherGrant is Null)
GO

