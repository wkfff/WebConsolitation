/*******************************************************************
 ��������� ���� Oracle �� ������ 2.1.1 � ��������� ������ 2.1.2
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */


/* Start - ����������� ����� */

/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */



/* Start - 1695 - ��������� ������ ������� - kutnyashenko - 13.01.2006 */

alter table PumpRegistry add StagesParams CLOB;

/* End - 1695 - ��������� ������ ������� - kutnyashenko - 13.01.2006 */



/* Start - ��� - �������������� ������� - kutnyashenko - 15.01.2006 */

update PumpRegistry
set ProgramIdentifier = 'FNS28nDataPump'
where ProgramIdentifier = 'UMNSDataPump28n';

update PumpRegistry
set ProgramIdentifier = 'FUVaultPump'
where ProgramIdentifier = 'UFKVaultFUPump';

commit;

/* End - ��� - �������������� ������� - kutnyashenko - 15.01.2006 */



/* Start - 1823 - ���������� ��������� ����� - borisov - 24.01.2006 */

/* ��������� � ������� ������������� ��������� ����� */
alter table TasksTemp add
(
	-- ������� �����������������
	LockByUser number (10),
	-- ������ �� ���������� ����� �����
	RefTasksTypes number (10) not null,
	-- ����������� ��������
	CAction varchar2(250),
	-- ���������� ���������
	CState varchar2(250),
	-- ���������� �� ���������� ����� �����
	constraint FKTasksTempRefTasksTypes foreign key ( RefTasksTypes )
		references TasksTypes ( ID )
);

/* ��������� ������ ��������� ������ - ������ ����� ������ � ������� ��������� �������, */
/* ������������� ������ �� ���������������� ������������ */
create or replace procedure sp_BeginTaskUpdate(taskID in Tasks.ID%Type, userID in Users.ID%Type, CAct varchar2, CSt varchar2) as
  lockByUser Tasks.LockByUser%Type;
begin
	-- �������� �� ��������� �� ������ � ������ ����������
	select LockByUser into lockByUser from Tasks where ID = taskID;

	if not (lockByUser is null) then
		raise_application_error(-20800, '������ ��� ��������� � ������ ��������������');
	end if;

	-- ������������� �������� ���� LockByUser � �������� �������
	update Tasks set LockByUser = userID where ID = taskID;

	-- �������� ���������� ������ �� ��������� �������
	insert into TasksTemp
		(ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks, RefTasksTypes, LockByUser)
	select
		ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks, RefTasksTypes, LockByUser
	from Tasks
	where ID = taskID;

	update TasksTemp set CAction = CAct, CState = CSt where ID = taskID;

end sp_BeginTaskUpdate;
/

/* ��������� ����������/������ ��������� ������ - � ����������� �� ���������� ��������� */
/* ������ � �������� �������, ������� ����� ������ � ������� ��������� �������, */
/* �������� ������ �� ���������������� ������������ */
create or replace procedure sp_EndTaskUpdate(taskID in Tasks.ID%Type, canceled in number) as
    recCount number;
  	lockByUser Tasks.LockByUser%Type;
	newState Tasks.State%Type;
	newFromDate Tasks.FromDate%Type;
	newToDate Tasks.ToDate%Type;
	newDoer Tasks.Doer%Type;
	newOwner Tasks.Owner%Type;
	newCurator Tasks.Curator%Type;
	newHeadline Tasks.Headline%Type;
	newJob Tasks.Job%Type;
	newDescription Tasks.Description%Type;
	newYear Tasks.Year%Type;
	newRefTasks Tasks.RefTasks%Type;
	newRefTasksTypes Tasks.RefTasksTypes%Type;
begin
	-- ������� ���� �� ����� ������ � �������� �������
	select count(ID) into recCount from Tasks where ID = taskID;

	-- ���� �������� �� �������� - ��������� ������ � �������� �������
	if canceled = 0 then

		-- � ����������� �� ������� ���������/��������� ������
		if (recCount = 0) then
			insert into Tasks
				(ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks, RefTasksTypes)
			select
				ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks, RefTasksTypes
			from TasksTemp
			where ID = taskID;
		else
    		-- �������� ��������� �����
	      	select
	       		State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks, RefTasksTypes
    		into
    			newState, newFromDate, newToDate, newDoer, newOwner, newCurator, newHeadline, newJob, newDescription, newYear, newRefTasks, newRefTasksTypes
	       	from
		      	TasksTemp
    		where
	       		ID = taskID;
			update Tasks
			set
				State = newState, FromDate = newFromDate, ToDate = newToDate, Doer = newDoer,
				Owner = newOwner, Curator = newCurator, Headline = newHeadline, Job = newJob,
				Description = newDescription, Year = newYear, RefTasks = newRefTasks, RefTasksTypes = newRefTasksTypes
			where
				ID = taskID;
		end if;
	end if;

	-- �������� ���� LockByUser � �������� �������
	update Tasks set LockByUser = null where ID = taskID;

	-- ������� ������ �� ��������� �������
	delete from TasksTemp where ID = taskID;

end sp_EndTaskUpdate;
/

/* End - 1823 - ���������� ��������� ����� - borisov - 24.01.2006 */

/* Start - ��� - ���������� ��������� ���������� ����� - borisov - 30.01.2006 */

/* ������� �������-��� ��� ���������� */
create table DocumentsTemp
(
	ID					number (10) not null,
	RefTasksTemp		number (10) not null,
	Name				varchar2 (255) not null,
	SourceFileName		varchar2 (255) not null,
	DocumentType		number (5) not null,
	IsPublic			number (1) default 1 not null,
	Version				number (10) not null,
	Document			blob,
	Description			varchar2 (4000),
	Ownership			number(10) not null,
	constraint PKDocumentsTemp primary key ( ID ),
	constraint FKDocumentsTempRefTasksTemp foreign key ( RefTasksTemp )
		references TasksTemp ( ID ) on delete cascade
);

