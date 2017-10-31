/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		EventProtocol.sql - Подсистема протоколирования событий
	СУБД	SQL Server 2005
*/

:!!echo ================================================================================
:!!echo  Подсистема протоколирования событий
:!!echo ================================================================================

/* Уведомления */
create table Notifications
(
	ID					int not null,			/* PK */
	RefUsers			int not null,			/* Пользователь которому предназначено уведомление */
	EventDateTime		datetime default GETDATE() not null,/* Дата и время записи уведомления */
	Headline			varchar (255) not null,	/* Суть уведомления */
	IsReaded			tinyint default 0 not null,	/* Признак прочитанности */
	IsArchive			tinyint default 0 not null,	/* Признак выполнения (удаления уведомления)*/
	NotifyBody			varbinary (max),		/* XML описание уведомления */
	constraint PKNotifications primary key ( ID ),
	constraint FKNotificationsRefUsers foreign key ( RefUsers )
		references Users ( ID )
);

/* Виды событий */
/*	Таблица с фиксированным набором записей. Не подлежит редактированию у клиентов,
	заполняется при выпуске версии или при обновлении базы данных */
create table KindsOfEvents
(
	ID					int not null,
	ClassOfEvent		smallint not null,		/* Класс события:
														0 - Информационное сообщение,
														1 - Системное сообщение,
														2 - Предупреждение,
														3 - Сообщение об ошибке,
														4 - Сообщение о фатальной ошибке
														5 - Сообщение об успешном завершении операции
														6 - Сообщение о завершении операции c ошибкой */
	Name				varchar (255) not null,	/* Расшифровка вида события */
	constraint PKKindsOfEvents primary key ( ID )
);

