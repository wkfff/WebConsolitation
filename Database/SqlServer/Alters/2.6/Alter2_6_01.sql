/*******************************************************************
  ��������� ���� Sql Server 2005 �� ������ 2.6 � ��������� ������ 2.X.X
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */

/* Start - FMQ00011203 - �������� ����� - Paluh - 24.08.2009 */

/* ����������� ���������� ������������ "�������� �����" */
insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (170, 'FinSourceDebtorBookUI', '�������� �����', 'Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.DebtBookNavigation, Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI');

go

/* ���������� ������ � ������� ������������� �� ������������� "������.������" */
alter table Users Add RefRegion int;

go


insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (72, '2.6.0.1', CONVERT(datetime, '2009.08.24', 102), GETDATE(), '�������� �����', 0);

go

/* End - FMQ00011203 - ��������� � ��������� �������� ������� ����������� - Paluh - 24.08.2009 */
