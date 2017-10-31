/*******************************************************************
 ��������� ���� Oracle �� ������ 2.5 � ��������� ������ 2.6
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */

/* Start - ����������� ����� */


/* Start -  - �������� ������ "�������������� � �������" ������ ��� �������  ������ � ���������������� - gbelov - 8.12.2008 */

delete from RegisteredUIModules where ID in (20, 30, 40, 50, 60);

insert into RegisteredUIModules (ID, Name, FullName, Description) values (150, 'EntityNavigationListUI', 'Krista.FM.Client.ViewObjects.AssociatedCLSUI.Navigation.EntityNavigationControl, Krista.FM.Client.ViewObjects.AssociatedCLSUI', '�������������� � �������');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (59, '2.5.0.3', CONVERT(datetime, '2008.12.08', 102), GETDATE(), '�������� ������ "�������������� � �������" ������ ��� �������  ������ � ����������������.', 0);

go

/* End   -  - �������� ������ "�������������� � �������" ������ ��� �������  ������ � ���������������� - gbelov - 8.12.2008 */
