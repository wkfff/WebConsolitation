/********************************************************************
    ��������� ���� Oracle �� ������ 3.0 � ��������� ������ 3.x 
********************************************************************/

/* Start - ����������� ����� */
/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */

/* Start - #14339 - ��������� ��������� ��� ������� �������� ���� � ����� ��� - tsvetkov - 24.10.2011 */

insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1101, 0, '�������� ��������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1102, 0, '������� �������������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1103, 0, '������ �������������� ������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1104, 0, '���������� �� ������ ���������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1105, 0, '���������� �� ������ ����');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1106, 2, '��������������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1107, 3, '������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1108, 1, '������ ������ ������� �������� �� ����� ���');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1109, 1, '��������� ������ ������� �������� �� ����� ���');

commit;

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

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (107, '3.0.0.11', To_Date('07.11.2011', 'dd.mm.yyyy'), SYSDATE, '��������� ��������� ��� ������� �������� ���� � ����� ���', 0);

commit;

/* End - #14339 - ��������� ��������� ��� ������� �������� ���� � ����� ��� - tsvetkov - 24.10.2011 */
