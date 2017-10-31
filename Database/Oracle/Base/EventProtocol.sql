/*
	��� "������ � ������������"
	������	3.1
	������
		EventProtocol.sql - ���������� ���������������� �������.
	����	Oracle 9.2
*/

pro ================================================================================
pro ���������� ���������������� �������.
pro ================================================================================

/* ����������� */
create table Notifications
(
	ID					number (10) not null,			/* PK */
	RefUsers			number (10) not null,			/* ������������ �������� ������������� ����������� */
	EventDateTime		DATE default SYSDATE not null,	/* ���� � ����� ������ ����������� */
	Headline			varchar2 (255) not null,		/* ���� ����������� */
	IsReaded			number (1) default 0 not null,	/* ������� ������������� */
	IsArchive			number (1) default 0 not null,	/* ������� ���������� (�������� �����������)*/
	NotifyBody			CLOB,							/* XML �������� ����������� */
	constraint PKNotifications primary key ( ID ),
	constraint FKNotificationsRefUsers foreign key ( RefUsers )
		references Users ( ID )
);

/* ���� ������� */
/*	������� � ������������� ������� �������. �� �������� �������������� � ��������,
	����������� ��� ������� ������ ��� ��� ���������� ���� ������ */
create table KindsOfEvents
(
	ID					number (10) not null,
	ClassOfEvent		number (5) not null,		/* ����� �������:
														0 - �������������� ���������,
														1 - ��������� ���������,
														2 - ��������������,
														3 - ��������� �� ������,
														4 - ��������� � ��������� ������
														5 - ��������� �� �������� ���������� ��������
														6 - ��������� � ���������� �������� c ������� */
	Name				varchar2 (255) not null,	/* ����������� ���� ������� */
	constraint PKKindsOfEvents primary key ( ID )
);

