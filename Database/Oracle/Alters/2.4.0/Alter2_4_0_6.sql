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

/* Start - 7699 - ������������� ����������  - blinkov - 27.02.2008 */

whenever SQLError continue commit;

insert into KindsOfEvents (ID, ClassOfEvent, Name) 
values (40701, 5, '������������� ����������');

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (40, '2.4.0.6', To_Date('27.02.2008', 'dd.mm.yyyy'), SYSDATE, '������������� ����������', 0);

commit;

whenever SQLError exit rollback;

/* End - 7699 - ������������� ����������  - blinkov - 27.02.2008 */

