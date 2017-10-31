/*******************************************************************
 Переводит базу Oracle из версии 2.1.3 в следующую версию 2.1.4
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */

/* Start - 1552 - Убираем поле Год Планирования из задач - borisov - 17.02.2006 */

alter table tasks drop column year;
alter table taskstemp drop column year;

/* Пересоздание этих двух процедур меня уже надоело */
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
		(ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes, LockByUser)
	select
		ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes, LockByUser
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
				(ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes)
			select
				ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes
			from TasksTemp
			where ID = taskID;
		else
    		-- получаем состояния полей
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

commit;

/* End - 1552 - Убираем поле Год Планирования из задач - borisov - 17.02.2006 */



/* Start -  - Опять просочилась русская буква "а" - gbelov - 27.02.2006 */
alter table MetaObjects
	drop constraint "FKMETAOBJECTSREFPACKАGES";

alter table MetaObjects
	add constraint FKMetaObjectsRefPackages foreign key ( RefPackages )
		references MetaPackages ( ID );

alter table MetaLinks
	drop constraint "FKMETALINKSREFPACKАGES";

alter table MetaLinks
	add constraint FKMetaLinksRefPackages foreign key ( RefPackages )
		references MetaPackages ( ID );

commit;

/* End -  - Опять просочилась русская буква "а" - gbelov - 27.02.2006 */

insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (9, '2.1.4', SYSDATE, SYSDATE, '');

commit;

