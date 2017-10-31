/********************************************************************
	��������� ���� SQL Server �� ������ 3.0 � ��������� ������ 3.X 
********************************************************************/

/* Start - ����������� ����� */
/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */

/* Start - - ���������� ������ ���������� "������ ������������� �����" - barhonina - 22.07.2011 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (230, 'RIA.FO41', '������ ������������� �����', 'Krista.FM.RIA.Extensions.FO41.FO41ExtensionInstaller, Krista.FM.RIA.Extensions.FO41');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (99, '3.0.0.4', To_Date('22.07.2011', 'dd.mm.yyyy'), SYSDATE, '��������������� web-��������� ������������ "������ ������������� �����"', 0);

commit;

whenever SQLError exit rollback;

/* End - - ���������� ������ ���������� "������ ������������� �����" - barhonina - 22.07.2011 */
