
/*��������� ��������� �������� user � ������� ���� ������ � ������������ 
���������� SQL Server login. ��������� user � login ������ ���� �������. 
�������� password ������ ����� �������� NULL ��� �� ������ ���� ������.
*/

USE demo
GO
EXEC sp_change_users_login 'update_one', 'demo', 'MikhailMamonov'
GO