/* Start - #4319 - ����������� 3� ����� ��������� ����� ���������� ��� ������ ���������� - shelpanov - 17.12.2012 */

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
           (12, 0, 'G', '�������� ��� ������, ��������������� ���������� ������������ �����������')
           
INSERT INTO [DV].[d_Doc_TypeDoc]
			([ID]
			,[RowType]
			,[Code]
			,[Name])
     VALUES
           (14, 0, 'H', '��� � ����������� ������������ �����������')
           
INSERT INTO [DV].[d_Doc_TypeDoc]
			([ID]
			,[RowType]
			,[Code]
			,[Name])
     VALUES
           (15, 0, 'J', '��� � �������� ����������� ����������� ����������� � ����� � �����������')

print N'��������� ��������'
GO

enable trigger t_d_Doc_TypeDoc_aa on [DV].[d_Doc_TypeDoc];
 
GO

/* End - #4319 - ����������� 3� ����� ��������� ����� ���������� ��� ������ ���������� - shelpanov - 17.12.2012 */