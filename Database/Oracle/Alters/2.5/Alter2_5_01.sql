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

/* Start - 6821 - ��������� � ��������� �������� ������� ����������� - Paluh - 06.05.2008 */

whenever SQLError continue commit;

alter table Templates add DocumentFileName varchar2 (500);
alter table Templates drop column UIDTemplate;
alter table Templates drop column DocumentName;

commit;

alter trigger t_Templates_AA disable;

alter table templates add Editor number(10) default -1;

update templates set Editor = -1;

alter trigger t_Templates_AA enable;

commit;

insert into RegisteredUIModules (ID, Name, Description, FullName) 
values (130, 'TemplatesViewObj', '����������� ��������', 'Krista.FM.Client.ViewObjects.TemplatesUI.TemplatesNavigation, Krista.FM.Client.ViewObjects.TemplatesUI');

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (57, '2.5.0.1', To_Date('02.12.2008', 'dd.mm.yyyy'), SYSDATE, '��������� � ��������� �������� ������� �����������', 0);

commit;

whenever SQLError exit rollback;

/* End - 6821 - ��������� � ��������� �������� ������� ����������� - Paluh - 06.05.2008 */
