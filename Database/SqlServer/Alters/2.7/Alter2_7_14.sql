/********************************************************************
	��������� ���� SQL Server �� ������ 2.7 � ��������� ������ 3.0 
********************************************************************/

/* Start - #16079 - ������ ���� - gbelov - 29.03.2011 */

/* ����������� ���������� ������������ "������ ����" */
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (190, 'RIA.MarksOMSU', '������ ����', 'Krista.FM.RIA.Extensions.MarksOMSU');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (93, '2.7.0.14', CONVERT(datetime, '2011.03.29', 102), GETDATE(), '��������������� ��������� ������������ "������ ����"', 0);

go

/* End - #16079 - ������ ���� - gbelov - 29.03.2011 */