/* ���� ������� ��� ��������� ������� ������ (1xx)*/
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (101, 0, '������ �������� �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (102, 0, '���������� � ��������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (103, 2, '��������������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (104, 5, '�������� ��������� �������� �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (105, 6, '��������� �������� ������� � �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (106, 3, '������ � �������� �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (107, 4, '����������� ������ � �������� �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (108, 0, '������ ������� �����');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (109, 5, '���������� ������� ����� � �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (110, 6, '�������� ���������� ������� �����');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (111, 0, '������ ��������� ��������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (112, 5, '���������� ��������� ��������� ������ c �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (113, 6, '�������� ���������� ��������� ��������� ������');

/* ���� ������� ��� ��������� ������������� ������ (2xx) */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (201, 0, '������ �������� �������������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (202, 0, '���������� � ��������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (203, 2, '��������������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (204, 5, '�������� ��������� �������� �������������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (205, 6, '��������� �������� ������������� � �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (206, 3, '������ � �������� �������������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (207, 4, '����������� ������ � �������� �������������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (211, 0, '������ ��������� ��������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (212, 5, '���������� ��������� ��������� ������ c �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (213, 6, '�������� ���������� ��������� ��������� ������');

/* ���� ������� ��� ���������� ��������� ����������� �������� (3xx) */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (301, 0, '������ ��������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (302, 0, '���������� � ��������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (303, 2, '��������������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (304, 5, '�������� ��������� ��������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (305, 6, '��������� �������� � �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (306, 3, '������ � �������� ����������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (307, 4, '����������� ������ � �������� ����������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (308, 0, '���������� �� ������');

/* ���� ������� ��� ���������� �������� ������������� (4xx) */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (401, 1, '������������ ����������� � �����');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (402, 1, '������������ ���������� �� �����');

/* ���� ��������� ������� (5xx) */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (502, 0, '����������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (503, 2, '��������������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (506, 3, '������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (507, 4, '����������� ������');

/* ���� ������� �� ������ ������ (6xx) */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (601, 0, '������ �������� ������ ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (602, 0, '���������� � ��������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (603, 2, '��������������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (604, 5, '�������� ��������� �������� ������ ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (605, 6, '��������� �������� ������ ������ � �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (606, 3, '������ � �������� ������ ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (607, 4, '����������� ������ � �������� ������ ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (611, 0, '������ ��������� ��������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (612, 5, '���������� ��������� ��������� ������ c �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (613, 6, '�������� ���������� ��������� ��������� ������');

/* ���� ������� ��� ��������� ��������� ������ (7xx)*/
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (701, 0, '������ �������� ��������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (702, 0, '���������� � ��������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (703, 2, '��������������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (704, 5, '�������� ��������� �������� ��������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (705, 6, '��������� �������� ��������� ������ � �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (706, 3, '������ � �������� ��������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (707, 4, '����������� ������ � �������� ��������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (711, 0, '������ ��������� ��������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (712, 5, '���������� ��������� ��������� ������ c �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (713, 6, '�������� ���������� ��������� ��������� ������');

/* ���� ������� ��� ��������� �������� ������ (8xx) */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (801, 0, '������ �������� �������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (802, 0, '���������� � �������� �������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (803, 2, '��������������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (804, 5, '�������� ��������� �������� �������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (805, 6, '��������� �������� �������� ������ � �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (806, 3, '������ � �������� �������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (807, 4, '����������� ������ � �������� �������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (811, 0, '������ ��������� ��������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (812, 5, '���������� ��������� ��������� ������ c �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (813, 6, '�������� ���������� ��������� ��������� ������');

insert into KindsOfEvents (ID, ClassOfEvent, Name) values (901, 0, '������ �������� �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (902, 0, '���������� � ��������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (903, 2, '��������������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (904, 5, '�������� ��������� �������� �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (905, 6, '��������� �������� ������� � �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (906, 3, '������ � �������� �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (907, 4, '����������� ������ � �������� �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (908, 0, '������ ������� �����');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (909, 5, '���������� ������� ����� � �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (910, 6, '�������� ���������� ������� �����');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (911, 0, '������ ��������� ��������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (912, 5, '���������� ��������� ��������� ������ c �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (913, 6, '�������� ���������� ��������� ��������� ������');

/* ���� ������� ��� ��������� "��������������" */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1001, 0, '�������������� ���������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1002, 2, '��������������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1003, 3, '������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1004, 4, '����������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1005, 0, '������ ��������� �������� ��������������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1006, 5, '�������� ���������� ��������� ��������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1007, 6, '���������� ��������� �������� � �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1008, 0, '�������� ��������������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1009, 0, '�������������� ������ � �������������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1010, 0, '������������ ������������� �������������� �� �������������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1011, 5, '�������� ���������� ��������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1012, 0, '������� ����� ��������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1013, 0, '������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1014, 0, '������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1015, 0, '������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1016, 0, '���������� ��������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1017, 0, '�������� ��������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1018, 0, '�������� ��������� ������' );

insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40100, 0, '������������ ����� ������');

/* ���������������� �������� ���������� �����������������  */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40301, 0, '������� ������ �������������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40302, 0, '������� ������ �����');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40303, 0, '������� ���������� �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40304, 0, '������� ���������� �����������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40305, 0, '������� ���������� ����� �����');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40306, 0, '�������� �������� � �������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40307, 0, '������� ������ ����������� ��������');

/* ���������������� �������� ���������� �����������������  */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40401, 0, '������� ����� ��������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40402, 0, '������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40403, 0, '������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40404, 0, '������� ������');

/* ���������������� �������� ������������ */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40501, 0, '���������� �����');

/* ���������������� �������������� ������ */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40666, 0, '�������������� ������');

/* ������������� ���������� */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40701, 5, '������������� ����������');

/* ���� ������� ��� ��������� ���������� ��������� ����� (50xxx) */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (50000, 0, '���������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (50001, 0, '������ ���������� �����');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (50002, 5, '���������� ������� ���������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (50003, 6, '���������� ��������� � �������');

/* ���� ������� ���������� ��� ������� �������� ���� � ����� ��� */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1101, 0, '�������� ��������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1102, 0, '������� �������������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1103, 0, '������ �������������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1104, 0, '���������� �� ������ ���������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1105, 0, '���������� �� ������ ����');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1106, 2, '��������������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1107, 3, '������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1108, 1, '������ ������ ������� �������� �� ����� ���');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1109, 1, '��������� ������ ������� �������� �� ����� ���');

-- ���������
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1201, 0, '�������� ������ ��������� �� ��������������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1202, 0, '�������� ���������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1203, 0, '������� ������������ ���������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1204, 0, '�������� ������ ��������� �� ���������� ������� �����');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1210, 0, '�������� ������ ���������(������)');

/* ����� ������� ��� ���� ����� ���������� */
create table HUB_EventProtocol
(
	ID					number (10) not null,		/* PK */
	EventDateTime		DATE default SYSDATE not null, /* ����� ������� */
	Module				varchar2 (255) not null,	/* ��� ������ �� ������� ������ ������� */
	RefKindsOfEvents	number (10) not null,		/* ��� ������� */
	InfoMessage			varchar2 (4000),			/* �������������� ��������� � �������. �������� �������������� ���������� */
	RefUsersOperations  number (10),				/* ������ �� ������ ��������� "�������� �������������" */
	ClassOfProtocol		number (4) not null,		/* ����� ���������. ������������� ������ "�������� ������ �������":
														1 - ������� ������
														7 - ��������� ������
														2 - ������������� ���������������
														3 - ������ ����������� ����
														4 - ������ ������
														5 - �������� �������������
														6 - ��������� ���������
														7 - ��������� ������
														8 - �������� ��������� */
	SessionID varchar2 (24),						/* ������������� ������ */
	constraint PKEventProtocol primary key ( ID ),
	constraint FKEventProtocolRefKindsOfEvent foreign key ( RefKindsOfEvents )
		references KindsOfEvents ( ID ) on delete cascade
);

create sequence g_HUB_EventProtocol;

/* ������� ������ */
create table SAT_DataPump
(
	ID					number (10) not null,		/* PK */
/* ����� ���� ���� ������������� ��� ������� ���� ��������� */
	PumpHistoryID		number (10) not null,		/* �������� �������. ������ �� PumpHistory.ID */
	DataSourceID		number (10),				/* ��������. ������ �� DataSource.ID */
	constraint PKSAT_DataPump primary key ( ID ),
	constraint FKSAT_DataPump foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* ������������� �� �������� "������� ������" ��� �������, ������� � �������� ������� */
create or replace view DataPumpProtocol
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.PumpHistoryID, SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_DataPump SAT on (HUB.ID = SAT.ID);

create or replace trigger t_DataPumpProtocol_i instead of insert on DataPumpProtocol
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- �������� �������� ����� �� ����������
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- ���� ����� ������� �� �������, �� ������������� � �������
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- ��������� ������ � ������ ���������� � �������� ��������
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 1, :new.SessionID);

	-- ��������� ������ �� �������������� ���������� � ���������� ��� ���������
	insert into SAT_DataPump (ID, PumpHistoryID, DataSourceID)
	values (NewID, :new.PumpHistoryID, :new.DataSourceID);
end t_DataPumpProtocol_i;
/

create or replace trigger t_DataPumpProtocol_u instead of update on DataPumpProtocol
begin
	raise_application_error(-20001, '������ ��������� �� ����� ���� ��������.');
end t_DataPumpProtocol_u;
/

create or replace trigger t_DataPumpProtocol_d instead of delete on DataPumpProtocol
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_DataPumpProtocol_d;
/

/* ������������� ��������������� */
create table SAT_BridgeOperations
(
	ID					number (10) not null,		/* PK */
	BridgeRoleA			varchar2 (100) not null,	/* ������������� �������������� ������ (��������������) */
	BridgeRoleB			varchar2 (100) not null,	/* ������������� ������������� �������������� (������������) */
	DataSourceID		number (10),				/* �������� ������ */
	PumpHistoryID		number (10),				/* �������� ������� */
	constraint PKSAT_BridgeOperations primary key ( ID ),
	constraint FKSAT_BridgeOperations foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* ������������� �� �������� "������������� ���������������" ��� �������, ������� � �������� ������� */
create or replace view BridgeOperations
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	BridgeRoleA, BridgeRoleB, PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.Sessionid,
	SAT.BridgeRoleA, SAT.BridgeRoleB, SAT.PumpHistoryID, SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_BridgeOperations SAT on (HUB.ID = SAT.ID);

create or replace trigger t_BridgeOperations_i instead of insert on BridgeOperations
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- �������� �������� ����� �� ����������
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- ���� ����� ������� �� �������, �� ������������� � �������
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- ��������� ������ � ������ ���������� � �������� ��������
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 2, :new.SessionID);

	-- ��������� ������ �� �������������� ���������� � ���������� ��� ���������
	insert into SAT_BridgeOperations (ID, BridgeRoleA, BridgeRoleB, PumpHistoryID, DataSourceID)
	values (NewID, :new.BridgeRoleA, :new.BridgeRoleB, :new.PumpHistoryID, :new.DataSourceID);
end t_BridgeOperations_i;
/

create or replace trigger t_BridgeOperations_u instead of update on BridgeOperations
begin
	raise_application_error(-20001, '������ ��������� �� ����� ���� ��������.');
end t_BridgeOperations_u;
/

create or replace trigger t_BridgeOperations_d instead of delete on BridgeOperations
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_BridgeOperations_d;
/

/* ������ ����������� ���� */
create table SAT_MDProcessing
(
	ID					number (10) not null,		/* PK */
	MDObjectName		varchar2 (255) not null,	/* ��� ��������������� ������� (��� ��� ���������) */
	ObjectIdentifier 	varchar2 (255),				/* ������������� ������� */
	OlapObjectType 		number (10),				/* ��� ������������ ������� */
	BatchId				varchar2 (132),				/* ������������� ������ */
	constraint PKSAT_MDProcessing primary key ( ID ),
	constraint FKSAT_MDProcessing foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* ������������� �� �������� "������ ����������� ����" ��� �������, ������� � �������� ������� */
create or replace view v_MDProcessing
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	MDObjectName, ObjectIdentifier, OlapObjectType, BatchId
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.MDObjectName, SAT.ObjectIdentifier,
	CASE SAT.OlapObjectType
		WHEN 0 THEN '�����'
		WHEN 1 THEN '���'
		WHEN 2 THEN '������ ���'
		WHEN 3 THEN '������ ����'
		WHEN 4 THEN '���������'
	END,
	SAT.BatchId
from HUB_EventProtocol HUB join SAT_MDProcessing SAT on (HUB.ID = SAT.ID);

create or replace view MDProcessing
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	MDObjectName, ObjectIdentifier, OlapObjectType, BatchId
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.MDObjectName, SAT.ObjectIdentifier, SAT.OlapObjectType, SAT.BatchId
from HUB_EventProtocol HUB join SAT_MDProcessing SAT on (HUB.ID = SAT.ID);

create or replace trigger t_MDProcessing_i instead of insert on MDProcessing
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- �������� �������� ����� �� ����������
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- ���� ����� ������� �� �������, �� ������������� � �������
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- ��������� ������ � ������ ���������� � �������� ��������
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 3, :new.SessionID);

	-- ��������� ������ �� �������������� ���������� � ���������� ��� ���������
	insert into SAT_MDProcessing (ID, MDObjectName, ObjectIdentifier, OlapObjectType, BatchId)
	values (NewID, :new.MDObjectName, :new.ObjectIdentifier, :new.OlapObjectType, :new.BatchId);
end t_MDProcessing_i;
/

create or replace trigger t_MDProcessing_u instead of update on MDProcessing
begin
	raise_application_error(-20001, '������ ��������� �� ����� ���� ��������.');
end t_MDProcessing_u;
/

create or replace trigger t_MDProcessing_d instead of delete on MDProcessing
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_MDProcessing_d;
/


/* �������� ������������� */
create table SAT_UsersOperations
(
	ID					number (10) not null,		/* PK */
	UserName			varchar2 (255) not null,	/* ��� ������������ (����� �� ��� � � Users.Name) */
	ObjectName			varchar2 (255) not null,	/* ��� ������� ������� (Objects.Name) */
	ActionName			varchar2 (255),				/* ������������(��������) ����������� �������� */
	UserHost			varchar2 (255),				/* ��� ������ � ������� ����������� ������������ */
	constraint PKSAT_UsersOperations primary key ( ID ),
	constraint FKSAT_UsersOperations foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* ������������� �� �������� "�������� �������������" ��� �������, ������� � �������� ������� */
create or replace view UsersOperations
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, SessionID,
	UserName, ObjectName, ActionName, UserHost
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.SessionID,
	SAT.UserName, SAT.ObjectName, SAT.ActionName, SAT.USERHOST
from HUB_EventProtocol HUB join SAT_UsersOperations SAT on (HUB.ID = SAT.ID);

create or replace trigger t_UsersOperations_i instead of insert on UsersOperations
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	NewID := :new.ID;
	-- ���� �� �� ���� �������� - �������� �������� �� ����������
	if NewID is null then select g_HUB_EventProtocol.NextVal into NewID from Dual; end if;

	-- ���� ����� ������� �� �������, �� ������������� � �������
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- ��������� ������ � ������ ���������� � �������� ��������
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, 5, :new.SessionID);

	-- ��������� ������ �� �������������� ���������� � ���������� ��� ���������
	insert into SAT_UsersOperations (ID, UserName, ObjectName, ActionName, UserHost)
	values (NewID, :new.UserName, :new.ObjectName, :new.ActionName, :new.UserHost);
end t_UsersOperations_i;
/

create or replace trigger t_UsersOperations_u instead of update on UsersOperations
begin
	raise_application_error(-20001, '������ ��������� �� ����� ���� ��������.');
end t_UsersOperations_u;
/

create or replace trigger t_UsersOperations_d instead of delete on UsersOperations
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_UsersOperations_d;
/


/* ��������� ��������� */
create table SAT_SystemEvents
(
	ID					number (10) not null,		/* PK */
	ObjectName			varchar2 (255) not null,	/* ��� ������� ������� (Objects.Name) */
	constraint PKSAT_SystemEvents primary key ( ID ),
	constraint FKSAT_SystemEvents foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* ������������� �� �������� "��������� ���������" ��� �������, ������� � �������� ������� */
create or replace view SystemEvents
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, SessionID,
	ObjectName
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.SessionID,
	SAT.ObjectName
from HUB_EventProtocol HUB join SAT_SystemEvents SAT on (HUB.ID = SAT.ID);

create or replace trigger t_SystemEvents_i instead of insert on SystemEvents
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- �������� �������� ����� �� ����������
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- ���� ����� ������� �� �������, �� ������������� � �������
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- ��������� ������ � ������ ���������� � �������� ��������
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, 6, :new.SessionID);

	-- ��������� ������ �� �������������� ���������� � ���������� ��� ���������
	insert into SAT_SystemEvents (ID, ObjectName)
	values (NewID, :new.ObjectName);
end t_SystemEvents_i;
/

create or replace trigger t_SystemEvents_u instead of update on SystemEvents
begin
	raise_application_error(-20001, '������ ��������� �� ����� ���� ��������.');
end t_SystemEvents_u;
/

create or replace trigger t_SystemEvents_d instead of delete on SystemEvents
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_SystemEvents_d;
/

/* �������� ��� ������ ������ */
create table SAT_ReviseData
(
	ID					number (10) not null,		/* PK */
	DataSourceID		number (10),				/* �������� ������ */
	PumpHistoryID		number (10),				/* �������� ������� */
/* ����� ���� ���� ������������� ��� ������� ���� ��������� */
	constraint PKSAT_ReviseData primary key ( ID ),
	constraint FKSAT_ReviseData foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* ������������� �� �������� "������ ������" ��� �������, ������� � �������� ������� */
create or replace view ReviseDataProtocol
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.PumpHistoryID, SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_ReviseData SAT on (HUB.ID = SAT.ID);

create or replace trigger t_ReviseDataProtocol_i instead of insert on ReviseDataProtocol
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- �������� �������� ����� �� ����������
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- ���� ����� ������� �� �������, �� ������������� � �������
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- ��������� ������ � ������ ���������� � �������� ��������
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 4, :new.SessionID);

	-- ��������� ������ �� �������������� ���������� � ���������� ��� ���������
	insert into SAT_ReviseData (ID, PumpHistoryID, DataSourceID)
	values (NewID, :new.PumpHistoryID, :new.DataSourceID);
end t_ReviseDataProtocol_i;
/

create or replace trigger t_ReviseDataProtocol_u instead of update on ReviseDataProtocol
begin
	raise_application_error(-20001, '������ ��������� �� ����� ���� ��������.');
end t_ReviseData_u;
/

create or replace trigger t_ReviseDataProtocol_d instead of delete on ReviseDataProtocol
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_ReviseData_d;
/

/* ��������� ������ */
create table SAT_ProcessData
(
	ID					number (10) not null,		/* PK */
/* ����� ���� ���� ������������� ��� ������� ���� ��������� */
	PumpHistoryID		number (10) not null,		/* �������� �������. ������ �� PumpHistory.ID */
	DataSourceID		number (10),				/* ��������. ������ �� DataSource.ID */
	constraint PKSAT_ProcessData primary key ( ID ),
	constraint FKSAT_ProcessData foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* ������������� �� �������� "��������� ������" ��� �������, ������� � �������� ������� */
create or replace view ProcessDataProtocol
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.PumpHistoryID, SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_ProcessData SAT on (HUB.ID = SAT.ID);

create or replace trigger t_ProcessDataProtocol_i instead of insert on ProcessDataProtocol
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- �������� �������� ����� �� ����������
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- ���� ����� ������� �� �������, �� ������������� � �������
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- ��������� ������ � ������ ���������� � �������� ��������
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 7, :new.SessionID);

	-- ��������� ������ �� �������������� ���������� � ���������� ��� ���������
	insert into SAT_ProcessData (ID, PumpHistoryID, DataSourceID)
	values (NewID, :new.PumpHistoryID, :new.DataSourceID);
end t_ProcessDataProtocol_i;
/

create or replace trigger t_ProcessDataProtocol_u instead of update on ProcessDataProtocol
begin
	raise_application_error(-20001, '������ ��������� �� ����� ���� ��������.');
end t_ProcessDataProtocol_u;
/

create or replace trigger t_ProcessDataProtocol_d instead of delete on ProcessDataProtocol
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_ProcessDataProtocol_d;
/

/* �������� ������ */
create table SAT_DeleteData
(
	ID					number (10) not null,		/* PK */
/* ����� ���� ���� ������������� ��� ������� ���� ��������� */
	PumpHistoryID		number (10),				/* �������� �������. ������ �� PumpHistory.ID */
	DataSourceID		number (10),				/* ��������. ������ �� DataSource.ID */
	constraint PKSAT_DeleteData primary key ( ID ),
	constraint FKSAT_DeleteData foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* ������������� �� �������� "��������� ������" ��� �������, ������� � �������� ������� */
create or replace view DeleteDataProtocol
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.PumpHistoryID, SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_DeleteData SAT on (HUB.ID = SAT.ID);

create or replace trigger t_DeleteDataProtocol_i instead of insert on DeleteDataProtocol
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- �������� �������� ����� �� ����������
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- ���� ����� ������� �� �������, �� ������������� � �������
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- ��������� ������ � ������ ���������� � �������� ��������
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 8, :new.SessionID);

	-- ��������� ������ �� �������������� ���������� � ���������� ��� ���������
	insert into SAT_DeleteData (ID, PumpHistoryID, DataSourceID)
	values (NewID, :new.PumpHistoryID, :new.DataSourceID);
end t_DeleteDataProtocol_i;
/

create or replace trigger t_DeleteDataProtocol_u instead of update on DeleteDataProtocol
begin
	raise_application_error(-20001, '������ ��������� �� ����� ���� ��������.');
end t_DeleteDataProtocol_u;
/

create or replace trigger t_DeleteDataProtocol_d instead of delete on DeleteDataProtocol
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_DeleteDataProtocol_d;
/


/* ������������ ������ */
create table SAT_PreviewData
(
	ID					number (10) not null,		/* PK */
/* ����� ���� ���� ������������� ��� ������� ���� ��������� */
	PumpHistoryID		number (10) not null,		/* �������� �������. ������ �� PumpHistory.ID */
	DataSourceID		number (10),				/* ��������. ������ �� DataSource.ID */
	constraint PKSAT_DataPreview primary key ( ID ),
	constraint FKSAT_DataPreview foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* ������������� �� �������� "������� ������" ��� �������, ������� � �������� ������� */
create or replace view PreviewDataProtocol
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.PumpHistoryID, SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_PreviewData SAT on (HUB.ID = SAT.ID);

create or replace trigger t_PreviewDataProtocol_i instead of insert on PreviewDataProtocol
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- �������� �������� ����� �� ����������
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- ���� ����� ������� �� �������, �� ������������� � �������
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- ��������� ������ � ������ ���������� � �������� ��������
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 1, :new.SessionID);

	-- ��������� ������ �� �������������� ���������� � ���������� ��� ���������
	insert into SAT_PreviewData (ID, PumpHistoryID, DataSourceID)
	values (NewID, :new.PumpHistoryID, :new.DataSourceID);
end t_PreviewDataProtocol_i;
/

create or replace trigger t_PreviewDataProtocol_u instead of update on PreviewDataProtocol
begin
	raise_application_error(-20001, '������ ��������� �� ����� ���� ��������.');
end t_PreviewDataProtocol_u;
/

create or replace trigger t_PreviewDataProtocol_d instead of delete on PreviewDataProtocol
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_PreviewDataProtocol_d;
/


/* �������� "��������������" */
create table SAT_ClassifiersOperations
(
	ID					number (10) not null,		/* PK */
/* ����� ���� ���� ������������� ��� ������� ���� ��������� */
	Classifier		varchar2(255),					/* �������������, � ������� ����������� �������� */
	PumpHistoryID		number (10) not null,		/* �������� �������. ������ �� PumpHistory.ID */
	DataSourceID		number (10),				/* ��������. ������ �� DataSource.ID */
	ObjectType			number (10),				/* ��� ������� �� */
	constraint PKSAT_ClassifiersOperations primary key ( ID ),
	constraint FKSAT_ClassifiersOperations foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* ������������� �� �������� "��������������" ��� �������, ������� � �������� ������� */
create or replace view ClassifiersOperations
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	Classifier, PumpHistoryID, DataSourceID, ObjectType
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.Classifier, SAT.PumpHistoryID, SAT.DataSourceID, SAT.ObjectType
from HUB_EventProtocol HUB join SAT_ClassifiersOperations SAT on (HUB.ID = SAT.ID);

create or replace trigger t_ClassifiersOperations_i instead of insert on ClassifiersOperations
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- �������� �������� ����� �� ����������
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- ���� ����� ������� �� �������, �� ������������� � �������
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- ��������� ������ � ������ ���������� � �������� ��������
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 2, :new.SessionID);

	-- ��������� ������ �� �������������� ���������� � ���������� ��� ���������
	insert into SAT_ClassifiersOperations (ID, Classifier, PumpHistoryID, DataSourceID, ObjectType)
	values (NewID, :new.Classifier, :new.PumpHistoryID, :new.DataSourceID, :new.ObjectType);
end t_ClassifiersOperations_i;
/

create or replace trigger t_ClassifiersOperations_u instead of update on ClassifiersOperations
begin
	raise_application_error(-20001, '������ ��������� �� ����� ���� ��������.');
end t_ClassifiersOperations_u;
/

create or replace trigger t_ClassifiersOperations_d instead of delete on ClassifiersOperations
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_ClassifiersOperations_d;
/

/* ���������� ����� */
create table SAT_UpdateScheme
(
	ID					number (10) not null,		/* PK */
/* ����� ���� ���� ������������� ��� ������� ���� ��������� */
	ObjectFullName		varchar2 (64) not null,		/* ���������� ������������ �������. */
	ObjectFullCaption	varchar2 (255),				/* ������� ������������ �������. */
	ModificationType	number (1),					/* ��� �����������:
														0 - �������� ������ �������,
														1 - ��������� ���������,
														2 - �������� ������������� �������. */
	constraint PKSAT_UpdateScheme primary key ( ID ),
	constraint FKSAT_UpdateScheme foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* ������������� �� �������� "���������� �����" ��� �������, ������� � �������� ������� */
create or replace view UpdateSchemeProtocol
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	ModificationType, ObjectFullName, ObjectFullCaption
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.ModificationType, SAT.ObjectFullName, SAT.ObjectFullCaption
from HUB_EventProtocol HUB join SAT_UpdateScheme SAT on (HUB.ID = SAT.ID);

create or replace trigger t_UpdateSchemeProtocol_i instead of insert on UpdateSchemeProtocol
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- �������� �������� ����� �� ����������
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- ���� ����� ������� �� �������, �� ������������� � �������
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- ��������� ������ � ������ ���������� � �������� ��������
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 12, :new.SessionID);

	-- ��������� ������ �� �������������� ���������� � ���������� ��� ���������
	insert into SAT_UpdateScheme (ID, ModificationType, ObjectFullName, ObjectFullCaption)
	values (NewID, :new.ModificationType, :new.ObjectFullName, :new.ObjectFullCaption);
end t_UpdateSchemeProtocol_i;
/

create or replace trigger t_UpdateSchemeProtocol_u instead of update on UpdateSchemeProtocol
begin
	raise_application_error(-20001, '������ ��������� �� ����� ���� ��������.');
end t_UpdateSchemeProtocol_u;
/

create or replace trigger t_UpdateSchemeProtocol_d instead of delete on UpdateSchemeProtocol
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_UpdateSchemeProtocol_d;
/


/* �������� �������� ��� ����������� */
create table SAT_DataSourcesOperations
(
	ID					number (10) not null,
	DataSourceID		number (10) not null,
	constraint PKSAT_DataSourcesOperations primary key ( ID ),
	constraint FKSAT_DataSourcesOperations foreign key ( ID )
		references HUB_EventProtocol (ID) on delete cascade
);

/* � ������������� ��� ��� */
create or replace view DataSourcesOperations (ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, DataSourceID)
as
  select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.Sessionid,
	SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_DataSourcesOperations SAT on (HUB.ID = SAT.ID);

CREATE OR REPLACE TRIGGER T_DATASOURCESOPERATIONS_I instead of insert on DataSourcesOperations
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 13, :new.SessionID);

	insert into  SAT_DataSourcesOperations (ID, DataSourceID)
	values (NewID, :new.DataSourceID);
end T_DATASOURCESOPERATIONS_I;
/

create or replace trigger t_DataSourcesOperations_u instead of update on DataSourcesOperations
begin
	raise_application_error(-20001, '������ ��������� �� ����� ���� ��������.');
end t_DataSourcesOperations_u;
/

create or replace trigger t_DataSourcesOperations_d instead of delete on DataSourcesOperations
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_DataSourcesOperations_d;
/

/* �������� �������� ��� �������� �������� ���� �� ����� ��� */
create table SAT_NewYearOperations
(
	ID					number (10) not null,
	DataSourceID		number (10) not null,
	constraint PKSAT_NewYearOperations primary key ( ID ),
	constraint FKSAT_NewYearOperations foreign key ( ID )
		references HUB_EventProtocol (ID) on delete cascade
);

/* � ������������� ��� ��� */
create or replace view NewYearOperations (ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, DataSourceID)
as
  select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.Sessionid,
	SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_NewYearOperations SAT on (HUB.ID = SAT.ID);

CREATE OR REPLACE TRIGGER T_NewYearOperations_I instead of insert on NewYearOperations
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 13, :new.SessionID);

	insert into  SAT_NewYearOperations (ID, DataSourceID)
	values (NewID, :new.DataSourceID);
end T_NewYearOperations_I;
/

create or replace trigger t_NewYearOperations_u instead of update on NewYearOperations
begin
	raise_application_error(-20001, '������ ��������� �� ����� ���� ��������.');
end t_NewYearOperations_u;
/

create or replace trigger t_NewYearOperations_d instead of delete on NewYearOperations
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_NewYearOperations_d;
/

/* ��������� ��� ��������� */
create table SAT_MessageExchangeOperations
(
  ID          number (10) not null,
  constraint PKSAT_MessageOperations primary key ( ID ),
  constraint FKSAT_MessageOperations foreign key ( ID )
    references HUB_EventProtocol (ID) on delete cascade
);

/* � ������������� ��� ��� */
create or replace view MessageExchangeOperations (ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID)
as
  select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.Sessionid
from HUB_EventProtocol HUB join SAT_MessageExchangeOperations SAT on (HUB.ID = SAT.ID);

CREATE OR REPLACE TRIGGER T_MessageExchangeOperations_I instead of insert on MessageExchangeOperations
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 13, :new.SessionID);

	insert into  SAT_MessageExchangeOperations (ID)
	values (NewID);
end T_MessageExchangeOperations_I;
/

create or replace trigger t_MessageExchangeOperations_u instead of update on MessageExchangeOperations
begin
	raise_application_error(-20001, '������ ��������� �� ����� ���� ��������.');
end t_MessageExchangeOperations_u;
/

create or replace trigger t_MessageExchangeOperations_d instead of delete on MessageExchangeOperations
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_MessageExchangeOperations_d;
/

commit;

commit work;
