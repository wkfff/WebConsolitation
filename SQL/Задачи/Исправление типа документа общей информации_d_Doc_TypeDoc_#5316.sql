/* Start - #5316 - ����������� ���� ��������� ����� ���������� - shelpanov - 15.01.2013 */

print N'���������� ��������'
GO

disable trigger t_d_Doc_TypeDoc_aa on [DV].[d_Doc_TypeDoc];

print N'���������....'
GO

update [DV].[d_Doc_TypeDoc]
set Name = '����� � ����������� ������������ � �� ������������� ���������'
where ID = 11;

print N'��������� ��������'
GO

enable trigger t_d_Doc_TypeDoc_aa on [DV].[d_Doc_TypeDoc];
 
GO

/* End - #5316 - ����������� ���� ��������� ����� ���������� - shelpanov - 15.01.2013 */