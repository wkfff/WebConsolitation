/*******************************************************************
 ��������� ���� Sql Server 2005 �� ������ 2.3.1 � ��������� ������ 2.X.X
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */


/* Start -  - �������� ������ ��� ������������ ���������� �������������� - gbelov - 16.07.2008 */

insert into templates (NAME, TYPE, PARENTID) values ('������� �������', 0, NULL);
insert into templates (NAME, TYPE, PARENTID) values ('��������� ��������������', 0, @@IDENTITY)

go

insert into RegisteredUIModules (ID, Name, FullName, Description) values (140, 'FinSourcePlanningUI', 'Krista.FM.Client.ViewObjects.FinSourcePlanningUI.FinSourcePlanningNavigation, Krista.FM.Client.ViewObjects.FinSourcePlanningUI', '��������� ��������������');

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (58, '2.5.0.2', CONVERT(datetime, '2008.12.02', 102), GETDATE(), '�������� ������ ��� ������������ ���������� ��������������.', 0);

go

/* End   -  - �������� ������ ��� ������������ ���������� �������������� - gbelov - 16.07.2008 */
