/*******************************************************************
 ��������� ���� SqlServer �� ������ 2.5 � ��������� ������ 2.6
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */


/* Start - ����������� ����� */

/* ������� �� ����� ������ */
/* End   - ����������� ����� */

/* Start - - �������� ���������� ������������� ������� ������������� �� ��������� - gbelov - 17.12.2008 */

/* ���������� ������������� ������� ������������� �� ��������� */
alter table MetaLinks
	add DefaultAssociateRule varchar (255);
	
go

/* ������������� ������� �� ��������� � �� �������, ������� ������� � xml-������������ */	
update MetaLinks set DefaultAssociateRule = 
	substring(
		substring(Configuration, charindex('associateRuleDefault', Configuration) + 22, 100),
		1,
		charindex(
			'">', 
			substring(Configuration, charindex('associateRuleDefault', Configuration) + 22,
			100)) - 1
	)
where Class = 2 and DefaultAssociateRule is null and Configuration like '%associateRuleDefault%';

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (61, '2.5.0.5', CONVERT(datetime, '2008.12.17', 102), GETDATE(), '�������� ���������� ������������� ������� ������������� �� ���������.', 0);

go

/* Start - - �������� ���������� ������������� ������� ������������� �� ��������� - gbelov - 17.12.2008 */
