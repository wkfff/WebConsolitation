/********************************************************************
	��������� ���� SQL Server �� ������ 3.0 � ��������� ������ 3.x
********************************************************************/

/* Start - #15909 - ���������� ������ ���������� "������ �������" - paluh - 14.04.2011 */

/* ����������� ���������� ������������ "������ �������" */
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (210, 'ReportsUI', '������ �������', 'Krista.FM.Client.ViewObjects.ReportsUI.Gui.ReportsNavigation, Krista.FM.Client.ViewObjects.ReportsUI');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (106, '3.0.0.1', CONVERT(datetime, '2011.04.14', 102), GETDATE(), '��������������� ��������� ������������ "������ �������"', 0);

go

/* End - #15909 - ���������� ������ ���������� "������ �������" - paluh - 14.04.2011 */