/* Виды событий для протокола закачки данных (1xx)*/
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (101, 0, 'Начало операции закачки');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (102, 0, 'Информация в процессе');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (103, 2, 'Предупреждение');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (104, 5, 'Успешное окончание операции закачки');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (105, 6, 'Окончание операции закачки с ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (106, 3, 'Ошибка в процессе закачки');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (107, 4, 'Критическая ошибка в процессе закачки');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (108, 0, 'Начало закачки файла');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (109, 5, 'Завершение закачки файла с ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (110, 6, 'Успешное завершение закачки файла');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (111, 0, 'Начало обработки источника данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (112, 5, 'Завершение обработки источника данных c ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (113, 6, 'Успешное завершение обработки источника данных');

/* Виды событий для протокола сопоставления данных (2xx) */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (201, 0, 'Начало операции сопоставления');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (202, 0, 'Информация в процессе');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (203, 2, 'Предупреждение');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (204, 5, 'Успешное окончание операции сопоставления');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (205, 6, 'Окончание операции сопоставления с ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (206, 3, 'Ошибка в процессе сопоставления');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (207, 4, 'Критическая ошибка в процессе сопоставления');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (211, 0, 'Начало обработки источника данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (212, 5, 'Завершение обработки источника данных c ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (213, 6, 'Успешное завершение обработки источника данных');

/* Виды событий для протоколов обработки многомерных хранилищ (3xx) */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (301, 0, 'Начало операции');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (302, 0, 'Информация в процессе');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (303, 2, 'Предупреждение');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (304, 5, 'Успешное окончание операции');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (305, 6, 'Окончание операции с ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (306, 3, 'Ошибка в процессе выполнения');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (307, 4, 'Критическая ошибка в процессе выполнения');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (308, 0, 'Требование на расчет');

/* Виды событий для протоколов действий пользователей (4xx) */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (401, 1, 'Пользователь подключился к схеме');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (402, 1, 'Пользователь отключился от схемы');

/* Типы системных событий (5xx) */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (502, 0, 'Информация');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (503, 2, 'Предупреждение');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (506, 3, 'Ошибка');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (507, 4, 'Критическая ошибка');

/* Типы событий по сверке данных (6xx) */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (601, 0, 'Начало операции сверки данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (602, 0, 'Информация в процессе');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (603, 2, 'Предупреждение');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (604, 5, 'Успешное окончание операции сверки данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (605, 6, 'Окончание операции сверки данных с ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (606, 3, 'Ошибка в процессе сверки данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (607, 4, 'Критическая ошибка в процессе сверки данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (611, 0, 'Начало обработки источника данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (612, 5, 'Завершение обработки источника данных c ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (613, 6, 'Успешное завершение обработки источника данных');

/* Виды событий для протокола обработки данных (7xx)*/
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (701, 0, 'Начало операции обработки данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (702, 0, 'Информация в обработки данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (703, 2, 'Предупреждение');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (704, 5, 'Успешное окончание операции обработки данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (705, 6, 'Окончание операции обработки данных с ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (706, 3, 'Ошибка в процессе обработки данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (707, 4, 'Критическая ошибка в процессе обработки данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (711, 0, 'Начало обработки источника данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (712, 5, 'Завершение обработки источника данных c ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (713, 6, 'Успешное завершение обработки источника данных');

/* Виды событий для протокола удаления данных (8xx) */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (801, 0, 'Начало операции удаления данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (802, 0, 'Информация в процессе удаления данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (803, 2, 'Предупреждение');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (804, 5, 'Успешное окончание операции удаления данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (805, 6, 'Окончание операции удаления данных с ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (806, 3, 'Ошибка в процессе удаления данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (807, 4, 'Критическая ошибка в процессе удаления данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (811, 0, 'Начало обработки источника данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (812, 5, 'Завершение обработки источника данных c ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (813, 6, 'Успешное завершение обработки источника данных');

insert into KindsOfEvents (ID, ClassOfEvent, Name) values (901, 0, 'Начало операции закачки');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (902, 0, 'Информация в процессе');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (903, 2, 'Предупреждение');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (904, 5, 'Успешное окончание операции закачки');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (905, 6, 'Окончание операции закачки с ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (906, 3, 'Ошибка в процессе закачки');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (907, 4, 'Критическая ошибка в процессе закачки');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (908, 0, 'Начало закачки файла');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (909, 5, 'Завершение закачки файла с ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (910, 6, 'Успешное завершение закачки файла');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (911, 0, 'Начало обработки источника данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (912, 5, 'Завершение обработки источника данных c ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (913, 6, 'Успешное завершение обработки источника данных');

/* Виды событий для протокола "Классификаторы" */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1001, 0, 'Информационное сообщение');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1002, 2, 'Предупреждение');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1003, 3, 'Ошибка');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1004, 4, 'Критическая ошибка');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1005, 0, 'Начало установки иерархии классификатора');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1006, 5, 'Успешное завершение установки иерархии');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1007, 6, 'Завершение установки иерархии с ошибкой');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1008, 0, 'Очищение классификатора');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1009, 0, 'Импортирование данных в классификатор');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1010, 0, 'Формирование сопоставимого классификатора по классификатору данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1011, 5, 'Успешное завершение операции');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1012, 0, 'Создана копия варианта');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1013, 0, 'Вариант закрыт');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1014, 0, 'Вариант открыт');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1015, 0, 'Вариант удален');
insert into kindsofevents (ID, ClassOfEvent, Name) values (1016, 13, 'Блокировка источника данных');
insert into kindsofevents (ID, ClassOfEvent, Name) values (1017, 13, 'Открытие источника данных');
insert into kindsofevents (ID, ClassOfEvent, Name) values (1018, 13, 'Удаление источника данных');

insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40100, 0, 'Пользователь начал работу');

/* Протоколирование операций интерфейса администрирования  */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40301, 0, 'Изменен список пользователей');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40302, 0, 'Изменен список групп');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40303, 0, 'Изменен справочник отделов');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40304, 0, 'Изменен справочник организаций');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40305, 0, 'Изменен справочник типов задач');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40306, 0, 'Изменено членство в группах');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40307, 0, 'Изменен список разрешенных операций');

/* Протоколирование операций интерфейса администрирования  */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40401, 0, 'Создана копия варианта');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40402, 0, 'Вариант закрыт');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40403, 0, 'Вариант открыт');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40404, 0, 'Вариант удален');

/* Протоколирование операций пользователя */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40501, 0, 'Обновление схемы');

/* Протоколирование необработанных ошибок*/
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40666, 0, 'Необработанная ошибка');

/* Архивирование протоколов */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40701, 5, 'Архивирование протоколов');

/* Виды событий для протокола обновления структуры схемы (50xxx) */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (50000, 0, 'Сообщение');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (50001, 0, 'Начало обновления схемы');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (50002, 5, 'Обновление успешно закончено');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (50003, 6, 'Обновление закончено с ошибкой');

/* Виды событий для протокола перевода базы на новый год */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1101, 0, 'Создание источника данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1102, 0, 'Экспорт классификатора данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1103, 0, 'Импорт классификатора данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1104, 0, 'Требование на расчет измерения');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1105, 0, 'Требование на расчет куба');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1106, 2, 'Предупреждение');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1107, 3, 'Ошибка');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1108, 1, 'Начало работы функции перехода на новый год');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1109, 1, 'Окончание работы функции перехода на новый год');

-- Сообщения
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1201, 0, 'Создание нового сообщения от администратора');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1202, 0, 'Удаление сообщения');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1203, 0, 'Очистка неактуальных сообщений');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1204, 0, 'Создание нового сообщения от интерфейса расчета кубов');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1210, 0, 'Создание нового сообщения(прочее)');


