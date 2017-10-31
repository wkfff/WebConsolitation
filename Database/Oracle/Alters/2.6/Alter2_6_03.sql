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

/* Start -  - ������� ��� ���������� ���� ������� � ���������� ��������������� �������� ������� - Chubov - 06.10.2009 */

whenever SQLError continue commit;

insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) 
values ('Forecast_Scenario', 'Forecast_Scenario', '��������', '�������� ��� �������� �������� �������', 25000);

insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) 
values ('Forecast_Valuation', 'Forecast_Valuation', '������� �������', '������� ������� ��� �������� �������� �������', 25000);

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (74, '2.6.0.3', To_Date('06.10.2009', 'dd.mm.yyyy'), SYSDATE, '������� ��� ���������� ���� ������� � ���������� ���������������', 0);

commit;

whenever SQLError exit rollback;

/* End - ������� ��� ���������� ���� ������� � ���������� ��������������� �������� ������� - Chubov - 06.10.2009 */
