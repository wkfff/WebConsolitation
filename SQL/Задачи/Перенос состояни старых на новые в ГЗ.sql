/* Start - #3825 - ��������� ������ ��������� ��� �� � ����� - shelpanov - 12.10.2012 */

print N'���������� ��������'
GO

disable trigger t_f_F_ParameterDoc_aa on [DV].[f_F_ParameterDoc];

print('���������: ������')
GO

UPDATE [DV].[f_F_ParameterDoc]
SET [RefStates] = (
      select ID
      from [DV].[d_State_States]
      where [Code] = 1)
WHERE [RefSost] = 2;
 
print('���������: �� ������������')
GO

UPDATE [DV].[f_F_ParameterDoc]
SET [RefStates] = (
      select ID
      from [DV].[d_State_States]
      where [Code] = 2)
WHERE [RefSost] = 4;

print('���������: �� ���������')
GO

UPDATE [DV].[f_F_ParameterDoc]
SET [RefStates] = (
      select ID
      from [DV].[d_State_States]
      where [Code] = 3)
WHERE [RefSost] = 5;

print('���������: ��������')
GO

UPDATE [DV].[f_F_ParameterDoc]
SET [RefStates] = (
      select ID
      from [DV].[d_State_States]
      where [Code] = 4)
WHERE [RefSost] = 7;

print('���������: ��������')
GO

UPDATE [DV].[f_F_ParameterDoc]
SET [RefStates] = (
      select ID
      from [DV].[d_State_States]
      where [Code] = 5)
WHERE [RefSost] = 8;

print N'��������� ��������'
GO

enable trigger t_f_F_ParameterDoc_aa on [DV].[f_F_ParameterDoc];

/* End - #3825 - ��������� ������ ��������� ��� �� � ����� - shelpanov - 12.10.2012 */