/*******************************************************************
 ��������� ���� Sql Server 2005 �� ������ 2.3.1 � ��������� ������ 2.X.X
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */

/* Start - 8052 - ����� ������������ �������� ������� - Feanor - 04.04.2008 */

alter table PumpHistory add UserName varchar (255);
alter table PumpHistory add UserHost varchar (255);
alter table PumpHistory add SessionID varchar (24);

go 

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (44, '2.4.0.10', CONVERT(datetime, '2008.04.04', 102), GETDATE(), '���� � ������������ � PumpHistory', 0);

go

/* End - 8052 - ����� ������������ �������� ������� - Feanor - 04.04.2008 */
