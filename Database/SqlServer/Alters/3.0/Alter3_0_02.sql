/********************************************************************
	��������� ���� SQL Server �� ������ 3.0 � ��������� ������ 3.X 
********************************************************************/

/* Start - - ���������� ������ ���������� "���� ������" - gbelov - 25.05.2011 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (220, 'RIA.Consolidation', '���� ������', 'Krista.FM.RIA.Extensions.Consolidation.ConsolidationExtensionInstaller, Krista.FM.RIA.Extensions.Consolidation');

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (221, 'RIA.Consolidation.Admin', '������������� ����� ������', 'Krista.FM.RIA.Extensions.Consolidation.ConsolidationAdminExtensionInstaller, Krista.FM.RIA.Extensions.Consolidation');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (97, '3.0.0.2', CONVERT(datetime, '2011.05.25', 102), GETDATE(), '��������������� web-��������� ������������ "���� ������"', 0);

go

/* End - - ���������� ������ ���������� "���� ������" - gbelov - 25.05.2011 */
