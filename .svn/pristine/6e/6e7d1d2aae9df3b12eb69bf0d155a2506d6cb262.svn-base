/*******************************************************************
 Переводит базу Oracle из версии 2.1.0 в следующую версию 2.1.1
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */

/* Start - Нет - Изменения в таблицах задач - Борисов - 12.12.2005 */
/* Новые поля Curator и Job в таблице Tasks*/
alter table Tasks add
(
	Curator number (10),	/* Куратор */
	Job varchar2 (4000),		/* Задание */
	LockByUser number (10), /* Признак заблокированности пользователем */
	constraint FKTasksRefCurator foreign key ( Curator )
		references Users ( ID ),
	constraint FKTasksRefLockByUser foreign key ( LockByUser )
		references Users ( ID )
);

update Tasks set Curator = 0, Job = 'Не указано', LockByUser = null;

alter table Tasks modify Curator not null;
alter table Tasks modify Job not null;

/* добавляем триггер на вставку, чтобы работало и на старых версиях */
create or replace trigger t_Tasks_bi before insert on Tasks for each row
begin
	if :new.ID is null then select g_Tasks.NextVal into :new.ID from Dual; end if;
	if :new.Curator is null then :new.Curator := 0; end if;
end t_Tasks_bi;
/

/* Таблица для промежуточных состояний задач */
create table TasksTemp
(
	ID					number (10) not null,		/* PK  - совпадает с ID основной таблицы (Tasks) */
	State				varchar2 (50) not null,		/* Состояние задачи */
	FromDate			date,						/* Срок исполнения - "с" */
	ToDate				date,						/* Срок исполнения - "по" */
	Doer				number (10) not null,		/* Исполнитель (при создании новой задачи автоматически заполняется из поля "Владелец")*/
	Owner				number (10) not null,		/* Владелец */
	Curator 			number (10) not null,		/* Куратор */
	Headline			varchar2 (255) not null,	/* Суть задачи */
	Job 				varchar2 (4000) not null,	/* Задание */
	Description			varchar2 (4000),			/* Описание (комментарий) */
	Year				number (4) not null,		/* Год планирования */
	RefTasks			number (10),				/* Родительская задача */
	constraint PKTasksTemp primary key ( ID ),
	constraint FKTasksTempRefDoer foreign key ( Doer )
		references Users ( ID ),
	constraint FKTasksTempRefOwner foreign key ( Owner )
		references Users ( ID ),
	constraint FKTasksTempRefTasks foreign key ( RefTasks )
		references Tasks ( ID )
);

/* Процедура начала изменения задачи - делает копию записи в таблице временных записей, */
/* устанавливает ссылку на заблокировавшего пользователя */
create or replace procedure SP_BEGINTASKUPDATE(taskID in Tasks.ID%Type, userID in Users.ID%Type) as
  lockByUser Tasks.LockByUser%Type;
begin
	-- проверим не находится ли задача в режиме обновления
	select LockByUser into lockByUser from Tasks where ID = userID;
	if not (lockByUser is null) then
		raise_application_error(20800, 'Задача уже находится в режиме редактирования');
	end if;
	-- копируем содержимое задачи во временную таблицу
	insert into TasksTemp
		(ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks)
	select
		ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, Year, RefTasks
	from Tasks
	where ID = taskID;
	-- устанавливаем значение поля LockByUser в основной таблице
	update Tasks set LockByUser = userID where ID = taskID;
end;
/

/* Процедура сохранения/отката изменений задачи - в зависимости от параметров обновляет */
/* запись в основной таблице, удаляет копию записи в таблице временных записей, */
/* обнуляет ссылку на заблокировавшего пользователя */
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
	-- проверяем находится ли запись в режиме обновления
	select LockByUser into lockByUser from Tasks where ID = taskID;
	if lockByUser is null then
		raise_application_error(20800, 'Задача не находится в режиме редактирования');
	end if;

	-- если действие не отменено - обновляем запись в основной таблице
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
	-- обнуляем поле LockByUser в основной таблице
	update Tasks set LockByUser = null where ID = taskID;
	-- удаляем запись из временной таблицы
	delete from TasksTemp where ID = taskID;
end;
/

commit;
/* End   - Нет - Изменения в таблицах задач - Борисов - 12.12.2005 */

/* Begin   - Нет - В таблицу Tasks - ссылку на справочник видов задач - Борисов - 16.12.2005 */

-- очищаем справочник типов задач
delete from TasksTypes;

-- вставляем в справочник задач временную фиктивную запись
insert into TasksTypes
  (ID, Code, Name, TaskType)
values
  (0, 0, 'Вид задачи не указан', 0);

-- регистрируем в качестве объекта системы
insert into Objects
  (Name, Caption, Description, ObjectType)
values
  ('0_TaskType', 'Вид задачи не указан', 'Вид задачи не указан', 19000);

-- создаем в Tasks новое поле, пока nullable
alter table Tasks add RefTasksTypes number (10);

-- ставим ссылку на времeнную запись справочника
update Tasks set RefTasksTypes = 0;

-- делаем поле not null
alter table Tasks modify RefTasksTypes not null;

-- создаем constraint
alter table Tasks add(
	constraint FKTasksRefTasksTypes foreign key ( RefTasksTypes )
		references TasksTypes ( ID )
);

commit;
/* End   - Нет - В таблицу Tasks - ссылку на справочник видов задач - Борисов - 16.12.2005 */


/* Begin   - Нет - В таблицу Documents - поле принадлежности документа - Борисов - 19.12.2005 */

-- сначала добавляем как nullable
alter table Documents add Ownership number(10);

-- проставляем значение 0 (общий документ)
update Documents set Ownership = 0;

-- делаем not null
alter table Documents modify Ownership not null;

-- добавляем  триггер на вставку чтобы по умолчанию делал новый документ общим
create or replace trigger t_Documents_bi before insert on Documents for each row
begin
	if :new.ID is null then select g_Documents.NextVal into :new.ID from Dual; end if;
	if :new.Ownership is null then :new.Ownership := 0; end if;
end t_Documents_bi;
/

commit;
/* End   - Нет - В таблицу Documents - поле принадлежности документа - Борисов - 19.12.2005 */

insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (6, '2.1.1', SYSDATE, SYSDATE, '');

commit;






