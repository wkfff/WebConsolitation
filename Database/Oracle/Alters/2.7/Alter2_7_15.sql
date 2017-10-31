/********************************************************************
	��������� ���� SQL Server �� ������ 2.7 � ��������� ������ 3.0 
********************************************************************/

/* Start - ����������� ����� */
/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */

/* Start - #15948 - ���� ������ � �� - gbelov - 29.03.2011 */

/* ����������� ���������� ������������ "���� ������ � ��" */
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (200, 'RIA.MOFOWebForms', '���� ������ � ��', 'Krista.FM.RIA.Extensions.MOFOWebForms');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (94, '2.7.0.15', To_Date('29.03.2011', 'dd.mm.yyyy'), SYSDATE, '��������������� ��������� ������������ "���� ������ � ��"', 0);

commit;

whenever SQLError exit rollback;

/* End - #15948 - ���� ������ � �� - gbelov - 29.03.2011 */
