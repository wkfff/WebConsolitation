/* Start - #6235 - ��������� �������� � ���������� ������� - shelpanov - 02.07.2013 */

print N'���������� ��������'
GO

disable trigger [DV].[t_f_ResultWork_FinNFinAsset_aa] on [DV].[f_ResultWork_FinNFinAssets] ;

print N'��������....'
GO

UPDATE [DV].[f_ResultWork_FinNFinAssets]
   SET [Value] = null
      ,[RefTypeIxm] = 3
 where Value = 0
GO


print N'��������� ��������'
GO

enable trigger [DV].[t_f_ResultWork_FinNFinAsset_aa] on [DV].[f_ResultWork_FinNFinAssets];
 
GO

/* End - #6235 - ��������� �������� � ���������� ������� - shelpanov - 02.07.2013 */




