/*
	��� "������ � ������������"
	������	3.1
	������
		Audit.sql - ����� ��������� ������.
	����	Oracle 9.2
*/

pro ================================================================================
pro ����� ��������� ������
pro ================================================================================

connect DVAudit/DVAudit@&DatabaseName;

/* �������� �� ��������� ������ */
create table DataOperations
(
	ID number(10) not null ,
	ChangeTime date default SYSDATE not null,
	KindOfOperation number(1) not null,			/* ��������: 0 = insert, 1 = update, 2 = delete */
	ObjectName varchar2 (31) not null,			/* ������ �� */
	ObjectType number (10) default 0 not null,	/* ��� ������� ��. ������������ Krista.FM.ServerLibrary.DataOperationsObjectTypes */
	UserName varchar2 (64) default USER not null,/* ������������ */
	SessionID varchar2 (24) not null,			/* ID ������ */
	RecordID number(10) not null,				/* ID ������ ������� */
	TaskID number(10),							/* ID ������ (������ ���� � �������� ������; ��� � ���������� ������� ������ � ��������� �������) */
	PumpID number(10),							/* ID ������� (����������� ������ ��� �������) */
	constraint PKDataOperationsID primary key ( ID )
);

create sequence g_DataOperations;

create or replace trigger t_DataOperations_bi before insert on DataOperations for each row
begin
	if :new.ID is null then select g_DataOperations.NextVal into :new.ID from Dual; end if;
end t_DataOperations_bi;
/

connect &UserName/&UserPassword@&DatabaseName;

grant ALL PRIVILEGES on DVAudit.Dataoperations to PUBLIC;

/* ���������� �������� ��� �������� ���������� ������� ������ */
create or replace context DVContext using DVContext accessed globally;

/* ����� ��� ������� � ��������� ���������� ������� ������ */
create or replace package DVContext as
   procedure SetValue(attribute varchar2, value varchar2, username varchar2, client_id varchar2);
end;
/

create or replace package body DVContext as
	/* ��������� �������� ��������� ������ */
	procedure SetValue(attribute varchar2, value varchar2, username varchar2, client_id varchar2) as
	begin
		DBMS_SESSION.SET_CONTEXT ('DVContext', attribute, value, username, client_id);
	end;
end;
/

