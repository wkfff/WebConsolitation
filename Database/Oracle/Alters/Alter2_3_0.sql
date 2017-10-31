/*******************************************************************
 ��������� ���� Oracle �� ������ 2.3.0 � ��������� ������ 2.3.1
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */


/* Start - ����������� ����� */

/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */


/* Start - 5267 - ����� �� ���������� ���������� - Paluh - 13.03.2007 */

create or replace trigger t_GlobalConsts_AA before insert OR update OR delete on GlobalConsts for each row
declare
	UserName varchar2(64);
	SessionID varchar2(24);
begin
	SELECT SYS_CONTEXT ('DVContext', 'USERNAME'), SYS_CONTEXT('DVContext', 'SESSIONID') into UserName, SessionID FROM dual;
	if inserting then insert into DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (0, 'GlobalConsts', 5, UserName, SessionID, :new.ID);
	elsif updating then insert into DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (1, 'GlobalConsts', 5, UserName, SessionID, :new.ID);
	elsif deleting then insert into DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (2, 'GlobalConsts', 5, UserName, SessionID, :old.ID); end if;
end;
/

commit;

/* End 5267 - ����� �� ���������� ���������� - Paluh - 13.03.2007 */



/* Start - 4578 - �����_0003_������ �������� - gbelov - 20.03.2007 15:23 */

update PUMPREGISTRY
set PROGRAMCONFIG =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="�������� ������������ ����� � ����� ������." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Name="gbVariant" Text="�������" LocationX="13" LocationY="0" Width="320" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="edVariant" Text="��������" Type="Edit" Value="��������" LocationX="6" LocationY="20" Width="300" Height="20"/>
	</ControlsGroup>
</DataPumpParams>'
where PROGRAMIDENTIFIER = 'GRBSOutcomesProjectPump';

commit;

/* End   - 4578 - �����_0003_������ �������� - gbelov - 20.03.2007 15:23 */



/* Start   -  - �������� "���������� �����" - gbelov - 20.03.2007 19:19 */

whenever SQLError continue commit;

/* ���������������� �������� ������������ */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40501, 0, '���������� �����');

