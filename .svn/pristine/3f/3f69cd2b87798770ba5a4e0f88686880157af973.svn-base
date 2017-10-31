/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		Tasks.sql - скрипт создания таблиц под задачи
	СУБД	Oracle 9.2
*/

pro ================================================================================
pro Таблицы под пользователей, группы, права и пр.
pro ================================================================================

/* Задачи */
create table Tasks
(
	ID					number (10) not null,		/* PK */
	State				varchar2 (50) not null,		/* Состояние задачи */
	FromDate			date,						/* Срок исполнения - "с" */
	ToDate				date,						/* Срок исполнения - "по" */
	Doer				number (10) not null,		/* Исполнитель (при создании новой задачи автоматически заполняется из поля "Владелец")*/
	Owner				number (10) not null,		/* Владелец */
	Headline			varchar2 (255) not null,	/* Суть задачи */
	Description			varchar2 (4000),			/* Описание (комментарий) */
	Job					varchar2 (4000) not null,	/* Задание */
	Curator				number (10) not null,		/* Куратор */
	LockByUser			number (10),				/* Признак заблокированности пользователем */
	RefTasks			number (10),				/* Родительская задача */
	RefTasksTypes		number (10) not null,		/* Тип задачи */
	constraint PKTasks primary key ( ID ),
	constraint FKTasksRefDoer foreign key ( Doer )
		references Users ( ID ),
	constraint FKTasksRefOwner foreign key ( Owner )
		references Users ( ID ),
	constraint FKTasksRefTasks foreign key ( RefTasks )
		references Tasks ( ID ),
	constraint FKTasksRefCurator foreign key ( Curator )
		references Users ( ID ),
	constraint FKTasksRefLockByUser foreign key ( LockByUser )
		references Users ( ID ),
	constraint FKTasksRefTasksTypes foreign key ( RefTasksTypes )
		references TasksTypes ( ID )
);

create sequence g_Tasks;

create or replace trigger t_Tasks_bi before insert on Tasks for each row
begin
	if :new.ID is null then select g_Tasks.NextVal into :new.ID from Dual; end if;
	if :new.Curator is null then :new.Curator := 0; end if;
end t_Tasks_bi;
/

insert into Tasks (ID, State, Doer, Owner, Headline, Job, Curator, RefTasksTypes)
	values (-1, 'Создана', 3, 3, 'Закачка данных', 'Закачка данных', 3, 0);

commit;

/* Таблица для промежуточных состояний задач */
create table TasksTemp
(
	ID					number (10) not null,		/* PK  - совпадает с ID основной таблицы (Tasks) */
	State				varchar2 (50) not null,		/* Состояние задачи */
	FromDate			date,						/* Срок исполнения - "с" */
	ToDate				date,						/* Срок исполнения - "по" */
	Doer				number (10) not null,		/* Исполнитель (при создании новой задачи автоматически заполняется из поля "Владелец")*/
	Owner				number (10) not null,		/* Владелец */
	Headline			varchar2 (255) not null,	/* Суть задачи */
	Description			varchar2 (4000),			/* Описание (комментарий) */
	Job 				varchar2 (4000) not null,	/* Задание */
	Curator 			number (10) not null,		/* Куратор */
	LockByUser			number (10),				/* Признак заблокированности пользователем */
	CAction				varchar2 (250),				/* Выполняемое действие */
	CState				varchar2 (250),				/* Предыдущее состояние */
	RefTasks			number (10),				/* Родительская задача */
	RefTasksTypes		number (10) not null,		/* Тип задачи */
	constraint PKTasksTemp primary key ( ID ),
	constraint FKTasksTempRefDoer foreign key ( Doer )
		references Users ( ID ),
	constraint FKTasksTempRefOwner foreign key ( Owner )
		references Users ( ID ),
	constraint FKTasksTempRefCurator foreign key ( Curator )
		references Users ( ID ),
	constraint FKTasksTempRefTasks foreign key ( RefTasks )
		references Tasks ( ID ),
	constraint FKTasksTempRefTasksTypes foreign key ( RefTasksTypes )
		references TasksTypes ( ID )
);

/* Документы задач */
create table Documents
(
	ID					number (10) not null,		/* PK */
	RefTasks			number (10) not null,		/* Ссылка на задачу */
	Name				varchar2 (255) not null,	/* Название документа */
	SourceFileName		varchar2 (255) not null,	/* Название первоначального файла */
	DocumentType		number (5) not null,		/* Тип документа:
														1 - Произвольный документ
														2 - форма ввода
														3 - расчетный лист
														4 - отчет
														5 - форма сбора данных */
	IsPublic			number (1) default 1 not null,/* Признак "приватный/публичный" */
	Version				number (10) not null,		/* Версия документа */
	Document			blob,						/* Сам документ */
	Description			varchar2 (4000),			/* Описание */
	Ownership			number(10) default 0 not null,	/* Принадлежность документа */
	constraint PKDocuments primary key ( ID ),
	constraint FKDocumentsRefTasks foreign key ( RefTasks )
		references Tasks ( ID ) on delete cascade
);

create sequence g_Documents;

create or replace trigger t_Documents_bi before insert on Documents for each row
begin
	if :new.ID is null then select g_Documents.NextVal into :new.ID from Dual; end if;
end t_Documents_bi;
/

