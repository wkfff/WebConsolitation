/********************************************************************
    ��������� ���� SQL Server �� ������ 3.0 � ��������� ������ 3.x
********************************************************************/

/* Start - #17838 - ���������� ������ ���������� "�������������� �������" - vorontsov - 01.09.2011 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (240, 'RIA.EO12InvestProjects', '�������������� �������', 'Krista.FM.RIA.Extensions.EO12InvestProjects.InvestProjectsExtensionInstaller, Krista.FM.RIA.Extensions.EO12InvestProjects');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (102, '3.0.0.7', CONVERT(datetime, '2011.09.01', 102), GETDATE(), '��������������� web-��������� ������������ "�������������� �������"', 0);

go

/* End - #17838 - ���������� ������ ���������� "�������������� �������" - vorontsov - 01.09.2011 */