/* Общая таблица для всех видов протоколов */
create table HUB_EventProtocol
(
	ID					int not null,			/* PK */
	EventDateTime		datetime default GETDATE() not null, /* Время события */
	Module				varchar (255) not null,	/* Имя сборки из которой пришло событие */
	RefKindsOfEvents	int not null,			/* Вид события */
	InfoMessage			varchar (4000),			/* Информационное сообщение о событии. Содержит дополнительную информацию */
	RefUsersOperations  int,					/* Ссылка на запись протокола "Действия пользователей" */
	ClassOfProtocol		smallint not null,		/* Класс протокола. Фиксированный список "Названия частей системы":
														1 - Закачка данных
														7 - Обработка данных
														2 - Сопоставление классификаторов
														3 - Расчет многомерной базы
														4 - Сверка данных
														5 - Действия пользователей
														6 - Системные сообщения
														7 - Обработка данных
														8 - Удаление сообщения */
	SessionID			varchar (24),			/* Идентификатор сессии */
	constraint PKEventProtocol primary key ( ID ),
	constraint FKEventProtocolRefKindsOfEvent foreign key ( RefKindsOfEvents )
		references KindsOfEvents ( ID ) on delete cascade
);

create table g.HUB_EventProtocol ( ID int identity not null );

/* Закачка данных */
create table SAT_DataPump
(
	ID					int not null,			/* PK */
/* Далее идут поля специфические для данного вида протогола */
	PumpHistoryID		int not null,			/* Операция закачки. Ссылка на PumpHistory.ID */
	DataSourceID		int,					/* Источник. Ссылка на DataSource.ID */
	constraint PKSAT_DataPump primary key ( ID ),
	constraint FKSAT_DataPump foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

go

/* Представление на протокол "Закачка данных" для выборки, вставки и удаления записей */
create view DataPumpProtocol
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.PumpHistoryID, SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_DataPump SAT on (HUB.ID = SAT.ID);

go

create trigger t_DataPumpProtocol_i on DataPumpProtocol instead of insert as
begin
	set nocount on;

	declare @nullsCount int;
	select @nullsCount = count(*) from inserted where ID is null;

	if @nullsCount = 0
	begin
		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, 1 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_DataPump (ID, PumpHistoryID, DataSourceID)
			select ID, PumpHistoryID, DataSourceID from inserted;
	end
	else
	if @nullsCount = 1
	begin
		insert into g.HUB_EventProtocol default values;
		delete from g.HUB_EventProtocol where ID = @@IDENTITY;

		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select @@IDENTITY, Module, KindsOfEvents, RefUsersOperations, InfoMessage, SessionID, 1 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_DataPump (ID, PumpHistoryID, DataSourceID)
			select @@IDENTITY, PumpHistoryID, DataSourceID from inserted;
	end else
		RAISERROR (90900, 1, 1);
end;

go

create trigger t_DataPumpProtocol_u on DataPumpProtocol instead of update as
	RAISERROR (90001, 1, 1);
go

create trigger t_DataPumpProtocol_d on DataPumpProtocol instead of delete as
	delete from HUB_EventProtocol where ID in (select ID from deleted);
go


/* Сопоставление классификаторов */
create table SAT_BridgeOperations
(
	ID					int not null,			/* PK */
	BridgeRoleA			varchar (100) not null,	/* Идентификатор классификатора данных (сопоставляемый) */
	BridgeRoleB			varchar (100) not null,	/* Идентификатор сопоставимого классификатора (сопостаримый) */
	DataSourceID		int,					/* Источник данных */
	PumpHistoryID		int,					/* Операция закачки */
	constraint PKSAT_BridgeOperations primary key ( ID ),
	constraint FKSAT_BridgeOperations foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

go

/* Представление на протокол "Сопоставление классификаторов" для выборки, вставки и удаления записей */
create view BridgeOperations
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	BridgeRoleA, BridgeRoleB, PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.BridgeRoleA, SAT.BridgeRoleB, SAT.PumpHistoryID, SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_BridgeOperations SAT on (HUB.ID = SAT.ID);

go

create trigger t_BridgeOperations_i on BridgeOperations instead of insert as
begin
	set nocount on;

	declare @nullsCount int;
	select @nullsCount = count(*) from inserted where ID is null;

	if @nullsCount = 0
	begin
		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, 2 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_BridgeOperations (ID, BridgeRoleA, BridgeRoleB, PumpHistoryID, DataSourceID)
			select ID, BridgeRoleA, BridgeRoleB, PumpHistoryID, DataSourceID from inserted;
	end
	else
	if @nullsCount = 1
	begin
		insert into g.HUB_EventProtocol default values;
		delete from g.HUB_EventProtocol where ID = @@IDENTITY;

		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select @@IDENTITY, Module, KindsOfEvents, RefUsersOperations, InfoMessage, SessionID, 2 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_BridgeOperations (ID, BridgeRoleA, BridgeRoleB, PumpHistoryID, DataSourceID)
			select @@IDENTITY, BridgeRoleA, BridgeRoleB, PumpHistoryID, DataSourceID from inserted;
	end else
		RAISERROR (90900, 1, 1);
end;

go



create trigger t_BridgeOperations_u on BridgeOperations instead of update as
	RAISERROR (90001, 1, 1);
go

create trigger t_BridgeOperations_d on BridgeOperations instead of delete as
	delete from HUB_EventProtocol where ID in (select ID from deleted);
go


/* Расчет многомерной базы */
create table SAT_MDProcessing
(
	ID					int not null,			/* PK */
	MDObjectName		varchar (255) not null,	/* Имя обрабатываемого объекта (куб или измерение) */
	ObjectIdentifier	varchar (255),			/* Идентификатор объекта */
	OlapObjectType		int,					/* Тип многомерного объекта */
	BatchId				varchar (132),			/* Идентификатор пакета */
	constraint PKSAT_MDProcessing primary key ( ID ),
	constraint FKSAT_MDProcessing foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

go

create view v_MDProcessing
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	MDObjectName, ObjectIdentifier, OlapObjectType, BatchId
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.MDObjectName, SAT.ObjectIdentifier,
	CASE SAT.OlapObjectType
		WHEN 0 THEN 'Пакет'
		WHEN 1 THEN 'Куб'
		WHEN 2 THEN 'Группа мер'
		WHEN 3 THEN 'Раздел куба'
		WHEN 4 THEN 'Измерение'
	END,
	SAT.BatchId
from HUB_EventProtocol HUB join SAT_MDProcessing SAT on (HUB.ID = SAT.ID);

go

/* Представление на протокол "Расчет многомерной базы" для выборки, вставки и удаления записей */
create view MDProcessing
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	MDObjectName, ObjectIdentifier, OlapObjectType, BatchId
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.MDObjectName, SAT.ObjectIdentifier, SAT.OlapObjectType, SAT.BatchId
from HUB_EventProtocol HUB join SAT_MDProcessing SAT on (HUB.ID = SAT.ID);

go

create trigger t_MDProcessing_i on MDProcessing instead of insert as
begin
	set nocount on;

	declare @nullsCount int;
	select @nullsCount = count(*) from inserted where ID is null;

	if @nullsCount = 0
	begin
		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, 3 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_MDProcessing (ID, MDObjectName, ObjectIdentifier, OlapObjectType, BatchId)
			select ID, MDObjectName, ObjectIdentifier, OlapObjectType, BatchId from inserted;
	end
	else
	if @nullsCount = 1
	begin
		insert into g.HUB_EventProtocol default values;
		delete from g.HUB_EventProtocol where ID = @@IDENTITY;

		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select @@IDENTITY, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, 3 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_MDProcessing (ID, MDObjectName, ObjectIdentifier, OlapObjectType, BatchId)
			select @@IDENTITY, MDObjectName, ObjectIdentifier, OlapObjectType, BatchId from inserted;
	end else
		RAISERROR (90900, 1, 1);
end;

go

create trigger t_MDProcessing_u on MDProcessing instead of update as
	RAISERROR (90001, 1, 1);
go

create trigger t_MDProcessing_d on MDProcessing instead of delete as
	delete from HUB_EventProtocol where ID in (select ID from deleted);
go


/* Действия пользователей */
create table SAT_UsersOperations
(
	ID					int not null,				/* PK */
	UserName			varchar (255) not null,		/* Имя пользователя (такое же что и в Users.Name) */
	ObjectName			varchar (255) not null,		/* Имя объекта системы (Objects.Name) */
	ActionName			varchar (255),				/* Наименование(описание) выполненной операции */
	UserHost			varchar (255),				/* Имя машины с которой подключился пользователь */
	constraint PKSAT_UsersOperations primary key ( ID ),
	constraint FKSAT_UsersOperations foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

go

/* Представление на протокол "Действия пользователей" для выборки, вставки и удаления записей */
create view UsersOperations
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, SessionID,
	UserName, ObjectName, ActionName, UserHost
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.SessionID,
	SAT.UserName, SAT.ObjectName, SAT.ActionName, SAT.UserHost
from HUB_EventProtocol HUB join SAT_UsersOperations SAT on (HUB.ID = SAT.ID);

go

create trigger t_UsersOperations_i on UsersOperations instead of insert as
begin
	set nocount on;

	declare @nullsCount int;
	select @nullsCount = count(*) from inserted where ID is null;

	if @nullsCount = 0
	begin
		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, SessionID, ClassOfProtocol)
			select ID, Module, KindsOfEvents, InfoMessage, SessionID, 5 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_UsersOperations (ID, UserName, ObjectName, ActionName, UserHost)
			select ID, UserName, ObjectName, ActionName, UserHost from inserted;
	end
	else
	if @nullsCount = 1
	begin
		insert into g.HUB_EventProtocol default values;
		delete from g.HUB_EventProtocol where ID = @@IDENTITY;

		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, SessionID, ClassOfProtocol)
			select @@IDENTITY, Module, KindsOfEvents, InfoMessage, SessionID, 5 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_UsersOperations (ID, UserName, ObjectName, ActionName, UserHost)
			select @@IDENTITY, UserName, ObjectName, ActionName, UserHost from inserted;
	end else
		RAISERROR (90900, 1, 1);
end;

go


create trigger t_UsersOperations_u on UsersOperations instead of update as
	RAISERROR (90001, 1, 1);
go

create trigger t_UsersOperations_d on UsersOperations instead of delete as
	delete from HUB_EventProtocol where ID in (select ID from deleted);
go


/* Системные сообщения */
create table SAT_SystemEvents
(
	ID					int not null,			/* PK */
	ObjectName			varchar (255) not null,	/* Имя объекта системы (Objects.Name) */
	constraint PKSAT_SystemEvents primary key ( ID ),
	constraint FKSAT_SystemEvents foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

go

/* Представление на протокол "Системные сообщения" для выборки, вставки и удаления записей */
create view SystemEvents
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, SessionID,
	ObjectName
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.SessionID,
	SAT.ObjectName
from HUB_EventProtocol HUB join SAT_SystemEvents SAT on (HUB.ID = SAT.ID);

go

create trigger t_SystemEvents_i on SystemEvents instead of insert as
begin
	set nocount on;

	declare @nullsCount int;
	select @nullsCount = count(*) from inserted where ID is null;

	if @nullsCount = 0
	begin
		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, SessionID, ClassOfProtocol)
			select ID, Module, KindsOfEvents, InfoMessage, SessionID, 6 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_SystemEvents (ID, ObjectName)
			select ID, ObjectName from inserted;
	end
	else
	if @nullsCount = 1
	begin
		insert into g.HUB_EventProtocol default values;
		delete from g.HUB_EventProtocol where ID = @@IDENTITY;

		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, SessionID, ClassOfProtocol)
			select @@IDENTITY, Module, KindsOfEvents, InfoMessage, SessionID, 6 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_SystemEvents (ID, ObjectName)
			select @@IDENTITY, ObjectName from inserted;
	end else
		RAISERROR (90900, 1, 1);
end;

go



create trigger t_SystemEvents_u on SystemEvents instead of update as
	RAISERROR (90001, 1, 1);
go

create trigger t_SystemEvents_d on SystemEvents instead of delete as
	delete from HUB_EventProtocol where ID in (select ID from deleted);
go


/* Протокол для сверки данных */
create table SAT_ReviseData
(
	ID					int not null,			/* PK */
	DataSourceID		int,					/* Источник данных */
	PumpHistoryID		int,					/* Операция закачки */
/* Далее идут поля специфические для данного вида протогола */
	constraint PKSAT_ReviseData primary key ( ID ),
	constraint FKSAT_ReviseData foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

go

/* Представление на протокол "Сверки данных" для выборки, вставки и удаления записей */
create view ReviseDataProtocol
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.PumpHistoryID, SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_ReviseData SAT on (HUB.ID = SAT.ID);

go

create trigger t_ReviseDataProtocol_i on ReviseDataProtocol instead of insert as
begin
	set nocount on;

	declare @nullsCount int;
	select @nullsCount = count(*) from inserted where ID is null;

	if @nullsCount = 0
	begin
		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, 4 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_ReviseData (ID, PumpHistoryID, DataSourceID)
			select ID, PumpHistoryID, DataSourceID from inserted;
	end
	else
	if @nullsCount = 1
	begin
		insert into g.HUB_EventProtocol default values;
		delete from g.HUB_EventProtocol where ID = @@IDENTITY;

		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select @@IDENTITY, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, 4 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_ReviseData (ID, PumpHistoryID, DataSourceID)
			select @@IDENTITY, PumpHistoryID, DataSourceID from inserted;
	end else
		RAISERROR (90900, 1, 1);
end;

go


create trigger t_ReviseDataProtocol_u on ReviseDataProtocol instead of update as
	RAISERROR (90001, 1, 1);
go

create trigger t_ReviseDataProtocol_d on ReviseDataProtocol instead of delete as
	delete from HUB_EventProtocol where ID in (select ID from deleted);
go


/* Обработка данных */
create table SAT_ProcessData
(
	ID					int not null,			/* PK */
/* Далее идут поля специфические для данного вида протогола */
	PumpHistoryID		int not null,			/* Операция закачки. Ссылка на PumpHistory.ID */
	DataSourceID		int,					/* Источник. Ссылка на DataSource.ID */
	constraint PKSAT_ProcessData primary key ( ID ),
	constraint FKSAT_ProcessData foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

go

/* Представление на протокол "Обработка данных" для выборки, вставки и удаления записей */
create view ProcessDataProtocol
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.PumpHistoryID, SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_ProcessData SAT on (HUB.ID = SAT.ID);

go

create trigger t_ProcessDataProtocol_i on ProcessDataProtocol instead of insert as
begin
	set nocount on;

	declare @nullsCount int;
	select @nullsCount = count(*) from inserted where ID is null;

	if @nullsCount = 0
	begin
		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, 7 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_ProcessData (ID, PumpHistoryID, DataSourceID)
			select ID, PumpHistoryID, DataSourceID from inserted;
	end
	else
	if @nullsCount = 1
	begin
		insert into g.HUB_EventProtocol default values;
		delete from g.HUB_EventProtocol where ID = @@IDENTITY;

		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select @@IDENTITY, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, 7 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_ProcessData (ID, PumpHistoryID, DataSourceID)
			select @@IDENTITY, PumpHistoryID, DataSourceID from inserted;
	end else
		RAISERROR (90900, 1, 1);
end;

go



create trigger t_ProcessDataProtocol_u on ProcessDataProtocol instead of update as
	RAISERROR (90001, 1, 1);
go

create trigger t_ProcessDataProtocol_d on ProcessDataProtocol instead of delete as
	delete from HUB_EventProtocol where ID in (select ID from deleted);
go


/* Удаление данных */
create table SAT_DeleteData
(
	ID					int not null,		/* PK */
/* Далее идут поля специфические для данного вида протогола */
	PumpHistoryID		int,				/* Операция закачки. Ссылка на PumpHistory.ID */
	DataSourceID		int,				/* Источник. Ссылка на DataSource.ID */
	constraint PKSAT_DeleteData primary key ( ID ),
	constraint FKSAT_DeleteData foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

go

/* Представление на протокол "Обработка данных" для выборки, вставки и удаления записей */
create view DeleteDataProtocol
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.PumpHistoryID, SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_DeleteData SAT on (HUB.ID = SAT.ID);

go

create trigger t_DeleteDataProtocol_i on DeleteDataProtocol instead of insert as
begin
	set nocount on;

	declare @nullsCount int;
	select @nullsCount = count(*) from inserted where ID is null;

	if @nullsCount = 0
	begin
		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, 8 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_DeleteData (ID, PumpHistoryID, DataSourceID)
			select ID, PumpHistoryID, DataSourceID from inserted;
	end
	else
	if @nullsCount = 1
	begin
		insert into g.HUB_EventProtocol default values;
		delete from g.HUB_EventProtocol where ID = @@IDENTITY;

		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select @@IDENTITY, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, 8 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_DeleteData (ID, PumpHistoryID, DataSourceID)
			select @@IDENTITY, PumpHistoryID, DataSourceID from inserted;
	end else
		RAISERROR (90900, 1, 1);
end;

go



create trigger t_DeleteDataProtocol_u on DeleteDataProtocol instead of update as
	RAISERROR (90001, 1, 1);
go

create trigger t_DeleteDataProtocol_d on DeleteDataProtocol instead of delete as
	delete from HUB_EventProtocol where ID in (select ID from deleted);
go


/* Предпросмотр данных */
create table SAT_PreviewData
(
	ID					int not null,		/* PK */
/* Далее идут поля специфические для данного вида протогола */
	PumpHistoryID		int not null,		/* Операция закачки. Ссылка на PumpHistory.ID */
	DataSourceID		int,				/* Источник. Ссылка на DataSource.ID */
	constraint PKSAT_DataPreview primary key ( ID ),
	constraint FKSAT_DataPreview foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

go

/* Представление на протокол "Закачка данных" для выборки, вставки и удаления записей */
create view PreviewDataProtocol
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.PumpHistoryID, SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_PreviewData SAT on (HUB.ID = SAT.ID);

go

create trigger t_PreviewDataProtocol_i on PreviewDataProtocol instead of insert as
begin
	set nocount on;

	declare @nullsCount int;
	select @nullsCount = count(*) from inserted where ID is null;

	if @nullsCount = 0
	begin
		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, 9 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_PreviewData (ID, PumpHistoryID, DataSourceID)
			select ID, PumpHistoryID, DataSourceID from inserted;
	end
	else
	if @nullsCount = 1
	begin
		insert into g.HUB_EventProtocol default values;
		delete from g.HUB_EventProtocol where ID = @@IDENTITY;

		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select @@IDENTITY, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, 9 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_PreviewData (ID, PumpHistoryID, DataSourceID)
			select @@IDENTITY, PumpHistoryID, DataSourceID from inserted;
	end else
		RAISERROR (90900, 1, 1);
end;

go



create trigger t_PreviewDataProtocol_u on PreviewDataProtocol instead of update as
	RAISERROR (90001, 1, 1);
go

create trigger t_PreviewDataProtocol_d on PreviewDataProtocol instead of delete as
	delete from HUB_EventProtocol where ID in (select ID from deleted);
go


/* протокол "Классификаторы" */
create table SAT_ClassifiersOperations
(
	ID					int not null,			/* PK */
/* Далее идут поля специфические для данного вида протогола */
	Classifier			varchar (255),			/* классификатор, с которым выполняется действие */
	PumpHistoryID		int not null,			/* Операция закачки. Ссылка на PumpHistory.ID */
	DataSourceID		int,					/* Источник. Ссылка на DataSource.ID */
	ObjectType			int,					/* Тип объекта БД */
	constraint PKSAT_ClassifiersOperations primary key ( ID ),
	constraint FKSAT_ClassifiersOperations foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

go

-- создание представления на протокол "Классификаторы"
create view ClassifiersOperations
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	Classifier, PumpHistoryID, DataSourceID, ObjectType
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.Classifier, SAT.PumpHistoryID, SAT.DataSourceID, SAT.ObjectType
from HUB_EventProtocol HUB join SAT_ClassifiersOperations SAT on (HUB.ID = SAT.ID);

go

create trigger t_ClassifiersOperations_i on ClassifiersOperations instead of insert as
begin
	set nocount on;

	declare @nullsCount int;
	select @nullsCount = count(*) from inserted where ID is null;

	if @nullsCount = 0
	begin
		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, 10 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_ClassifiersOperations (ID, PumpHistoryID, DataSourceID, ObjectType)
			select ID, PumpHistoryID, DataSourceID, ObjectType from inserted;
	end
	else
	if @nullsCount = 1
	begin
		insert into g.HUB_EventProtocol default values;
		delete from g.HUB_EventProtocol where ID = @@IDENTITY;

		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select @@IDENTITY, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, 10 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_ClassifiersOperations (ID, PumpHistoryID, DataSourceID, ObjectType)
			select @@IDENTITY, PumpHistoryID, DataSourceID, ObjectType from inserted;
	end else
		RAISERROR (90900, 1, 1);
end;

go

create trigger t_ClassifiersOperations_u on ClassifiersOperations instead of update as
	RAISERROR (90001, 1, 1);
go

create trigger t_ClassifiersOperations_d on ClassifiersOperations instead of delete as
	delete from HUB_EventProtocol where ID in (select ID from deleted);
go


/* Обновление схемы */
create table SAT_UpdateScheme
(
	ID					int not null,		/* PK */
/* Далее идут поля специфические для данного вида протогола */
	ObjectFullName		varchar (64) not null,		/* Английское наименование объекта. */
	ObjectFullCaption	varchar (255),				/* Русское наименование объекта. */
	ModificationType	tinyint,					/* Тип модификации:
														0 - создание нового объекта,
														1 - изменение структуры,
														2 - удаление существующего объекта. */
	constraint PKSAT_UpdateScheme primary key ( ID ),
	constraint FKSAT_UpdateScheme foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

go

/* Представление на протокол "Обновление схемы" для выборки, вставки и удаления записей */
create view UpdateSchemeProtocol
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	ModificationType, ObjectFullName, ObjectFullCaption
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.ModificationType, SAT.ObjectFullName, SAT.ObjectFullCaption
from HUB_EventProtocol HUB join SAT_UpdateScheme SAT on (HUB.ID = SAT.ID);

go

create trigger t_UpdateSchemeProtocol_i on UpdateSchemeProtocol instead of insert as
begin
	set nocount on;

	declare @nullsCount int;
	select @nullsCount = count(*) from inserted where ID is null;

	if @nullsCount = 0
	begin
		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, 12 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_UpdateScheme (ID, ModificationType, ObjectFullName, ObjectFullCaption)
			select ID, ModificationType, ObjectFullName, ObjectFullCaption from inserted;
	end
	else
	if @nullsCount = 1
	begin
		insert into g.HUB_EventProtocol default values;
		delete from g.HUB_EventProtocol where ID = @@IDENTITY;

		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select @@IDENTITY, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, 12 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_UpdateScheme (ID, ModificationType, ObjectFullName, ObjectFullCaption)
			select @@IDENTITY, ModificationType, ObjectFullName, ObjectFullCaption from inserted;
	end else
		RAISERROR (90900, 1, 1);
end;

go

create trigger t_UpdateSchemeProtocol_u on UpdateSchemeProtocol instead of update as
	RAISERROR (90001, 1, 1);
go

create trigger t_UpdateSchemeProtocol_d on UpdateSchemeProtocol instead of delete as
	delete from HUB_EventProtocol where ID in (select ID from deleted);
go


/* Создаем таблицу для протокола операций над источниками. */
create table SAT_DataSourcesOperations
(
	ID int not null,
	dataSourceID int not null,
	constraint PKSAT_DataSourcesOperations primary key ( ID ),
	constraint FKSAT_DataSourcesOperations foreign key ( ID ) references HUB_EventProtocol ( ID ) on delete cascade
);

go

/* И представление для них */
create view DataSourcesOperations (ID, EventDateTime, module, kindsOfEvents, infoMessage, refUsersOperations, sessionID, datasourceID)
as
select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.Sessionid,
	SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_DataSourcesOperations SAT on (HUB.ID = SAT.ID);

go

/* Создаем триггеры для представления*/
create trigger t_DataSourcesOperations_i on DataSourcesOperations instead of insert
as
begin
	set nocount on;

	declare	@nullsCount int;
	select @nullsCount = count(*) from inserted where ID is null;

	if @nullsCount = 0
	begin
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
			select ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, 13, SessionID from inserted;

		insert into  SAT_DataSourcesOperations (ID, DataSourceID)
			select ID, DataSourceID from inserted;
	end
	else
	if @nullsCount = 1
	begin
		insert into g.HUB_EventProtocol default values;
		delete from g.HUB_EventProtocol where ID = @@IDENTITY;

		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
			select @@IDENTITY, Module, KindsOfEvents, InfoMessage, RefUsersOperations, 13, SessionID from inserted;

		insert into  SAT_DataSourcesOperations (ID, DataSourceID)
			select @@IDENTITY, DataSourceID from inserted;

		end else
		RAISERROR (90900, 1, 1);
end;

go

create trigger t_DataSourcesOperations_u on DataSourcesOperations instead of update
as
	RAISERROR (90001, 1, 1);
go

create trigger t_DataSourcesOperations_d on DataSourcesOperations instead of delete
as
	delete from HUB_EventProtocol where ID in (select ID from deleted);
go

/* Протокол операций над функцией перевода базы на новый год */
create table SAT_NewYearOperations
(
	ID					int not null,
	DataSourceID		int not null,
	constraint PKSAT_NewYearOperations primary key ( ID ),
	constraint FKSAT_NewYearOperations foreign key ( ID )
		references HUB_EventProtocol (ID) on delete cascade
);

go

/* И представление для них */
create view NewYearOperations (ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, DataSourceID)
as
  select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.Sessionid,
	SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_NewYearOperations SAT on (HUB.ID = SAT.ID);

go

CREATE TRIGGER T_NewYearOperations_I on NewYearOperations instead of insert as
begin
	set nocount on;

	declare @nullsCount int;
	select @nullsCount = count(*) from inserted where ID is null;

	if @nullsCount = 0
	begin
		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, 2 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_NewYearOperations (ID, DataSourceID)
			select ID, DataSourceID from inserted;
	end
	else
	if @nullsCount = 1
	begin
		insert into g.HUB_EventProtocol default values;
		delete from g.HUB_EventProtocol where ID = @@IDENTITY;

		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select @@IDENTITY, Module, KindsOfEvents, RefUsersOperations, InfoMessage, SessionID, 2 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_NewYearOperations (ID, DataSourceID)
			select @@IDENTITY, DataSourceID from inserted;
	end else
		RAISERROR (90900, 1, 1);
end;

go

create trigger t_NewYearOperations_u on NewYearOperations instead of update as
	RAISERROR (90001, 1, 1);

go

create trigger t_NewYearOperations_d on NewYearOperations instead of delete as
	delete from HUB_EventProtocol where ID in (select ID from deleted);
GO

/* Протокол операций с сообщениями*/
create table SAT_MessageExchangeOperations
(
	ID	int not null,
	constraint PKSAT_MessageExchangeOperations primary key ( ID ),
	constraint FKSAT_MessageExchangeOperations foreign key ( ID )
		references HUB_EventProtocol (ID) on delete cascade
);
go

/* И представление для них */
create view MessageExchangeOperations (ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID)
as
  select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.Sessionid
from HUB_EventProtocol HUB join SAT_MessageExchangeOperations SAT on (HUB.ID = SAT.ID);
go

CREATE TRIGGER T_MessageExchangeOperations_I on MessageExchangeOperations instead of insert as
begin
	set nocount on;

	declare @nullsCount int;
	select @nullsCount = count(*) from inserted where ID is null;

	if @nullsCount = 0
	begin
		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, 2 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_MessageExchangeOperations (ID)
			select ID from inserted;
	end
	else
	if @nullsCount = 1
	begin
		insert into g.HUB_EventProtocol default values;
		delete from g.HUB_EventProtocol where ID = @@IDENTITY;

		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select @@IDENTITY, Module, KindsOfEvents, RefUsersOperations, InfoMessage, SessionID, 2 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_MessageExchangeOperations (ID)
			select @@IDENTITY from inserted;
	end else
		RAISERROR (90900, 1, 1);
end;
go

create trigger t_MessageExchangeOperations_u on MessageExchangeOperations instead of update as
	RAISERROR (90001, 1, 1);
go

create trigger t_MessageExchangeOperations_d on MessageExchangeOperations instead of delete as
	delete from HUB_EventProtocol where ID in (select ID from deleted);
go