/* Таблица-кэш для документов */
create table DocumentsTemp
(
	ID					number (10) not null,		/* PK */
	Name				varchar2 (255) not null,	/* Название документа */
	SourceFileName		varchar2 (255) not null,	/* Название первоначального файла */
	DocumentType		number (5) not null,		/* Тип документа */
	IsPublic			number (1) default 1 not null,/* Признак "приватный/публичный" */
	Version				number (10) not null,		/* Версия документа */
	Document			blob,						/* Сам документ */
	Description			varchar2 (4000),			/* Описание */
	Ownership			number(10) default 0 not null,	/* Принадлежность документа */
	RefTasksTemp		number (10) not null,		/* Ссылка временную на задачу */
	constraint PKDocumentsTemp primary key ( ID ),
	constraint FKDocumentsTempRefTasksTemp foreign key ( RefTasksTemp )
		references TasksTemp ( ID ) on delete cascade
);

/* Таблица параметров задач*/
create table TasksParameters
(
	ID					number (10) not null,		/* PK */
	Name				varchar2 (1000) not null,	/* Название*/
	Dimension			varchar2 (1000),			/* Измерение и семантика*/
	AllowMultiSelect	number (1) default 0 not null, /* Разрешен ли множеств.выбор (0 - нет, 1 - да) */
	Description			varchar2 (2000),			/* Комментарий */
	ParamValues			CLOB,						/* Значение параметра */
	ParamType			number(1) default 0 not null, /* Тип параметра (0 - параметр, 1 - константа)*/
	RefTasks			number (10) not null,		/* Ссылка на задачу */
	constraint PKTasksParameters primary key ( ID ),
	constraint FKTasksParametersRefTasks foreign key ( RefTasks )
		references Tasks ( ID ) on delete cascade
);

/* Последовательность для генерации ID параметров */
create sequence g_TasksParameters;

/* Триггер на вставку в таблицу параметров задач */
create or replace trigger t_TasksParameters_bi before insert on TasksParameters for each row
begin
	if :new.ID is null then select g_TasksParameters.NextVal into :new.ID from Dual; end if;
end t_TasksParameters_bi;
/

/* Таблица-кэш для параметров задач */
create table TasksParametersTemp
(
	ID					number (10) not null,		/* PK */
	Name				varchar2 (1000) not null,	/* Название*/
	Dimension			varchar2 (1000),			/* Измерение и семантика*/
	AllowMultiSelect	number (1) default 0 not null, /* Разрешен ли множеств.выбор (0 - нет, 1 - да) */
	Description			varchar2 (2000),			/* Комментарий */
	ParamValues			CLOB,						/* Значение параметра */
	ParamType			number(1) default 0 not null, /* Тип параметра (0 - параметр, 1 - константа)*/
	RefTasks			number (10) not null,		/* Ссылка на задачу */
	constraint PKTasksParametersTemp primary key ( ID ),
	constraint FKTsksPrmtrsTmpRefTsksTmp foreign key ( RefTasks )
		references TasksTemp ( ID ) on delete cascade
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

	-- копируем параметры во временную таблицу
	insert into TasksParametersTemp
		(ID, Name, Dimension, AllowMultiSelect, Description, ParamValues, ParamType, RefTasks)
	select
		ID, Name, Dimension, AllowMultiSelect, Description, ParamValues, ParamType, RefTasks
	from TasksParameters
	where RefTasks = taskID;

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

		-- переносим документы из временной таблицы в основную
		-- удаляем что было
		delete from Documents where RefTasks = taskID;

		-- переносим что стало
		insert into Documents
			(ID, RefTasks, Name, SourceFileName, DocumentType, IsPublic, Version, Document, Description, Ownership)
		select
			ID, RefTasksTemp, Name, SourceFileName, DocumentType, IsPublic, Version, Document, Description, Ownership
		from DocumentsTemp
		where RefTasksTemp = taskID;

		-- переносим параметры из временной таблицы в основную
		-- удаляем что было
		delete from TasksParameters where RefTasks = taskID;

		-- переносим что стало
		insert into TasksParameters
			(ID, Name, Dimension, AllowMultiSelect, Description, ParamValues, ParamType, RefTasks)
		select
			ID, Name, Dimension, AllowMultiSelect, Description, ParamValues, ParamType, RefTasks
		from TasksParametersTemp
		where RefTasks = taskID;

	end if; -- if canceled = 0

	-- обнуляем поле LockByUser в основной таблице
	update Tasks set LockByUser = null where ID = taskID;

	-- удаляем запись из временной таблицы задач
	delete from TasksTemp where ID = taskID;

	-- удаляем закэшированные документы
	delete from DocumentsTemp where RefTasksTemp = taskID;

	-- удаляем закэшированные параметры
	delete from TasksParametersTemp where RefTasks = taskID;

end sp_EndTaskUpdate;
/

/* Действия */
create table Actions
(
	ID					number (10) not null,		/* PK */
	ActionDate			date not null,				/* Дата и время выполнения действия */
	Action				varchar2 (50) not null,		/* Выполненное действие */
	RefUsers			number (10) not null,		/* Пользователь выполнивший действие */
	OldState			varchar2 (50) not null,		/* Прежнее состояние */
	NewState			varchar2 (50) not null,		/* Новое состояние */
	RefTasks			number (10) not null,		/* Ссылка на задачу */
	constraint PKActions primary key ( ID ),
	constraint FKActionsRefUsers foreign key ( RefUsers )
		references Users ( ID ),
	constraint FKActionsRefTasks foreign key ( RefTasks )
		references Tasks ( ID ) on delete cascade
);

create sequence g_Actions;

commit;
