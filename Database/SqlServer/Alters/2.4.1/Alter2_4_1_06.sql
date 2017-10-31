/*******************************************************************
 ��������� ���� Sql Server 2005 �� ������ 2.3.1 � ��������� ������ 2.X.X
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */


/* Start - 9239 - ����������� ��� web ����������. �������� ��� ������� web ���������. - avolkov - 08.10.2008 */

insert into g.Objects default values;
insert into objects (ID, name, caption, objecttype, objectkey) 
values (@@IDENTITY, 'WebReports', 'Web-���������', 20000, 'WebReports');

go

/* End - 9239 - ����������� ��� web ����������. �������� ��� ������� web ���������. - avolkov - 08.10.2008 */


/* Start -  - ��������� ������� ��� �������� �������� web parts. - gbelov - 08.10.2008 */

CREATE TABLE Personalization 
(
	UserName        varchar (255),
	Application     varchar (645),
	pagesettings    varchar (max)
	constraint UK_Pers UNIQUE (UserName, Application)
);

/* End - - ��������� ������� ��� �������� �������� web parts. - gbelov - 08.10.2008  */

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (54, '2.4.1.6', CONVERT(datetime, '2008.10.08', 102), GETDATE(), '�������� ��� ������� web ���������.', 0);

go

