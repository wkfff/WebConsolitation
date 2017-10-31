/* Start - #6235 - Изменение значений в финансовых активах - shelpanov - 02.07.2013 */

print N'Отключение триггера'
GO

disable trigger [DV].[t_f_ResultWork_FinNFinAsset_aa] on [DV].[f_ResultWork_FinNFinAssets] ;

print N'Изменяем....'
GO

UPDATE [DV].[f_ResultWork_FinNFinAssets]
   SET [Value] = null
      ,[RefTypeIxm] = 3
 where Value = 0
GO


print N'Включение триггера'
GO

enable trigger [DV].[t_f_ResultWork_FinNFinAsset_aa] on [DV].[f_ResultWork_FinNFinAssets];
 
GO

/* End - #6235 - Изменение значений в финансовых активах - shelpanov - 02.07.2013 */




