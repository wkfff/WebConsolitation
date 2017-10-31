/********************************************************************
	��������� ���� SQL Server �� ������ 3.0 � ��������� ������ 3.X 
********************************************************************/

/* Start - #17327 - ���������� ������ ���������� "������ ������������� �����" - barhonina - 07.12.2011 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (262, 'RIA.FO41', '������ ������������� �����', 'Krista.FM.RIA.Extensions.FO41.FO41ExtensionInstaller, Krista.FM.RIA.Extensions.FO41');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (114, '3.0.0.18', CONVERT(datetime, '2011.12.07', 102), GETDATE(), '��������������� web-��������� ������������ "������ ������������� �����"', 0);

go

/* End - #17327 - ���������� ������ ���������� "������ ������������� �����" - barhonina - 07.12.2011 */
