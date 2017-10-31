/*******************************************************************
 ��������� ���� Oracle �� ������ 2.6 � ��������� ������ 2.X.X
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */


/* Start - ����������� ����� */

/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */

/* Start - FMQ00011203 - �������� ����� - Paluh - 24.08.2009 */

whenever SQLError continue commit;

/* ����������� ���������� ������������ "�������� �����" */

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (170, 'FinSourceDebtorBookUI', '�������� �����', 'Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.DebtBookNavigation, Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI');

commit;

/* ���������� ������ � ������� ������������� �� ������������� "������.������" */
alter table Users Add RefRegion number (10);

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (72, '2.6.0.1', To_Date('24.08.2009', 'dd.mm.yyyy'), SYSDATE, '�������� �����', 0);

commit;

whenever SQLError exit rollback;

/* End - FMQ00011203 - ��������� � ��������� �������� ������� ����������� - Paluh - 24.08.2009 */