/* ���� ������� ��� ��������� ���������� ��������� ����� (50xxx) */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (50000, 0, '���������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (50001, 0, '������ ���������� �����');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (50002, 5, '���������� ������� ���������');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (50003, 6, '���������� ��������� � �������');

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

commit;

whenever SQLError exit rollback;

/* Start   -  - �������� "���������� �����" - gbelov - 20.03.2007 19:19 */


/* Start - 5267 - ����� �� ���������� ����������. - Paluh - 25.03.2007 */

-- ������� ����������� ����� ���������, � �� �� ���, ��� ���� ������
create or replace trigger t_GlobalConsts_AA after insert OR update OR delete on GlobalConsts for each row
declare
	UserName varchar2(64);
	SessionID varchar2(24);
begin
	SELECT SYS_CONTEXT ('DVContext', 'USERNAME'), SYS_CONTEXT('DVContext', 'SESSIONID') into UserName, SessionID FROM dual;
	if inserting then insert into DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (0, 'GlobalConsts', 5, UserName, SessionID, :new.ID);
	elsif updating then insert into DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (1, 'GlobalConsts', 5, UserName, SessionID, :new.ID);
	elsif deleting then insert into DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (2, 'GlobalConsts', 5, UserName, SessionID, :old.ID); end if;
end;
/

commit;

/* End 5267 - ����� �� ���������� ����������. - Paluh - 25.03.2007 */


/* Start - 5750 - ���_0003_1-�� - ��������� �������� ��� � ����� - Feanor - 12.04.2007 */

update PUMPREGISTRY
set PROGRAMCONFIG =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="�������� ������������ ����� � ����� ������." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbDeleteEarlierData" Text="������� ���������� ����� ������ �� ���� �� ���������." LocationX="13" LocationY="20" Width="500" Height="20" Value="True"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="Individual">
		<Control Name="lbComment" Type="Label" Text="��������� ������������ ������ ��� ������� ����� ��������� ������." LocationX="13" LocationY="10" Width="600" Height="20" Value=""/>
	</ControlsGroup>
	<ControlsGroup Name="gbYear" Text="���" LocationX="13" LocationY="40" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="umeYears" Text="" Type="Combo_Years" Value="-1" LocationX="6" LocationY="20" Width="220" Height="20"/>
	</ControlsGroup>
	<ControlsGroup Name="gbMonth" Text="�����" LocationX="251" LocationY="40" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="ucMonths" Text="" Type="Combo_Months" Value="-1" LocationX="6" LocationY="19" Width="220" Height="21"/>
	</ControlsGroup>
</DataPumpParams>'
where PROGRAMIDENTIFIER = 'Form1NMPump';

commit;

/* End  - 5750 - ���_0003_1-�� - ��������� �������� ��� � ����� - Feanor - 12.04.2007 */


/* Start - 5752 - ��_0002_������ - ��������� �������� ��� � ����� - Feanor - 12.04.2007 */

update PUMPREGISTRY
set PROGRAMCONFIG =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="�������� ������������ ����� � ����� ������." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbDeleteEarlierData" Text="������� ���������� ����� ������ �� ���� �� ���������." LocationX="13" LocationY="20" Width="500" Height="20" Value="True"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="Individual">
		<Control Name="lbComment" Type="Label" Text="��������� ������������ ������ ��� ������� ����� ��������� ������." LocationX="13" LocationY="10" Width="600" Height="20" Value=""/>
	</ControlsGroup>
	<ControlsGroup Name="gbYear" Text="���" LocationX="13" LocationY="40" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="umeYears" Text="" Type="Combo_Years" Value="-1" LocationX="6" LocationY="20" Width="220" Height="20"/>
	</ControlsGroup>
	<ControlsGroup Name="gbMonth" Text="�����" LocationX="251" LocationY="40" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="ucMonths" Text="" Type="Combo_Months" Value="-1" LocationX="6" LocationY="19" Width="220" Height="21"/>
	</ControlsGroup>
</DataPumpParams>'
where PROGRAMIDENTIFIER = 'SKIFMonthRepPump';

commit;

/* End  - 5752 - ��_0002_������ - ��������� �������� ��� � ����� - Feanor - 12.04.2007 */



/* Start  - 4121 - ��������� ����� �������� "������� ������" - gbelov - 24.04.2007 */

create or replace view MetaLinksWithRolesNames (ID, Semantic, Name, Class, RoleAName, RoleBName, RefPackages, Configuration) as
select
	L.ID, L.Semantic, L.Name, L.Class,
	case OP.Class when 0 then 'b' when 1 then 'd' when 2 then 'fx' when 3 then 'f' when 10 then 't' end || '.' || OP.Semantic || '.' || OP.Name,
	case OC.Class when 0 then 'b' when 1 then 'd' when 2 then 'fx' when 3 then 'f' when 10 then 't' end || '.' || OC.Semantic || '.' || OC.Name,
	L.RefPackages, L.Configuration
from MetaLinks L join MetaObjects OP on (OP.ID = L.RefParent) join MetaObjects OC on (OC.ID = L.RefChild);

/* End  - 4121 - ��������� ����� �������� "������� ������" - gbelov - 24.04.2007 */



/* Start - 5459 - �������������� ������ ����� - tartushkin - 24.04.2007 */

/* ����������. �������� �������� ��� ���� �������� �� ������ �������� ����������� ���� */
create table Accumulator
(
	ID					number (10) not null,			/* PK */
	ObjectType			number (10) not null,			/* ��� �������:
																���� = 0,
																��� = 1,
																������ ��� = 2,
																������ = 3,
																��������� = 4*/
	DatabaseId			varchar2 (132) not null,		/* ������������� ����. */
	CubeId				varchar2 (132),					/* ������������� ����. ����, ���� ������ - ���������. */
	MeasureGroupId		varchar2 (132),					/* ������������� ������ ���. ����, ���� ������ - ���������. */
	ObjectId			varchar2 (132) not null,		/* ������������� �������. */
	ObjectName			varchar2 (132) not null,		/* ������������ �������. */
	CubeName			varchar2 (132),					/* ��� ����. �����, ���� ������ - ���������. */
	ProcessType			number (10) not null,			/* ��� ��������, �������� ������������ Microsoft.AnalysisServices.ProcessType. */
	RefBatch			number (10),					/* ������ �� ����� */
	State				number (10) not null,			/* ��������� �������, �������� ������������ Microsoft.AnalysisServices.AnalysisState.  */
	LastProcessed		date,							/* ���� ���������� ������� */
	RecordStatus		number (10) not null,			/* ��������� ������:
																�������� = 0,
																� ������ = 1,
																� �������� ������� = 2,
																������ �������� ������� = 3,
																������ �������� � �������� = 4,
																������ ������� ������������� = 5,
																������ ������� ������������ = 6. */
	ProcessReason		number (10) not null,			/* ������� �������:
																�������� ������ �� ����� ������������ = 0,
																������� ������������� = 1,
																�������� ������ �� ������������ = 2,
																������� ������������ = 3,
																����� �� ������� = 4. */
	RefUser				number (10) not null,			/* ������������, ���������� ������ � ����������. */
	AdditionTime		date not null,					/* ����� ���������� ������ � ����������. */
	ErrorMessage		varchar2 (255),					/* ��������� �� ������, ���� ������� ��������� � �������� �������. */
	OptimizationMember	number (1) default 1 not null	/* 1 - ���� ������ ������ �������������� �������������, 0 - � ��������� ������. */
);

create sequence g_Accumulator;

create or replace trigger t_Accumulator_bi before insert on Accumulator for each row
begin
	if :new.ID is null then select g_Accumulator.NextVal into :new.ID from Dual; end if;
end t_Accumulator_bi;
/

/* ������. */
create table Batch
(
	ID					number (10) not null,		/* PK */
	RefUser				number (10) not null,		/* ������������, ���������� �����. */
	AdditionTime		date not null,				/* ����� �������� ������. */
	BatchState			number (10) not null,		/* ��������� ������:
															������������� = 0,
															����������� = 1,
															�������� = 2,
															������� = 3. */
	SessionId			varchar2 (132)				/* ������������� ������, � ������� ���� ������. */
);

create sequence g_Batch;

create or replace trigger t_Batch_bi before insert on Batch for each row
begin
	if :new.ID is null then select g_Batch.NextVal into :new.ID from Dual; end if;
end t_Batch_bi;
/

alter table Batch
	add constraint PKBatch primary key ( ID );
alter table Batch
	add constraint FKBatchRefUser foreign key ( RefUser )
	references Users ( ID );

alter table Accumulator
	add constraint PKAccumulator primary key ( ID );
alter table Accumulator
	add constraint FKAccumulatorRefUser foreign key ( RefUser )
	references Users ( ID );
alter table Accumulator
	add constraint FKAccumulatorRefBatch foreign key ( RefBatch )
	references Batch ( ID );

/* End - 5459 - �������������� ������ ����� - tartushkin - 24.04.2007 */


/* Start - 5834 - ��������� ������� "������������� �������"  - Feanor - 27.04.2007 */

-- ��� �������������� ������������ ������ �������� �� ������������� ������� �
-- �����������, �.�. ���������� ���� ������ ���� ������� � ����� ������ 2.3.0.30.
declare
	cnt pls_integer;
begin
	select count(*) into cnt from PumpRegistry where ProgramIdentifier = 'IncomesDistributionPump';
	if (cnt = 0) then
		insert into PUMPREGISTRY (SUPPLIERCODE, DATACODE, PROGRAMIDENTIFIER, NAME, PROGRAMCONFIG, COMMENTS, STAGESPARAMS, SCHEDULE)
		values ('���', 00012, 'IncomesDistributionPump', '������� ������������� ������� (����������)',
		'<?xml version="1.0" encoding="windows-1251"?>
		<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
			<ControlsGroup Type="Control" ParamsKind="General">
				<Check Name="ucbMoveFilesToArchive" Text="�������� ������������ ����� � ����� ������." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
			</ControlsGroup>
			<ControlsGroup Type="Control" ParamsKind="Individual">
				<Control Name="lbComment" Type="Label" Text="��������� ������������ ������ ��� ������� ����� ��������� ������." LocationX="13" LocationY="10" Width="600" Height="20" Value=""/>
			</ControlsGroup>
			<ControlsGroup Name="gbYear" Text="���" LocationX="13" LocationY="40" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
				<Control Name="umeYears" Text="" Type="Combo_Years" Value="-1" LocationX="6" LocationY="20" Width="220" Height="20"/>
			</ControlsGroup>
			<ControlsGroup Name="gbMonth" Text="�����" LocationX="251" LocationY="40" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
				<Control Name="ucMonths" Text="" Type="Combo_Months" Value="-1" LocationX="6" LocationY="19" Width="220" Height="21"/>
			</ControlsGroup>
		</DataPumpParams>',
		'���������� �� ������������� � ������� �����, ���� ������, ����, ���������� � ������������.',
		'<?xml version="1.0" encoding="windows-1251"?>
		<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
			<PumpData StageInitialState="InQueue" Comment="����������� ������� ������ �� ����������."/>
			<ProcessData StageInitialState="InQueue" Comment="����������� ���� �� ������ ������� ������, �������������� ������� ������� � ������������ � ����� ����������."/>
			<AssociateData StageInitialState="Skipped" Comment="����������� ������������� ������ ��������������� �� ���������� ����������."/>
			<ProcessCube StageInitialState="Skipped" Comment="����������� �������� ��� ���������� ��������� �� ������ ���������� ����������."/>
		</DataPumpStagesParams>',
		'<?xml version="1.0" encoding="windows-1251"?>
		<PumpSchedule xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="PumpSchedule.xsd">
			<ScheduleSettings Enabled="false" StartDate="0" StartTime="0" Periodicity="Once"/>
		</PumpSchedule>');
	end if;
end;
/

commit;

/* End - 5834 - ��������� ������� "������������� �������" - Feanor - 27.04.2007 */



/* Start - 5987 - ��������� ����������� ����������� ����� ������� � ������� �����������  - ������� - 04.05.2007 */

-- ��������� ����� ������� ��� �������� ����� ��������
alter table PumpRegistry add PumpProgram varchar2 (255);

-- ��������� ������������ ������� �� 255 ��������
alter table PumpRegistry modify ProgramIdentifier varchar2 (255);

-- ��������� ����������� ������������ �� ������������� ������� (������� ������ ��� �� ���� ������)
alter table PumpRegistry add constraint UKPumpRegistry unique (ProgramIdentifier);

/* End - 5987 - ��������� ����������� ����������� ����� ������� � ������� �����������  - ������� - 04.05.2007 */



/* Start -  - ������������� ��� ������ �������� � ���� ������ - gbelov - 07.05.2007 */

create or replace view DVDB_Objects (Name, Type) as
select object_name, object_type from user_objects
union
select constraint_name, constraint_type from user_constraints;

/* End -  - ������������� ��� ������ �������� � ���� ������ - gbelov - 07.05.2007 */



/* Start - 6016 - ����� � ��������� ����.������������ - gbelov - 17.05.2007 */

whenever SQLError continue commit;

/* ������� ������ ������ �� �������� */
delete from DataOperations where UserName like 'Krista.FM.Server.DataPumps%';

commit;

drop user DVAudit cascade;
drop tablespace DVAudit including contents and datafiles cascade constraints;

spool off;

/* -- ������� ��������� ������������ DVAudit ------------------------------ */
define UserFile=TempScript.sql

rem ��������� �����������/����������������
set echo off;
set Heading off;
set LineSize 250;
set PageSize 2000;
set SQLPrompt "";
set Autoprint off;
set Feedback off;
set Verify off;

host copy &SpoolFile &SpoolFile.2
spool &UserFile;

select 'create tablespace DVAudit datafile ''' || substr(file_name, 1, length(file_name) - 6) || 'DVAudit.dbf'' size 3M reuse autoextend on next 1280K maxsize unlimited extent management local segment space management auto;' from sys.dba_data_files where tablespace_name = 'DV';
prompt

spool off;

set Heading on;
set Feedback on;
set Autoprint on;
set Verify on;
set LineSize 200;
set PageSize 20;
set SQLPrompt "SQL>";
set echo on;

spool &SpoolFile

@TempScript.sql

/* -- ������� ������������ DVAudit � �������� ��� ����� ------------------- */
create user DVAudit identified by DVAudit default tablespace DVAudit;
alter user DVAudit quota unlimited on DVAudit;
grant connect to DVAudit;
grant create table to DVAudit;
grant create any index to DVAudit;
grant create trigger to DVAudit;

whenever SQLError continue commit;
	grant create sequence to DVAudit;
whenever SQLError exit rollback;

alter user DV quota unlimited on DVAudit;

whenever SQLError continue commit;
	grant drop sequence to DV;
	drop sequence g_DataOperations;
whenever SQLError exit rollback;

rename DataOperations to DataOperationsOld;
alter table DataOperationsOld drop constraint PKDataOperationsID;

grant ALL PRIVILEGES on DV.DataoperationsOld to PUBLIC;


/* -- ������������ ��� ������������� DVAudit ------------------------------ */
connect DVAudit/DVAudit@&DatabaseName;

/* -- ����������� ������������������ � ��������� ������������ DVAudit ----- */
rem ��������� �����������/����������������
set echo off;
set Heading off;
set LineSize 250;
set PageSize 2000;
set SQLPrompt "";
set Autoprint off;
set Feedback off;
set Verify off;

spool off;
host copy &SpoolFile &SpoolFile.3
spool &UserFile;

define tableName = 'DV.DataOperationsOld';
select 'create sequence DVAudit.g_' || 'DataOperations' || ' start with ' || case (select count(ID) from &tableName) when 0 then 1 else (select max(ID) + 1 from &tableName) end || ';' from Dual;
prompt

spool off;

set Heading on;
set Feedback on;
set Autoprint on;
set Verify on;
set LineSize 200;
set PageSize 20;
set SQLPrompt "SQL>";
set echo on;

spool &SpoolFile

@TempScript.sql

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


create or replace trigger t_DataOperations_bi before insert on DataOperations for each row
begin
	if :new.ID is null then select g_DataOperations.NextVal into :new.ID from Dual; end if;
end t_DataOperations_bi;
/

/* -- ������������ ��� ������������� DV ----------------------------------- */
connect DV/&UserPassword@&DatabaseName;

/* �������� ����� �� ������� DVAudit.Dataoperations */
grant ALL PRIVILEGES on DVAudit.Dataoperations to PUBLIC;

/* ������� ������ ������ � ��������� ��������� ������������. ����� ���� ���������� �����.*/
insert into DVAudit.DataOperations select * from DataOperationsOld;

commit;

drop table DataOperationsOld;

-- ������� ����������� ����� ���������, � �� �� ���, ��� ���� ������
create or replace trigger t_GlobalConsts_AA after insert OR update OR delete on GlobalConsts for each row
declare
	UserName varchar2(64);
	SessionID varchar2(24);
begin
	SELECT SYS_CONTEXT ('DVContext', 'USERNAME'), SYS_CONTEXT('DVContext', 'SESSIONID') into UserName, SessionID FROM dual;
	if inserting then insert into DVAudit.DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (0, 'GlobalConsts', 5, UserName, SessionID, :new.ID);
	elsif updating then insert into DVAudit.DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (1, 'GlobalConsts', 5, UserName, SessionID, :new.ID);
	elsif deleting then insert into DVAudit.DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (2, 'GlobalConsts', 5, UserName, SessionID, :old.ID); end if;
end;
/

/* End - 6016 - ����� � ��������� ����.������������ - gbelov - 17.05.2007 */



/* Start -  - ������� ����������� ������� �� ������������� ���������� ���������� ����� - gbelov - 18.05.2007 */

/* ������� ����������� ������� �� ������������� ���������� ���������� �����
	0 - ������ ���������,
	1 - ���������� ����������
   ����� ���������� ������ ������ �������� �������. */
alter table DatabaseVersions
	add NeedUpdate number (1) default 0 not null;

/* End   -  - ������� ����������� ������� �� ������������� ���������� ���������� ����� - gbelov - 18.05.2007 */



/* Start - 5977 - ����� ��� ��������� ��������� - ������� - gbelov - 24.05.2007 */

create or replace view DataSources (
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory, DataSourceName
) as
select
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory,
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

/* End   - 5977 - ����� ��� ��������� ��������� - ������� - gbelov - 24.05.2007 */


insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (21, '2.3.1.0', SYSDATE, SYSDATE, '', 1);

commit;
