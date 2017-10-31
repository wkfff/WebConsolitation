/*******************************************************************
 ��������� ���� Oracle �� ������ 2.5 � ��������� ������ 2.6
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */


/* Start - ����������� ����� */

/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */

/* Start - - �������� ���������� ������������� ������� ������������� �� ��������� - gbelov - 17.12.2008 */

/* ���������� ������������� ������� ������������� �� ��������� */
alter table MetaLinks
	add DefaultAssociateRule varchar2 (255);

/* ������������� ������� �� ��������� � �� �������, ������� ������� � xml-������������ */	
update MetaLinks set DefaultAssociateRule = 
	SYS.dbms_lob.substr(
		SYS.dbms_lob.substr(Configuration, 100, SYS.dbms_lob.instr(Configuration, 'associateRuleDefault', 1, 1) + 22),
		SYS.dbms_lob.instr(SYS.dbms_lob.substr(Configuration, 100, SYS.dbms_lob.instr(Configuration, 'associateRuleDefault', 1, 1) + 22), '">', 1, 1) - 1,
		1
	)
where Class = 2 and DefaultAssociateRule is null and Configuration like '%associateRuleDefault%';

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (61, '2.5.0.5', To_Date('17.12.2008', 'dd.mm.yyyy'), SYSDATE, '�������� ���������� ������������� ������� ������������� �� ���������.', 0);

commit;

/* Start - - �������� ���������� ������������� ������� ������������� �� ��������� - gbelov - 17.12.2008 */
