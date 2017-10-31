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

/* Start - 9239 - ����������� ��� web ����������. �������� ��� ������� web ���������. - avolkov - 08.10.2008 */

insert into objects (name, caption, objecttype, objectkey) 
values ('WebReports', 'Web-���������', 20000, 'WebReports');

commit;

whenever SQLError exit rollback;

/* End - 9239 - ����������� ��� web ����������. �������� ��� ������� web ���������. - avolkov - 08.10.2008 */


/* Start -  - ��������� ������� ��� �������� �������� web parts. - gbelov - 08.10.2008 */

create table Personalization 
(
	UserName        varchar2 (255),
	Application     varchar2 (1000),
	PageSettings    blob default empty_blob(),
	constraint UK_Pers UNIQUE ( UserName, Application )
);

whenever SQLError exit rollback;

/* End - - ��������� ������� ��� �������� �������� web parts. - gbelov - 08.10.2008  */

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (54, '2.4.1.6', To_Date('08.10.2008', 'dd.mm.yyyy'), SYSDATE, '�������� ��� ������� web ���������.', 0);

commit;

