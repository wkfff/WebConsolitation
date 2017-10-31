/*******************************************************************
 ��������� ���� Oracle �� ������ 2.1.0 � ��������� ������ 2.1.1
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */


/* Start - ����������� ����� */

/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */

/* Start - ��� - ��������� � �������� ����� - ������� - 12.12.2005 */
/* ����� ���� Curator � Job � ������� Tasks*/
alter table Tasks add
(
	Curator number (10),	/* ������� */
	Job varchar2 (4000),		/* ������� */
	LockByUser number (10), /* ������� ����������������� ������������� */
	constraint FKTasksRefCurator foreign key ( Curator )
		references Users ( ID ),
	constraint FKTasksRefLockByUser foreign key ( LockByUser )
		references Users ( ID )
);

update Tasks set Curator = 0, Job = '�� �������', LockByUser = null;

alter table Tasks modify Curator not null;
alter table Tasks modify Job not null;

/* ��������� ������� �� �������, ����� �������� � �� ������ ������� */
create or replace trigger t_Tasks_bi before insert on Tasks for each row
begin
	if :new.ID is null then select g_Tasks.NextVal into :new.ID from Dual; end if;
	if :new.Curator is null then :new.Curator := 0; end if;
end t_Tasks_bi;
/

/* ������� ��� ������������� ��������� ����� */
create table TasksTemp
(
	ID					number (10) not null,		/* PK  - ��������� � ID �������� ������� (Tasks) */
	State				varchar2 (50) not null,		/* ��������� ������ */
	FromDate			date,						/* ���� ���������� - "�" */
	ToDate				date,						/* ���� ���������� - "��" */
	Doer				number (10) not null,		/* ����������� (��� �������� ����� ������ ������������� ����������� �� ���� "��������")*/
	Owner				number (10) not null,		/* �������� */
	Curator 			number (10) not null,		/* ������� */
	Headline			varchar2 (255) not null,	/* ���� ������ */
	Job 				varchar2 (4000) not null,	/* ������� */
	Description			varchar2 (4000),			/* �������� (�����������) */
	Year				number (4) not null,		/* ��� ������������ */
	RefTasks			number (10),				/* ������������ ������ */
	constraint PKTasksTemp primary key ( ID ),
	constraint FKTasksTempRefDoer foreign key ( Doer )
		references Users ( ID ),
	constraint FKTasksTempRefOwner foreign key ( Owner )
		references Users ( ID ),
	constraint FKTasksTempRefTasks foreign key ( RefTasks )
		references Tasks ( ID )
);

/* ��������� ������ ��������� ������ - ������ ����� ������ � ������� ��������� �������, */
/* ������������� ������ �� ���������������� ������������ */
create or replace procedure SP_BEGINTASKUPDATE(taskID in Tasks.ID%Type, userID in Users.ID%Type) as
  lockByUser Tasks.LockByUser%Type;
begin
	-- �������� �� ��������� �� ������ � ������ ����������
	select LockByUser into lockByUser from Tasks where ID = userID;
	if not (lockByUser is null) then
		raise_application_error(20800, '������ ��� ��������� � ������ ��������������');
	end if;
	-- �������� ���������� ������ �� ��������� �������
	insert into TasksTemp
		(ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks)
	select
		ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks
	from Tasks
	where ID = taskID;
	-- ������������� �������� ���� LockByUser � �������� �������
	update Tasks set LockByUser = userID where ID = taskID;
end;
/

/* ��������� ����������/������ ��������� ������ - � ����������� �� ���������� ��������� */
/* ������ � �������� �������, ������� ����� ������ � ������� ��������� �������, */
/* �������� ������ �� ���������������� ������������ */
create or replace procedure SP_ENDTASKUPDATE(taskID in Tasks.ID%Type, canceled in number) as
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
begin
	-- ��������� ��������� �� ������ � ������ ����������
	select LockByUser into lockByUser from Tasks where ID = taskID;
	if lockByUser is null then
		raise_application_error(20800, '������ �� ��������� � ������ ��������������');
	end if;

	-- ���� �������� �� �������� - ��������� ������ � �������� �������
	if canceled = 0 then
		select
			State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks
		into
			newState, newFromDate, newToDate, newDoer, newOwner, newCurator, newHeadline, newJob, newDescription, newYear, newRefTasks
		from
			TasksTemp
		where
			ID = taskID;

		update Tasks
		set
			State = newState, FromDate = newFromDate, ToDate = newToDate, Doer = newDoer,
			Owner = newOwner, Curator = newCurator, Headline = newHeadline, Job = newJob,
			Description = newDescription, Year = newYear, RefTasks = newRefTasks
		where
			ID = taskID;
	end if;
	-- �������� ���� LockByUser � �������� �������
	update Tasks set LockByUser = null where ID = taskID;
	-- ������� ������ �� ��������� �������
	delete from TasksTemp where ID = taskID;
end;
/

commit;
/* End   - ��� - ��������� � �������� ����� - ������� - 12.12.2005 */

/* Begin   - ��� - � ������� Tasks - ������ �� ���������� ����� ����� - ������� - 16.12.2005 */

-- ������� ���������� ����� �����
delete from TasksTypes;

-- ��������� � ���������� ����� ��������� ��������� ������
insert into TasksTypes
  (ID, Code, Name, TaskType)
values
  (0, 0, '��� ������ �� ������', 0);

-- ������������ � �������� ������� �������
insert into Objects
  (Name, Caption, Description, ObjectType)
values
  ('0_TaskType', '��� ������ �� ������', '��� ������ �� ������', 19000);

-- ������� � Tasks ����� ����, ���� nullable
alter table Tasks add RefTasksTypes number (10);

-- ������ ������ �� ����e���� ������ �����������
update Tasks set RefTasksTypes = 0;

-- ������ ���� not null
alter table Tasks modify RefTasksTypes not null;

-- ������� constraint
alter table Tasks add(
	constraint FKTasksRefTasksTypes foreign key ( RefTasksTypes )
		references TasksTypes ( ID )
);

commit;
/* End   - ��� - � ������� Tasks - ������ �� ���������� ����� ����� - ������� - 16.12.2005 */


/* Begin   - ��� - � ������� Documents - ���� �������������� ��������� - ������� - 19.12.2005 */

-- ������� ��������� ��� nullable
alter table Documents add Ownership number(10);

-- ����������� �������� 0 (����� ��������)
update Documents set Ownership = 0;

-- ������ not null
alter table Documents modify Ownership not null;

-- ���������  ������� �� ������� ����� �� ��������� ����� ����� �������� �����
create or replace trigger t_Documents_bi before insert on Documents for each row
begin
	if :new.ID is null then select g_Documents.NextVal into :new.ID from Dual; end if;
	if :new.Ownership is null then :new.Ownership := 0; end if;
end t_Documents_bi;
/

commit;
/* End   - ��� - � ������� Documents - ���� �������������� ��������� - ������� - 19.12.2005 */

insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (6, '2.1.1', SYSDATE, SYSDATE, '');

commit;






