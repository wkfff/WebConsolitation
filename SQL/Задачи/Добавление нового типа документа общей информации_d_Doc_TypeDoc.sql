/* Start - #4318 - ����������� ������ ���� ��������� ����� ���������� ��� ������ ���������� - shelpanov - 14.12.2012 */

print N'���������� ��������'
GO

disable trigger t_d_Doc_TypeDoc_aa on [DV].[d_Doc_TypeDoc];

print N'���������....'
GO

INSERT INTO [DV].[d_Doc_TypeDoc]
           ([ID]
           ,[RowType]
           ,[Code]
           ,[Name])
     VALUES
           (11, 0, 'D', '���������� � ����������� ������������ � �� ������������� ���������')

print N'��������� ��������'
GO

enable trigger t_d_Doc_TypeDoc_aa on [DV].[d_Doc_TypeDoc];
 
GO

/* End - #4318 - ����������� ������ ���� ��������� ����� ���������� ��� ������ ���������� - shelpanov - 14.12.2012 */