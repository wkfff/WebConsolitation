/*
	��� "������ � ������������"
	������	2.6
	������
		������ ��� ���������� ������� �� ����	SQL Server 2005
*/

:Error Error.txt

SELECT @@VERSION As '������ �������'
SELECT @@SERVERNAME AS '��� �������'
GO

print('����������� � ��')

:Connect $(SERVER_NAME) -U $(UserName) -P $(Password)

USE $(DatabaseName)
GO


print('������ ���������� ��������...')

:r Add_AuditDB.sql

print('���������� ���������� ��������')

GO
