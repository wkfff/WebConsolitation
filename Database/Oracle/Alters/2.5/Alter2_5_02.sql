/*******************************************************************
 ��������� ���� Oracle �� ������ 2.4.1 � ��������� ������ 2.X.X
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */


/* Start - ����������� ����� */

/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */

/* Start -  - �������� ������ ��� ������������ ���������� �������������� - gbelov - 16.07.2008 */

alter trigger T_TEMPLATES_AA disable;

/* ����������  �������� �������� �������*/
declare
	NewID pls_integer;
begin
	select g_Templates.NextVal into NewID from Dual;

	insert into templates (ID, NAME, TYPE, PARENTID) 
		values (NewID , '������� �������', 0, null);
	insert into templates (NAME, TYPE, PARENTID) 
		values ('��������� ��������������', 0, NewID);
end;
/

commit;

alter trigger T_TEMPLATES_AA enable;

insert into RegisteredUIModules (ID, Name, FullName, Description) values (140, 'FinSourcePlanningUI', 'Krista.FM.Client.ViewObjects.FinSourcePlanningUI.FinSourcePlanningNavigation, Krista.FM.Client.ViewObjects.FinSourcePlanningUI', '��������� ��������������');

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (58, '2.5.0.2', To_Date('12.08.2008', 'dd.mm.yyyy'), SYSDATE, '�������� ������ ��� ������������ ���������� ��������������.', 0);

commit;

whenever SQLError exit rollback;

/* End   -  - �������� ������ ��� ������������ ���������� �������������� - gbelov - 16.07.2008 */
