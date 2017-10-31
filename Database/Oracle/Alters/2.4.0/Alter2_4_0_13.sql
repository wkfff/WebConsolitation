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


/* Start 8569 - ��������� ������������� ������ � ��������� ������������� fx_Date_YearDayUNV - gbelov - 29.05.2008 */

insert into fx_Date_YearDayUNV (ID, ROWTYPE, DateYearID, DateYear, DateHalfYearID,          DateHalfYear, DateQuarterID,           DateQuarter, DateMonthID,             DateMonth, DateDayID,               DateDay, OrderByDefault,     Name, ParentID)
												values (-1,       0,          0, '�������� �� �������', 0, '�������� �� �������',             0, '�������� �� �������',           0, '�������� �� �������',         0, '�������� �� �������', 0, '�������� �� �������', null);

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (47, '2.4.0.13', To_Date('29.05.2008', 'dd.mm.yyyy'), SYSDATE, '��������� ������������� ������ � ��������� ������������� fx_Date_YearDayUNV.', 0);

commit;

/* End   8569 - ��������� ������������� ������ � ��������� ������������� fx_Date_YearDayUNV - gbelov - 29.05.2008 */


