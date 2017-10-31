/********************************************************************
	��������� ���� SQL Server �� ������ 3.0 � ��������� ������ 3.X 
********************************************************************/

/* Start - ����������� ����� */
/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */

/* Start - - ���������� ������ ���������� "���������� ����� ������" - nikonenko - 03.12.2011 */

/* ����������� ���������� ������������ "���������� ����� ������" */
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (260, 'RIA.MinSport', '���������� ����� ������', 'Krista.FM.RIA.Extensions.MinSport.ExtensionInstaller, Krista.FM.RIA.Extensions.MinSport');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (112, '3.0.0.16', To_Date('03.12.2011', 'dd.mm.yyyy'), SYSDATE, '��������������� ��������� ������������ "���������� ����� ������"', 0);

commit;

whenever SQLError exit rollback;

/* End - - ���������� ������ ���������� "���������� ����� ������" - nikonenko - 03.12.2011 */
