/*******************************************************************
 ��������� ���� Oracle �� ������ 2.1.3 � ��������� ������ 2.1.4
*******************************************************************/

/* ������ ���������� �������: */
/* Start - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */
    /* ��� SQL-������ */
/* End   - <����� ������ � ClearQuest> - <���� ������> - <�������> - <����: DD.MM.YYYY> */


/* Start - ����������� ����� */

/* ������� �� ����� ������ */
whenever SQLError exit rollback;
/* End   - ����������� ����� */

/* Start - 1552 - ������� ���� ��� ������������ �� ����� - borisov - 17.02.2006 */

alter table tasks drop column year;
alter table taskstemp drop column year;

/* ������������ ���� ���� �������� ���� ��� ������� */
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
		(ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes, LockByUser)
	select
		ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes, LockByUser
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
				(ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes)
			select
				ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes
			from TasksTemp
			where ID = taskID;
		else
    		-- �������� ��������� �����
	      	select
	       		State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes
    		into
    			newState, newFromDate, newToDate, newDoer, newOwner, newCurator, newHeadline, newJob, newDescription, newRefTasks, newRefTasksTypes
	       	from
		      	TasksTemp
    		where
	       		ID = taskID;
			update Tasks
			set
				State = newState, FromDate = newFromDate, ToDate = newToDate, Doer = newDoer,
				Owner = newOwner, Curator = newCurator, Headline = newHeadline, Job = newJob,
				Description = newDescription, RefTasks = newRefTasks, RefTasksTypes = newRefTasksTypes
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

commit;

/* End - 1552 - ������� ���� ��� ������������ �� ����� - borisov - 17.02.2006 */



/* Start -  - ����� ����������� ������� ����� "�" - gbelov - 27.02.2006 */
alter table MetaObjects
	drop constraint "FKMETAOBJECTSREFPACK�GES";

alter table MetaObjects
	add constraint FKMetaObjectsRefPackages foreign key ( RefPackages )
		references MetaPackages ( ID );

alter table MetaLinks
	drop constraint "FKMETALINKSREFPACK�GES";

alter table MetaLinks
	add constraint FKMetaLinksRefPackages foreign key ( RefPackages )
		references MetaPackages ( ID );

commit;

/* End -  - ����� ����������� ������� ����� "�" - gbelov - 27.02.2006 */

insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (9, '2.1.4', SYSDATE, SYSDATE, '');

commit;

