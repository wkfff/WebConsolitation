/* Start - #4570 - ����������� ����� ��������� ����� ���������� ��� ������ ���������� - shelpanov - 18.12.2012 */

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
           (16, 0, 'K', '������ ���������������� (��������������) ���������� (�. 0503730)')
           
INSERT INTO [DV].[d_Doc_TypeDoc]
			([ID]
			,[RowType]
			,[Code]
			,[Name])
     VALUES
           (17, 0, 'M', '����� � ���������� ����������� ������������ (�. 0503721)')
           
INSERT INTO [DV].[d_Doc_TypeDoc]
			([ID]
			,[RowType]
			,[Code]
			,[Name])
     VALUES
           (18, 0, 'N', '����� �� ���������� ����������� ����� ��� ���������-������������� ������������ (�. 0503737)')

INSERT INTO [DV].[d_Doc_TypeDoc]
           ([ID]
           ,[RowType]
           ,[Code]
           ,[Name])
     VALUES
           (19, 0, 'Q', '������ �������� �������������, �������������, ���������� ��������� �������, �������� ��������������, �������������� ���������� �������������� �������� �������, �������� ��������������, �������������� ������� ������� (�. 0503130)')
           
INSERT INTO [DV].[d_Doc_TypeDoc]
			([ID]
			,[RowType]
			,[Code]
			,[Name])
     VALUES
           (20, 0, 'R', '����� � ���������� ����������� ������������ (�. 0503121)')
           
INSERT INTO [DV].[d_Doc_TypeDoc]
			([ID]
			,[RowType]
			,[Code]
			,[Name])
     VALUES
           (21, 0, 'T', '����� �� ���������� ������� (�. 0503127)')

INSERT INTO [DV].[d_Doc_TypeDoc]
			([ID]
			,[RowType]
			,[Code]
			,[Name])
     VALUES
           (22, 0, 'U', '����� �� ���������� ���� ������� � �������� �� ���������� ����� ������������ (�. 0503137)')

print N'��������� ��������'
GO

enable trigger t_d_Doc_TypeDoc_aa on [DV].[d_Doc_TypeDoc];
 
GO

/* End - #4570 - ����������� ����� ��������� ����� ���������� ��� ������ ���������� - shelpanov - 18.12.2012 */