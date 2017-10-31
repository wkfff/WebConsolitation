/********************************************************************
    ��������� ���� Oracle �� ������ 3.0 � ��������� ������ 3.x 
********************************************************************/

/* Start - ����������� ����� */
/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */

/* Start - #17838 - ���������� ������ ���������� "�������������� �������" - vorontsov - 01.09.2011 */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (240, 'RIA.EO12InvestProjects', '�������������� �������', 'Krista.FM.RIA.Extensions.EO12InvestProjects.InvestProjectsExtensionInstaller, Krista.FM.RIA.Extensions.EO12InvestProjects');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (102, '3.0.0.7', To_Date('01.09.2011', 'dd.mm.yyyy'), SYSDATE, '��������������� web-��������� ������������ "�������������� �������"', 0);

commit;

/* End - #17838 - ���������� ������ ���������� "�������������� �������" - vorontsov - 01.09.2011 */
