/*******************************************************************
 ��������� ���� Sql Server 2005 �� ������ 2.3.1 � ��������� ������ 2.X.X
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */


/* Start -  - �������� ������ ��� �������� �������� ������� - chubov - 19.12.2008 */


insert into RegisteredUIModules (ID, Name, FullName, Description) values (150, 'ForecastUI', 'Krista.FM.Client.ViewObjects.ForecastUI.ForecastNavigation, Krista.FM.Client.ViewObjects.ForecastUI', '������� �������� �������');

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (62, '2.5.0.6', CONVERT(datetime, '2008.12.19', 102), GETDATE(), '�������� ������ ��� �������� �������� �������.', 0);

go

/* End   -  - �������� ������ ��� �������� �������� ������� - chubov - 19.12.2008 */
