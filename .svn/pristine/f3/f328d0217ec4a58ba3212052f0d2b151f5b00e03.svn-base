/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		Tasks.sql - скрипт создания таблиц под задачи
	СУБД	SQL Server 2005
*/

:!!echo ================================================================================
:!!echo  Подсистема задач
:!!echo ================================================================================

/* Задачи */
create table Tasks
(
	ID					int not null,			/* PK */
	State				varchar (50) not null,	/* Состояние задачи */
	FromDate			datetime,				/* Срок исполнения - "с" */
	ToDate				datetime,				/* Срок исполнения - "по" */
	Doer				int not null,			/* Исполнитель (при создании новой задачи автоматически заполняется из поля "Владелец")*/
	Owner				int not null,			/* Владелец */
	Headline			varchar (255) not null,	/* Суть задачи */
	Description			varchar (4000),			/* Описание (комментарий) */
	Job					varchar (4000) not null,/* Задание */
	Curator				int default 0 not null,	/* Куратор */
	LockByUser			int,					/* Признак заблокированности пользователем */
	RefTasks			int,					/* Родительская задача */
	RefTasksTypes		int not null,			/* Тип задачи */
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

create table g.Tasks ( ID int identity not null );

insert into Tasks (ID, State, Doer, Owner, Headline, Job, Curator, RefTasksTypes)
	values (-1, 'Создана', 3, 3, 'Закачка данных', 'Закачка данных', 3, 0);

go

/* Таблица для промежуточных состояний задач */
create table TasksTemp
(
	ID					int not null,		/* PK  - совпадает с ID основной таблицы (Tasks) */
	State				varchar (50) not null,		/* Состояние задачи */
	FromDate			datetime,						/* Срок исполнения - "с" */
	ToDate				datetime,						/* Срок исполнения - "по" */
	Doer				int not null,		/* Исполнитель (при создании новой задачи автоматически заполняется из поля "Владелец")*/
	Owner				int not null,		/* Владелец */
	Headline			varchar (255) not null,	/* Суть задачи */
	Description			varchar (4000),			/* Описание (комментарий) */
	Job 				varchar (4000) not null,	/* Задание */
	Curator 			int not null,		/* Куратор */
	LockByUser			int,				/* Признак заблокированности пользователем */
	CAction				varchar (250),				/* Выполняемое действие */
	CState				varchar (250),				/* Предыдущее состояние */
	RefTasks			int,				/* Родительская задача */
	RefTasksTypes		int not null,		/* Тип задачи */
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
	ID					int not null,			/* PK */
	RefTasks			int not null,			/* Ссылка на задачу */
	Name				varchar (255) not null,	/* Название документа */
	SourceFileName		varchar (255) not null,	/* Название первоначального файла */
	DocumentType		tinyint not null,		/* Тип документа:
														1 - Произвольный документ
														2 - форма ввода
														3 - расчетный лист
														4 - отчет
														5 - форма сбора данных */
	IsPublic			tinyint default 1 not null,/* Признак "приватный/публичный" */
	Version				int not null,			/* Версия документа */
	Document			varbinary (max),		/* Сам документ */
	Description			varchar (4000),			/* Описание */
	Ownership			int default 0 not null,	/* Принадлежность документа */
	constraint PKDocuments primary key ( ID ),
	constraint FKDocumentsRefTasks foreign key ( RefTasks )
		references Tasks ( ID ) on delete cascade
);

create table g.Documents ( ID int identity not null );

/* Таблица-кэш для документов */
create table DocumentsTemp
(
	ID					int not null,			/* PK */
	Name				varchar (255) not null,	/* Название документа */
	SourceFileName		varchar (255) not null,	/* Название первоначального файла */
	DocumentType		tinyint not null,		/* Тип документа */
	IsPublic			tinyint default 1 not null,/* Признак "приватный/публичный" */
	Version				int not null,			/* Версия документа */
	Document			varbinary (max),		/* Сам документ */
	Description			varchar (4000),			/* Описание */
	Ownership			int default 0 not null,	/* Принадлежность документа */
	RefTasksTemp		int not null,			/* Ссылка временную на задачу */
	constraint PKDocumentsTemp primary key ( ID ),
	constraint FKDocumentsTempRefTasksTemp foreign key ( RefTasksTemp )
		references TasksTemp ( ID ) on delete cascade
);

/* Таблица параметров задач*/
create table TasksParameters
(
	ID					int not null,			/* PK */
	Name				varchar (1000) not null,/* Название*/
	Dimension			varchar (1000),			/* Измерение и семантика*/
	AllowMultiSelect	tinyint default 0 not null, /* Разрешен ли множеств.выбор (0 - нет, 1 - да) */
	Description			varchar (2000),			/* Комментарий */
	ParamValues			varchar (max),		/* Значение параметра */
	ParamType			tinyint default 0 not null, /* Тип параметра (0 - параметр, 1 - константа)*/
	RefTasks			int not null,			/* Ссылка на задачу */
	constraint PKTasksParameters primary key ( ID ),
	constraint FKTasksParametersRefTasks foreign key ( RefTasks )
		references Tasks ( ID ) on delete cascade
);

/* Последовательность для генерации ID параметров */
create table g.TasksParameters ( ID int identity not null );

/* Таблица-кэш для параметров задач */
create table TasksParametersTemp
(
	ID					int not null,			/* PK */
	Name				varchar (1000) not null,/* Название*/
	Dimension			varchar (1000),			/* Измерение и семантика*/
	AllowMultiSelect	tinyint default 0 not null, /* Разрешен ли множеств.выбор (0 - нет, 1 - да) */
	Description			varchar (2000),			/* Комментарий */
	ParamValues			varchar (max),		/* Значение параметра */
	ParamType			tinyint default 0 not null, /* Тип параметра (0 - параметр, 1 - константа)*/
	RefTasks			int not null,			/* Ссылка на задачу */
	constraint PKTasksParametersTemp primary key ( ID ),
	constraint FKTsksPrmtrsTmpRefTsksTmp foreign key ( RefTasks )
		references TasksTemp ( ID ) on delete cascade
);

go

/* Процедура начала изменения задачи - делает копию записи в таблице временных записей, */
/* устанавливает ссылку на заблокировавшего пользователя */
create procedure sp_BeginTaskUpdate
	@taskID int,
	@userID int,
	@CAct varchar (250),
	@CSt varchar (250)
as
begin
	set nocount on;

	declare
		  @lockByUser int;

	-- проверим не находится ли задача в режиме обновления
	select @lockByUser = LockByUser from Tasks where ID = @taskID;
	if not (@lockByUser is null)
		RAISERROR('Задача уже находится в режиме редактирования', 1, 1);

	-- устанавливаем значение поля LockByUser в основной таблице
	update Tasks set LockByUser = @userID where ID = @taskID;

	-- копируем содержимое задачи во временную таблицу
	insert into TasksTemp
		(ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes, LockByUser)
	select
		ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes, LockByUser
	from Tasks
	where ID = @taskID;

	-- устанавливаем значения текущего действия и состояния задачи
	update TasksTemp set CAction = @CAct, CState = @CSt where ID = @taskID;

	-- копируем документы задачи во временную таблицу
	insert into DocumentsTemp
		(ID, RefTasksTemp, Name, SourceFileName, DocumentType, IsPublic, Version, Document, Description, Ownership)
	select
		ID, RefTasks, Name, SourceFileName, DocumentType, IsPublic, Version, Document, Description, Ownership
	from Documents
	where RefTasks = @taskID;

	-- копируем параметры во временную таблицу
	insert into TasksParametersTemp
		(ID, Name, Dimension, AllowMultiSelect, Description, ParamValues, ParamType, RefTasks)
	select
		ID, Name, Dimension, AllowMultiSelect, Description, ParamValues, ParamType, RefTasks
	from TasksParameters
	where RefTasks = @taskID;

end;

go

/* Процедура сохранения/отката изменений задачи - в зависимости от параметров обновляет */
/* запись в основной таблице, удаляет копию записи в таблице временных записей, */
/* обнуляет ссылку на заблокировавшего пользователя */
create procedure sp_EndTaskUpdate
	@taskID int,
	@canceled int
as
begin
	set nocount on;

	declare
	    @recCount int,
  		@lockByUser int,
		@newState varchar (50),
		@newFromDate datetime,
		@newToDate datetime,
		@newDoer int,
		@newOwner int,
		@newCurator int,
		@newHeadline varchar (255),
		@newJob varchar (4000),
		@newDescription varchar (4000),
		@newRefTasks int,
		@newRefTasksTypes int

	-- смотрим была ли такая запись в основной таблице
	select @recCount = count(ID) from Tasks where ID = @taskID;

	-- если действие не отменено - обновляем запись в основной таблице
	if @canceled = 0
	begin
		-- в зависимости от наличия вставляем/обновляем запись
		if @recCount = 0
			insert into Tasks
				(ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes)
			select
				ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes
			from TasksTemp
			where ID = @taskID;
		else
		begin
    		-- получаем состояния полей
	      	select
	       		@newState = State,
	       		@newFromDate = FromDate,
	       		@newToDate = ToDate,
	       		@newDoer = Doer,
	       		@newOwner = Owner,
	       		@newCurator = Curator,
	       		@newHeadline = Headline,
	       		@newJob = Job,
	       		@newDescription = Description,
	       		@newRefTasks = RefTasks,
	       		@newRefTasksTypes = RefTasksTypes
	       	from
		      	TasksTemp
    		where
	       		ID = @taskID;

			update Tasks
			set
				State = @newState, FromDate = @newFromDate, ToDate = @newToDate, Doer = @newDoer,
				Owner = @newOwner, Curator = @newCurator, Headline = @newHeadline, Job = @newJob,
				Description = @newDescription, RefTasks = @newRefTasks, RefTasksTypes = @newRefTasksTypes
			where
				ID = @taskID;
		end; -- if recCount = 0

		-- переносим документы из временной таблицы в основную
		-- удаляем что было
		delete from Documents where RefTasks = @taskID;

		-- переносим что стало
		insert into Documents
			(ID, RefTasks, Name, SourceFileName, DocumentType, IsPublic, Version, Document, Description, Ownership)
		select
			ID, RefTasksTemp, Name, SourceFileName, DocumentType, IsPublic, Version, Document, Description, Ownership
		from DocumentsTemp
		where RefTasksTemp = @taskID;

		-- переносим параметры из временной таблицы в основную
		-- удаляем что было
		delete from TasksParameters where RefTasks = @taskID;

		-- переносим что стало
		insert into TasksParameters
			(ID, Name, Dimension, AllowMultiSelect, Description, ParamValues, ParamType, RefTasks)
		select
			ID, Name, Dimension, AllowMultiSelect, Description, ParamValues, ParamType, RefTasks
		from TasksParametersTemp
		where RefTasks = @taskID;

	end; -- if @canceled = 0

	-- обнуляем поле LockByUser в основной таблице
	update Tasks set LockByUser = null where ID = @taskID;

	-- удаляем запись из временной таблицы задач
	delete from TasksTemp where ID = @taskID;

	-- удаляем закэшированные документы
	delete from DocumentsTemp where RefTasksTemp = @taskID;

	-- удаляем закэшированные параметры
	delete from TasksParametersTemp where RefTasks = @taskID;

end;

go

/* Действия */
create table Actions
(
	ID					int not null,			/* PK */
	ActionDate			datetime not null,		/* Дата и время выполнения действия */
	Action				varchar (50) not null,	/* Выполненное действие */
	RefUsers			int not null,			/* Пользователь выполнивший действие */
	OldState			varchar (50) not null,	/* Прежнее состояние */
	NewState			varchar (50) not null,	/* Новое состояние */
	RefTasks			int not null,			/* Ссылка на задачу */
	constraint PKActions primary key ( ID ),
	constraint FKActionsRefUsers foreign key ( RefUsers )
		references Users ( ID ),
	constraint FKActionsRefTasks foreign key ( RefTasks )
		references Tasks ( ID ) on delete cascade
);

create table g.Actions ( ID int identity not null );

go
