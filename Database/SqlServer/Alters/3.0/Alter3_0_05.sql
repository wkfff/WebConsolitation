/*******************************************************************
 ��������� ���� SQL�� ������ 3.0 � ��������� ������ 3.X.X
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */

/* Start - ������.������ - ������� ��� ���� - zaharchenko - 30.08.2011 */

alter table fx_Date_YearDayUNV alter column DateMonthID int not null; 

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (100, '3.0.0.5', CONVERT(datetime, '2011.08.30', 102), GETDATE(), '������.������ - �������� ��������� ID ��� �����', 0);

go

/* End - ������.������ - ������� ��� ���� - zaharchenko - 30.08.2011 */=======