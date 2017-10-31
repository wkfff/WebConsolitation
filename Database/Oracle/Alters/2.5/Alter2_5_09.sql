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

/* Start - 10026 - ��������� � ��������� �������� ������� ����������� - gbelov - 24.02.2009 */

-- ��� ������
alter table Templates add Code varchar2 (100);
--  ���� ��� ����������
alter table Templates add SortIndex number (10);
--  ���� ��� ������� (1 - �������; 2 - ������; 4 - �����)
alter table Templates add Flags number (10) default 0;

-- ���� ��������
create table TemplatesTypes
(
	ID					number (10) not null,		/* PK */
	Name				varchar2 (500) not null,	/* �������� ���� ������� */
	Description			varchar2 (2048),			/* �������� ���� ������� */
	constraint PKTemplatesTypes primary key ( ID )
);

create sequence g_TemplatesTypes;

create or replace trigger t_TemplatesTypes_bi before insert on TemplatesTypes for each row
begin
	if :new.ID is null then select g_TemplatesTypes.NextVal into :new.ID from Dual; end if;
end t_TemplatesTypes_bi;
/

-- ��������� � ���������� ����� ��������� ��������� ������
insert into TemplatesTypes (ID, Name, Description) values (1, '������� �������', '������� ������� MDX ��������');
insert into TemplatesTypes (ID, Name, Description) values (2, '������ �������', '������ ������: ��������� ��������������');
insert into TemplatesTypes (ID, Name, Description) values (3, '���-������', '������ ���-����� � ��������');
insert into TemplatesTypes (ID, Name, Description) values (4, 'iPhone-������', '������ ��� ����������� �� iPhone');

-- ������������ � �������� ������� �������
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) values ('1_TemplateType', '1_TemplateType', '������� �������', '������� ������� MDX ��������', 22000);
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) values ('2_TemplateType', '2_TemplateType', '������ �������', '������ ������: ��������� ��������������', 22000);
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) values ('3_TemplateType', '3_TemplateType', '���-������', '������ ���-����� � ��������', 22000);
insert into Objects (ObjectKey, Name, Caption, Description, ObjectType) values ('4_TemplateType', '4_TemplateType', 'iPhone-������', '������ ��� ����������� �� iPhone', 22000);

commit;

-- ������ �� ���������� ����� ��������
alter table Templates add RefTemplatesTypes number (10);

alter trigger t_Templates_aa disable; 

update Templates set RefTemplatesTypes = 1;

alter trigger t_Templates_aa enable; 

alter table Templates modify RefTemplatesTypes number (10) not null;

alter table Templates 
	add	constraint FKTemplatesRefTemplatesTypes foreign key (RefTemplatesTypes)
		references TemplatesTypes (ID);

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (65, '2.5.0.9', To_Date('25.02.2009', 'dd.mm.yyyy'), SYSDATE, '��������� � ��������� �������� ������� �����������.', 0);

commit;

/* End - 10026 - ��������� � ��������� �������� ������� ����������� - gbelov - 24.02.2009 */
