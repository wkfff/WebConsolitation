/*******************************************************************
 ��������� ���� Oracle �� ������ 2.X.X � ��������� ������ 2.X.X
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */

/* Start - ����������� ����� */

/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */

/* Start - 6748 - ���������� ���� DV.OlapObjects.ProcessResult �� 2000 �������� - gbelov - 20.02.2008 */

alter table OlapObjects
	modify ProcessResult varchar2 (2000);

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (39, '2.4.0.5', To_Date('20.02.2008', 'dd.mm.yyyy'), SYSDATE, '���������� ���� DV.OlapObjects.ProcessResult �� 2000 ��������', 0);

commit;

whenever SQLError exit rollback;

/* End   - 6748 - ���������� ���� DV.OlapObjects.ProcessResult �� 2000 �������� - gbelov - 20.02.2008 */