/* ���������  ������� �� ������� ����� �� ��������� ����� ����� �������� ����� */
create or replace trigger t_DocumentsTemp_bi before insert on DocumentsTemp for each row
begin
	if :new.ID is null then select g_Documents.NextVal into :new.ID from Dual; end if;
	if :new.Ownership is null then :new.Ownership := 0; end if;
end t_Documents_bi;
/

/* ����� ��������� �������� ���������, ����� ������������ ��� � ��������� */
create or replace procedure sp_BeginTaskUpdate(taskID in Tasks.ID%Type, userID in Users.ID%Type, CAct varchar2, CSt varchar2) as
  lockByUser Tasks.LockByUser%Type;
begin
	-- �������� �� ��������� �� ������ � ������ ����������
	select LockByUser into lockByUser from Tasks where ID = taskID;
	if not (lockByUser is null) then
		raise_application_error(-20800, '������ ��� ��������� � ������ ��������������');
	end if;

	-- ������������� �������� ���� LockByUser � �������� �������
	update Tasks set LockByUser = userID where ID = taskID;

	-- �������� ���������� ������ �� ��������� �������
	insert into TasksTemp
		(ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks, RefTasksTypes, LockByUser)
	select
		ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks, RefTasksTypes, LockByUser
	from Tasks
	where ID = taskID;

	-- ������������� �������� �������� �������� � ��������� ������
	update TasksTemp set CAction = CAct, CState = CSt where ID = taskID;

	-- �������� ��������� ������ �� ��������� �������
	insert into DocumentsTemp
		(ID, RefTasksTemp, Name, SourceFileName, DocumentType, IsPublic, Version, Document, Description, Ownership)
	select
		ID, RefTasks, Name, SourceFileName, DocumentType, IsPublic, Version, Document, Description, Ownership
	from Documents
	where RefTasks = taskID;

end sp_BeginTaskUpdate;
/

create or replace procedure sp_EndTaskUpdate(taskID in Tasks.ID%Type, canceled in number) as
    recCount number;
  	lockByUser Tasks.LockByUser%Type;
	newState Tasks.State%Type;
	newFromDate Tasks.FromDate%Type;
	newToDate Tasks.ToDate%Type;
	newDoer Tasks.Doer%Type;
	newOwner Tasks.Owner%Type;
	newCurator Tasks.Curator%Type;
	newHeadline Tasks.Headline%Type;
	newJob Tasks.Job%Type;
	newDescription Tasks.Description%Type;
	newYear Tasks.Year%Type;
	newRefTasks Tasks.RefTasks%Type;
	newRefTasksTypes Tasks.RefTasksTypes%Type;
begin
	-- ������� ���� �� ����� ������ � �������� �������
	select count(ID) into recCount from Tasks where ID = taskID;

	-- ���� �������� �� �������� - ��������� ������ � �������� �������
	if canceled = 0 then

		-- � ����������� �� ������� ���������/��������� ������
		if (recCount = 0) then
			insert into Tasks
				(ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks, RefTasksTypes)
			select
				ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks, RefTasksTypes
			from TasksTemp
			where ID = taskID;
		else
    		-- �������� ��������� �����
	      	select
	       		State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks, RefTasksTypes
    		into
    			newState, newFromDate, newToDate, newDoer, newOwner, newCurator, newHeadline, newJob, newDescription, newYear, newRefTasks, newRefTasksTypes
	       	from
		      	TasksTemp
    		where
	       		ID = taskID;
			update Tasks
			set
				State = newState, FromDate = newFromDate, ToDate = newToDate, Doer = newDoer,
				Owner = newOwner, Curator = newCurator, Headline = newHeadline, Job = newJob,
				Description = newDescription, Year = newYear, RefTasks = newRefTasks, RefTasksTypes = newRefTasksTypes
			where
				ID = taskID;
		end if; -- if (recCount = 0)

		-- ������ ��������� ���������

		-- ������� ��� ����
		delete from Documents where RefTasks = taskID;

		-- ��������� ��� �����
		insert into Documents
			(ID, RefTasks, Name, SourceFileName, DocumentType, IsPublic, Version, Document, Description, Ownership)
		select
			ID, RefTasksTemp, Name, SourceFileName, DocumentType, IsPublic, Version, Document, Description, Ownership
		from DocumentsTemp
		where RefTasksTemp = taskID;

	end if; -- if canceled = 0

	-- �������� ���� LockByUser � �������� �������
	update Tasks set LockByUser = null where ID = taskID;

	-- ������� ������ �� ��������� ������� �����
	delete from TasksTemp where ID = taskID;

	-- ������� �������������� ���������
	delete from DocumentsTemp where RefTasksTemp = taskID;

end sp_EndTaskUpdate;
/

/* End - ��� - ���������� ��������� ���������� ����� - borisov - 30.01.2006 */


/* Start - 1958 - ���������� � ���������� ���������� ���������� ���������� �� �������- paluh - 30.01.2006 */

-- ������� constraint, ��� ���������� ���������� ����� �� ������������
-- ���� (Basic, Init_Date, Region)
alter table DisintRules_Ex
	drop constraint UKDisintRules_Ex;

-- ����� (RefDisintRules_KD, Init_Date, Region)
alter table DisintRules_Ex
	add constraint UKDisintRules_Ex unique ( RefDisintRules_KD, Init_Date, Region );

/* End - 1958 - ���������� � ���������� ���������� ���������� ���������� �� ������� - paluh - 30.01.2006 */

insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (7, '2.1.2', SYSDATE, SYSDATE, '');

commit;






