/* Start - #4352 - ������� ��� ���������(fx.FX.PartDoc) ID = 4 - shelpanov - 03.10.2012 */

print N'���������� ��������'
GO

disable trigger t_f_F_ParameterDoc_aa on [DV].[f_F_ParameterDoc];

print N'�������� ���������� � ����� 4'
GO

delete from [DV].[f_F_ParameterDoc]
where RefPartDoc = 4;

 
print N'��������� ��������'
GO

enable trigger t_f_F_ParameterDoc_aa on [DV].[f_F_ParameterDoc];
 
GO

