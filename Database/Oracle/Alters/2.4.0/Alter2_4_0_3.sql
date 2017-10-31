/*******************************************************************
 ��������� ���� Oracle �� ������ 2.X.X � ��������� ������ 2.X.X
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */


/* Start - ����������� ����� */

/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */


/* Start - 1176 - ��������� �� ������� ���� - ��������� �� ������ - avolkov - 06.02.2008 */

whenever SQLError continue commit;

/* ��������� � ������� ���� - ������� ����������.*/
alter table HUB_DataSources
   add locked number(1) default 0 not null;

/* ��������� ��� �� ���� � �������������. */
create or replace view datasources
(id, suppliercode, datacode, dataname, kindsofparams, name, year, month, variant, quarter, territory, locked, datasourcename)
as
select
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory, Locked,
	SupplierCode ||' '|| DataName ||
		CASE KindsOfParams
			WHEN 8 THEN ''
			ELSE ' - ' ||
				CASE KindsOfParams
					WHEN 0 THEN Name || ' ' || Year
					WHEN 1 THEN cast(Year as varchar(4))
					WHEN 2 THEN Year || ' ' || Month
					WHEN 3 THEN Year || ' ' || Month || ' ' || Variant
					WHEN 4 THEN Year || ' ' || Variant
					WHEN 5 THEN Year || ' ' || Quarter
					WHEN 6 THEN Year || ' ' || Territory
					WHEN 7 THEN Year || ' ' || Quarter || ' ' || Month
					WHEN 9 THEN Variant
				END
		END
from HUB_DataSources;

commit;

/* ������� ������� ��� ��������� �������� ��� �����������. */
create table SAT_DataSourcesOperations
(
	ID					number (10) not null,
	DataSourceID		number (10) not null,
	constraint PKSAT_DataSourcesOperations primary key ( ID ),
	constraint FKSAT_DataSourcesOperations foreign key ( ID )
		references HUB_EventProtocol (ID) on delete cascade
);

commit;

/* � ������������� ��� ��� */
create or replace view DataSourcesOperations (ID, EventDateTime, module, kindsOfEvents, infoMessage, refUsersOperations, sessionID, datasourceID)
as
  select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.Sessionid,
	SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_DataSourcesOperations SAT on (HUB.ID = SAT.ID);

commit;

/* ������� �������� ��� �������������*/
CREATE OR REPLACE TRIGGER T_DATASOURCESOPERATIONS_D instead of delete on DataSourcesOperations
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end T_DATASOURCESOPERATIONS_D;
/

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

CREATE OR REPLACE TRIGGER T_DATASOURCESOPERATIONS_U instead of update on DataSourcesOperations
begin
	raise_application_error(-20001, '������ ��������� �� ����� ���� ��������.');
end T_DATASOURCESOPERATIONS_U;
/

/* ��������� ���� ������� ���������� � �������� ��������� */
insert into kindsofevents (ID, ClassOfEvent, Name) values (1016, 0, '���������� ��������� ������' );

commit;

insert into kindsofevents (ID, ClassOfEvent, Name) values (1017, 0, '�������� ��������� ������' );

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (37, '2.4.0.3', To_Date('06.02.2008', 'dd.mm.yyyy'), SYSDATE, '���������� ���� - �������� ���������� ��������� � ������� HUB_DataSources � ������������� DataSources; ���������� ������� ���������� ���������.', 0);

commit;

whenever SQLError exit rollback;

/* End 1176 - ��������� �� ������� ���� - ��������� �� ������ - avolkov - 06.02.2008 */


