/*******************************************************************
 ��������� ���� Sql Server 2005 �� ������ 2.3.1 � ��������� ������ 2.X.X
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */


/* Start - 6748 - ���������� ���� DV.OlapObjects.ProcessResult �� 2000 �������� - gbelov - 20.02.2008 */

alter table OlapObjects
	alter column ProcessResult varchar (2000);

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (39, '2.4.0.5', CONVERT(datetime, '2008.02.20', 102), GETDATE(), '���������� ���� DV.OlapObjects.ProcessResult �� 2000 ��������', 0);

go

/* End   - 6748 - ���������� ���� DV.OlapObjects.ProcessResult �� 2000 �������� - gbelov - 20.02.2008 */
