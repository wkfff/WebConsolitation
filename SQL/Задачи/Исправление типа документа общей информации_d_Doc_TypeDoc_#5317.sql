/* Start - #5317 - ����������� ���� ��������� ����� ���������� - shelpanov - 25.01.2013 */

print N'���������� ��������'
GO

disable trigger t_d_Doc_TypeDoc_aa on [DV].[d_Doc_TypeDoc];

print N'���������....'
GO

update [DV].[d_Doc_TypeDoc]
set Name = '������ �������� �������������, �������������, ���������� ��������� ������� (�. 0503130)'
where ID = 19;

print N'��������� ��������'
GO

enable trigger t_d_Doc_TypeDoc_aa on [DV].[d_Doc_TypeDoc];
 
GO

/* End - #5317 - ����������� ���� ��������� ����� ���������� - shelpanov - 25.01.2013 */