/*******************************************************************
 Переводит базу Oracle из версии 2.1.1 в следующую версию 2.1.2
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */



/* Start - 1695 - Состояния этапов закачек - kutnyashenko - 13.01.2006 */

alter table PumpRegistry add StagesParams CLOB;

/* End - 1695 - Состояния этапов закачек - kutnyashenko - 13.01.2006 */



/* Start - нет - Переименование закачек - kutnyashenko - 15.01.2006 */

update PumpRegistry
set ProgramIdentifier = 'FNS28nDataPump'
where ProgramIdentifier = 'UMNSDataPump28n';

update PumpRegistry
set ProgramIdentifier = 'FUVaultPump'
where ProgramIdentifier = 'UFKVaultFUPump';

commit;

/* End - нет - Переименование закачек - kutnyashenko - 15.01.2006 */



/* Start - 1823 - Сохранение состояния задач - borisov - 24.01.2006 */

/* изменения в таблице промежуточных состояний задач */
alter table TasksTemp add
(
	-- признак заблокированности
	LockByUser number (10),
	-- ссылка на справочник типов задач
	RefTasksTypes number (10) not null,
	-- выполняемое действие
	CAction varchar2(250),
	-- предыдущее состояние
	CState varchar2(250),
	-- констрэйнт на справочник типов задач
	constraint FKTasksTempRefTasksTypes foreign key ( RefTasksTypes )
		references TasksTypes ( ID )
);

/* Процедура начала изменения задачи - делает копию записи в таблице временных записей, */
/* устанавливает ссылку на заблокировавшего пользователя */
create or replace procedure sp_BeginTaskUpdate(taskID in Tasks.ID%Type, userID in Users.ID%Type, CAct varchar2, CSt varchar2) as
  lockByUser Tasks.LockByUser%Type;
begin
	-- проверим не находится ли задача в режиме обновления
	select LockByUser into lockByUser from Tasks where ID = taskID;

	if not (lockByUser is null) then
		raise_application_error(-20800, 'Задача уже находится в режиме редактирования');
	end if;

	-- устанавливаем значение поля LockByUser в основной таблице
	update Tasks set LockByUser = userID where ID = taskID;

	-- копируем содержимое задачи во временную таблицу
	insert into TasksTemp
		(ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks, RefTasksTypes, LockByUser)
	select
		ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks, RefTasksTypes, LockByUser
	from Tasks
	where ID = taskID;

	update TasksTemp set CAction = CAct, CState = CSt where ID = taskID;

end sp_BeginTaskUpdate;
/

/* Процедура сохранения/отката изменений задачи - в зависимости от параметров обновляет */
/* запись в основной таблице, удаляет копию записи в таблице временных записей, */
/* обнуляет ссылку на заблокировавшего пользователя */
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
	-- смотрим была ли такая запись в основной таблице
	select count(ID) into recCount from Tasks where ID = taskID;

	-- если действие не отменено - обновляем запись в основной таблице
	if canceled = 0 then

		-- в зависимости от наличия вставляем/обновляем запись
		if (recCount = 0) then
			insert into Tasks
				(ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks, RefTasksTypes)
			select
				ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks, RefTasksTypes
			from TasksTemp
			where ID = taskID;
		else
    		-- получаем состояния полей
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

	-- обнуляем поле LockByUser в основной таблице
	update Tasks set LockByUser = null where ID = taskID;

	-- удаляем запись из временной таблицы
	delete from TasksTemp where ID = taskID;

end sp_EndTaskUpdate;
/

/* End - 1823 - Сохранение состояния задач - borisov - 24.01.2006 */

/* Start - Нет - Сохранение состояния документов задач - borisov - 30.01.2006 */

/* создаем таблицу-кэш для документов */
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

/* добавляем  триггер на вставку чтобы по умолчанию делал новый документ общим */
create or replace trigger t_DocumentsTemp_bi before insert on DocumentsTemp for each row
begin
	if :new.ID is null then select g_Documents.NextVal into :new.ID from Dual; end if;
	if :new.Ownership is null then :new.Ownership := 0; end if;
end t_Documents_bi;
/

/* Снова обновляем хранимые процедуры, чтобы кэшировались еще и документы */
create or replace procedure sp_BeginTaskUpdate(taskID in Tasks.ID%Type, userID in Users.ID%Type, CAct varchar2, CSt varchar2) as
  lockByUser Tasks.LockByUser%Type;
begin
	-- проверим не находится ли задача в режиме обновления
	select LockByUser into lockByUser from Tasks where ID = taskID;
	if not (lockByUser is null) then
		raise_application_error(-20800, 'Задача уже находится в режиме редактирования');
	end if;

	-- устанавливаем значение поля LockByUser в основной таблице
	update Tasks set LockByUser = userID where ID = taskID;

	-- копируем содержимое задачи во временную таблицу
	insert into TasksTemp
		(ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks, RefTasksTypes, LockByUser)
	select
		ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks, RefTasksTypes, LockByUser
	from Tasks
	where ID = taskID;

	-- устанавливаем значения текущего действия и состояния задачи
	update TasksTemp set CAction = CAct, CState = CSt where ID = taskID;

	-- копируем документы задачи во временную таблицу
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
	-- смотрим была ли такая запись в основной таблице
	select count(ID) into recCount from Tasks where ID = taskID;

	-- если действие не отменено - обновляем запись в основной таблице
	if canceled = 0 then

		-- в зависимости от наличия вставляем/обновляем запись
		if (recCount = 0) then
			insert into Tasks
				(ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks, RefTasksTypes)
			select
				ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks, RefTasksTypes
			from TasksTemp
			where ID = taskID;
		else
    		-- получаем состояния полей
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

		-- теперь переносим документы

		-- удаляем что было
		delete from Documents where RefTasks = taskID;

		-- переносим что стало
		insert into Documents
			(ID, RefTasks, Name, SourceFileName, DocumentType, IsPublic, Version, Document, Description, Ownership)
		select
			ID, RefTasksTemp, Name, SourceFileName, DocumentType, IsPublic, Version, Document, Description, Ownership
		from DocumentsTemp
		where RefTasksTemp = taskID;

	end if; -- if canceled = 0

	-- обнуляем поле LockByUser в основной таблице
	update Tasks set LockByUser = null where ID = taskID;

	-- удаляем запись из временной таблицы задач
	delete from TasksTemp where ID = taskID;

	-- удаляем закэшированные документы
	delete from DocumentsTemp where RefTasksTemp = taskID;

end sp_EndTaskUpdate;
/

/* End - Нет - Сохранение состояния документов задач - borisov - 30.01.2006 */


/* Start - 1958 - добавление и сохранение исключений нормативов отчислений по районам- paluh - 30.01.2006 */

-- изменим constraint, где изменяется комбинация полей на уникальность
-- было (Basic, Init_Date, Region)
alter table DisintRules_Ex
	drop constraint UKDisintRules_Ex;

-- стало (RefDisintRules_KD, Init_Date, Region)
alter table DisintRules_Ex
	add constraint UKDisintRules_Ex unique ( RefDisintRules_KD, Init_Date, Region );

/* End - 1958 - добавление и сохранение исключений нормативов отчислений по районам - paluh - 30.01.2006 */

insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (7, '2.1.2', SYSDATE, SYSDATE, '');

commit;






