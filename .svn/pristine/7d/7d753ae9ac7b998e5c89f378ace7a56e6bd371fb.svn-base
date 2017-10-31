/*
	АИС "Анализ и планирование"
	ВЕРСИЯ	3.1
	МОДУЛЬ
		EventProtocol.sql - Подсистема протоколирования событий.
	СУБД	Oracle 9.2
*/

pro ================================================================================
pro Подсистема протоколирования событий.
pro ================================================================================

/* Уведомления */
create table Notifications
(
	ID					number (10) not null,			/* PK */
	RefUsers			number (10) not null,			/* Пользователь которому предназначено уведомление */
	EventDateTime		DATE default SYSDATE not null,	/* Дата и время записи уведомления */
	Headline			varchar2 (255) not null,		/* Суть уведомления */
	IsReaded			number (1) default 0 not null,	/* Признак прочитанности */
	IsArchive			number (1) default 0 not null,	/* Признак выполнения (удаления уведомления)*/
	NotifyBody			CLOB,							/* XML описание уведомления */
	constraint PKNotifications primary key ( ID ),
	constraint FKNotificationsRefUsers foreign key ( RefUsers )
		references Users ( ID )
);

/* Виды событий */
/*	Таблица с фиксированным набором записей. Не подлежит редактированию у клиентов,
	заполняется при выпуске версии или при обновлении базы данных */
create table KindsOfEvents
(
	ID					number (10) not null,
	ClassOfEvent		number (5) not null,		/* Класс события:
														0 - Информационное сообщение,
														1 - Системное сообщение,
														2 - Предупреждение,
														3 - Сообщение об ошибке,
														4 - Сообщение о фатальной ошибке
														5 - Сообщение об успешном завершении операции
														6 - Сообщение о завершении операции c ошибкой */
	Name				varchar2 (255) not null,	/* Расшифровка вида события */
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
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1016, 0, 'Блокировка источника данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1017, 0, 'Открытие источника данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1018, 0, 'Удаление источника данных' );

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

/* Протоколирование необработанных ошибок */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40666, 0, 'Необработанная ошибка');

/* Архивирование протоколов */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40701, 5, 'Архивирование протоколов');

/* Виды событий для протокола обновления структуры схемы (50xxx) */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (50000, 0, 'Сообщение');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (50001, 0, 'Начало обновления схемы');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (50002, 5, 'Обновление успешно закончено');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (50003, 6, 'Обновление закончено с ошибкой');

/* Виды событий протоколов для функции перевода базы а новый год */
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
	ID					number (10) not null,		/* PK */
	EventDateTime		DATE default SYSDATE not null, /* Время события */
	Module				varchar2 (255) not null,	/* Имя сборки из которой пришло событие */
	RefKindsOfEvents	number (10) not null,		/* Вид события */
	InfoMessage			varchar2 (4000),			/* Информационное сообщение о событии. Содержит дополнительную информацию */
	RefUsersOperations  number (10),				/* Ссылка на запись протокола "Действия пользователей" */
	ClassOfProtocol		number (4) not null,		/* Класс протокола. Фиксированный список "Названия частей системы":
														1 - Закачка данных
														7 - Обработка данных
														2 - Сопоставление классификаторов
														3 - Расчет многомерной базы
														4 - Сверка данных
														5 - Действия пользователей
														6 - Системные сообщения
														7 - Обработка данных
														8 - Удаление сообщения */
	SessionID varchar2 (24),						/* Идентификатор сессии */
	constraint PKEventProtocol primary key ( ID ),
	constraint FKEventProtocolRefKindsOfEvent foreign key ( RefKindsOfEvents )
		references KindsOfEvents ( ID ) on delete cascade
);

create sequence g_HUB_EventProtocol;

/* Закачка данных */
create table SAT_DataPump
(
	ID					number (10) not null,		/* PK */
/* Далее идут поля специфические для данного вида протогола */
	PumpHistoryID		number (10) not null,		/* Операция закачки. Ссылка на PumpHistory.ID */
	DataSourceID		number (10),				/* Источник. Ссылка на DataSource.ID */
	constraint PKSAT_DataPump primary key ( ID ),
	constraint FKSAT_DataPump foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* Представление на протокол "Закачка данных" для выборки, вставки и удаления записей */
create or replace view DataPumpProtocol
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.PumpHistoryID, SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_DataPump SAT on (HUB.ID = SAT.ID);

create or replace trigger t_DataPumpProtocol_i instead of insert on DataPumpProtocol
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- Получаем значение ключа из генератора
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- Если время события не указано, то устанавливаем в текущее
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- Вставляем запись с общими атрибутами в основной протокол
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 1, :new.SessionID);

	-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
	insert into SAT_DataPump (ID, PumpHistoryID, DataSourceID)
	values (NewID, :new.PumpHistoryID, :new.DataSourceID);
end t_DataPumpProtocol_i;
/

create or replace trigger t_DataPumpProtocol_u instead of update on DataPumpProtocol
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end t_DataPumpProtocol_u;
/

create or replace trigger t_DataPumpProtocol_d instead of delete on DataPumpProtocol
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_DataPumpProtocol_d;
/

/* Сопоставление классификаторов */
create table SAT_BridgeOperations
(
	ID					number (10) not null,		/* PK */
	BridgeRoleA			varchar2 (100) not null,	/* Идентификатор классификатора данных (сопоставляемый) */
	BridgeRoleB			varchar2 (100) not null,	/* Идентификатор сопоставимого классификатора (сопостаримый) */
	DataSourceID		number (10),				/* Источник данных */
	PumpHistoryID		number (10),				/* Операция закачки */
	constraint PKSAT_BridgeOperations primary key ( ID ),
	constraint FKSAT_BridgeOperations foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* Представление на протокол "Сопоставление классификаторов" для выборки, вставки и удаления записей */
create or replace view BridgeOperations
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	BridgeRoleA, BridgeRoleB, PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.Sessionid,
	SAT.BridgeRoleA, SAT.BridgeRoleB, SAT.PumpHistoryID, SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_BridgeOperations SAT on (HUB.ID = SAT.ID);

create or replace trigger t_BridgeOperations_i instead of insert on BridgeOperations
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- Получаем значение ключа из генератора
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- Если время события не указано, то устанавливаем в текущее
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- Вставляем запись с общими атрибутами в основной протокол
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 2, :new.SessionID);

	-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
	insert into SAT_BridgeOperations (ID, BridgeRoleA, BridgeRoleB, PumpHistoryID, DataSourceID)
	values (NewID, :new.BridgeRoleA, :new.BridgeRoleB, :new.PumpHistoryID, :new.DataSourceID);
end t_BridgeOperations_i;
/

create or replace trigger t_BridgeOperations_u instead of update on BridgeOperations
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end t_BridgeOperations_u;
/

create or replace trigger t_BridgeOperations_d instead of delete on BridgeOperations
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_BridgeOperations_d;
/

/* Расчет многомерной базы */
create table SAT_MDProcessing
(
	ID					number (10) not null,		/* PK */
	MDObjectName		varchar2 (255) not null,	/* Имя обрабатываемого объекта (куб или измерение) */
	ObjectIdentifier 	varchar2 (255),				/* Идентификатор объекта */
	OlapObjectType 		number (10),				/* Тип многомерного объекта */
	BatchId				varchar2 (132),				/* Идентификатор пакета */
	constraint PKSAT_MDProcessing primary key ( ID ),
	constraint FKSAT_MDProcessing foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* Представление на протокол "Расчет многомерной базы" для выборки, вставки и удаления записей */
create or replace view v_MDProcessing
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

create or replace view MDProcessing
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	MDObjectName, ObjectIdentifier, OlapObjectType, BatchId
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.MDObjectName, SAT.ObjectIdentifier, SAT.OlapObjectType, SAT.BatchId
from HUB_EventProtocol HUB join SAT_MDProcessing SAT on (HUB.ID = SAT.ID);

create or replace trigger t_MDProcessing_i instead of insert on MDProcessing
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- Получаем значение ключа из генератора
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- Если время события не указано, то устанавливаем в текущее
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- Вставляем запись с общими атрибутами в основной протокол
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 3, :new.SessionID);

	-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
	insert into SAT_MDProcessing (ID, MDObjectName, ObjectIdentifier, OlapObjectType, BatchId)
	values (NewID, :new.MDObjectName, :new.ObjectIdentifier, :new.OlapObjectType, :new.BatchId);
end t_MDProcessing_i;
/

create or replace trigger t_MDProcessing_u instead of update on MDProcessing
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end t_MDProcessing_u;
/

create or replace trigger t_MDProcessing_d instead of delete on MDProcessing
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_MDProcessing_d;
/


/* Действия пользователей */
create table SAT_UsersOperations
(
	ID					number (10) not null,		/* PK */
	UserName			varchar2 (255) not null,	/* Имя пользователя (такое же что и в Users.Name) */
	ObjectName			varchar2 (255) not null,	/* Имя объекта системы (Objects.Name) */
	ActionName			varchar2 (255),				/* Наименование(описание) выполненной операции */
	UserHost			varchar2 (255),				/* Имя машины с которой подключился пользователь */
	constraint PKSAT_UsersOperations primary key ( ID ),
	constraint FKSAT_UsersOperations foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* Представление на протокол "Действия пользователей" для выборки, вставки и удаления записей */
create or replace view UsersOperations
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, SessionID,
	UserName, ObjectName, ActionName, UserHost
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.SessionID,
	SAT.UserName, SAT.ObjectName, SAT.ActionName, SAT.USERHOST
from HUB_EventProtocol HUB join SAT_UsersOperations SAT on (HUB.ID = SAT.ID);

create or replace trigger t_UsersOperations_i instead of insert on UsersOperations
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	NewID := :new.ID;
	-- Если ИД не было передано - получаем значение из генератора
	if NewID is null then select g_HUB_EventProtocol.NextVal into NewID from Dual; end if;

	-- Если время события не указано, то устанавливаем в текущее
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- Вставляем запись с общими атрибутами в основной протокол
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, 5, :new.SessionID);

	-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
	insert into SAT_UsersOperations (ID, UserName, ObjectName, ActionName, UserHost)
	values (NewID, :new.UserName, :new.ObjectName, :new.ActionName, :new.UserHost);
end t_UsersOperations_i;
/

create or replace trigger t_UsersOperations_u instead of update on UsersOperations
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end t_UsersOperations_u;
/

create or replace trigger t_UsersOperations_d instead of delete on UsersOperations
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_UsersOperations_d;
/


/* Системные сообщения */
create table SAT_SystemEvents
(
	ID					number (10) not null,		/* PK */
	ObjectName			varchar2 (255) not null,	/* Имя объекта системы (Objects.Name) */
	constraint PKSAT_SystemEvents primary key ( ID ),
	constraint FKSAT_SystemEvents foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* Представление на протокол "Системные сообщения" для выборки, вставки и удаления записей */
create or replace view SystemEvents
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, SessionID,
	ObjectName
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.SessionID,
	SAT.ObjectName
from HUB_EventProtocol HUB join SAT_SystemEvents SAT on (HUB.ID = SAT.ID);

create or replace trigger t_SystemEvents_i instead of insert on SystemEvents
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- Получаем значение ключа из генератора
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- Если время события не указано, то устанавливаем в текущее
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- Вставляем запись с общими атрибутами в основной протокол
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, 6, :new.SessionID);

	-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
	insert into SAT_SystemEvents (ID, ObjectName)
	values (NewID, :new.ObjectName);
end t_SystemEvents_i;
/

create or replace trigger t_SystemEvents_u instead of update on SystemEvents
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end t_SystemEvents_u;
/

create or replace trigger t_SystemEvents_d instead of delete on SystemEvents
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_SystemEvents_d;
/

/* Протокол для сверки данных */
create table SAT_ReviseData
(
	ID					number (10) not null,		/* PK */
	DataSourceID		number (10),				/* Источник данных */
	PumpHistoryID		number (10),				/* Операция закачки */
/* Далее идут поля специфические для данного вида протогола */
	constraint PKSAT_ReviseData primary key ( ID ),
	constraint FKSAT_ReviseData foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* Представление на протокол "Сверки данных" для выборки, вставки и удаления записей */
create or replace view ReviseDataProtocol
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.PumpHistoryID, SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_ReviseData SAT on (HUB.ID = SAT.ID);

create or replace trigger t_ReviseDataProtocol_i instead of insert on ReviseDataProtocol
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- Получаем значение ключа из генератора
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- Если время события не указано, то устанавливаем в текущее
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- Вставляем запись с общими атрибутами в основной протокол
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 4, :new.SessionID);

	-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
	insert into SAT_ReviseData (ID, PumpHistoryID, DataSourceID)
	values (NewID, :new.PumpHistoryID, :new.DataSourceID);
end t_ReviseDataProtocol_i;
/

create or replace trigger t_ReviseDataProtocol_u instead of update on ReviseDataProtocol
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end t_ReviseData_u;
/

create or replace trigger t_ReviseDataProtocol_d instead of delete on ReviseDataProtocol
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_ReviseData_d;
/

/* Обработка данных */
create table SAT_ProcessData
(
	ID					number (10) not null,		/* PK */
/* Далее идут поля специфические для данного вида протогола */
	PumpHistoryID		number (10) not null,		/* Операция закачки. Ссылка на PumpHistory.ID */
	DataSourceID		number (10),				/* Источник. Ссылка на DataSource.ID */
	constraint PKSAT_ProcessData primary key ( ID ),
	constraint FKSAT_ProcessData foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* Представление на протокол "Обработка данных" для выборки, вставки и удаления записей */
create or replace view ProcessDataProtocol
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.PumpHistoryID, SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_ProcessData SAT on (HUB.ID = SAT.ID);

create or replace trigger t_ProcessDataProtocol_i instead of insert on ProcessDataProtocol
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- Получаем значение ключа из генератора
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- Если время события не указано, то устанавливаем в текущее
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- Вставляем запись с общими атрибутами в основной протокол
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 7, :new.SessionID);

	-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
	insert into SAT_ProcessData (ID, PumpHistoryID, DataSourceID)
	values (NewID, :new.PumpHistoryID, :new.DataSourceID);
end t_ProcessDataProtocol_i;
/

create or replace trigger t_ProcessDataProtocol_u instead of update on ProcessDataProtocol
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end t_ProcessDataProtocol_u;
/

create or replace trigger t_ProcessDataProtocol_d instead of delete on ProcessDataProtocol
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_ProcessDataProtocol_d;
/

/* Удаление данных */
create table SAT_DeleteData
(
	ID					number (10) not null,		/* PK */
/* Далее идут поля специфические для данного вида протогола */
	PumpHistoryID		number (10),				/* Операция закачки. Ссылка на PumpHistory.ID */
	DataSourceID		number (10),				/* Источник. Ссылка на DataSource.ID */
	constraint PKSAT_DeleteData primary key ( ID ),
	constraint FKSAT_DeleteData foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* Представление на протокол "Обработка данных" для выборки, вставки и удаления записей */
create or replace view DeleteDataProtocol
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.PumpHistoryID, SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_DeleteData SAT on (HUB.ID = SAT.ID);

create or replace trigger t_DeleteDataProtocol_i instead of insert on DeleteDataProtocol
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- Получаем значение ключа из генератора
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- Если время события не указано, то устанавливаем в текущее
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- Вставляем запись с общими атрибутами в основной протокол
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 8, :new.SessionID);

	-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
	insert into SAT_DeleteData (ID, PumpHistoryID, DataSourceID)
	values (NewID, :new.PumpHistoryID, :new.DataSourceID);
end t_DeleteDataProtocol_i;
/

create or replace trigger t_DeleteDataProtocol_u instead of update on DeleteDataProtocol
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end t_DeleteDataProtocol_u;
/

create or replace trigger t_DeleteDataProtocol_d instead of delete on DeleteDataProtocol
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_DeleteDataProtocol_d;
/


/* Предпросмотр данных */
create table SAT_PreviewData
(
	ID					number (10) not null,		/* PK */
/* Далее идут поля специфические для данного вида протогола */
	PumpHistoryID		number (10) not null,		/* Операция закачки. Ссылка на PumpHistory.ID */
	DataSourceID		number (10),				/* Источник. Ссылка на DataSource.ID */
	constraint PKSAT_DataPreview primary key ( ID ),
	constraint FKSAT_DataPreview foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* Представление на протокол "Закачка данных" для выборки, вставки и удаления записей */
create or replace view PreviewDataProtocol
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.PumpHistoryID, SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_PreviewData SAT on (HUB.ID = SAT.ID);

create or replace trigger t_PreviewDataProtocol_i instead of insert on PreviewDataProtocol
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- Получаем значение ключа из генератора
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- Если время события не указано, то устанавливаем в текущее
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- Вставляем запись с общими атрибутами в основной протокол
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 1, :new.SessionID);

	-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
	insert into SAT_PreviewData (ID, PumpHistoryID, DataSourceID)
	values (NewID, :new.PumpHistoryID, :new.DataSourceID);
end t_PreviewDataProtocol_i;
/

create or replace trigger t_PreviewDataProtocol_u instead of update on PreviewDataProtocol
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end t_PreviewDataProtocol_u;
/

create or replace trigger t_PreviewDataProtocol_d instead of delete on PreviewDataProtocol
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_PreviewDataProtocol_d;
/


/* протокол "Классификаторы" */
create table SAT_ClassifiersOperations
(
	ID					number (10) not null,		/* PK */
/* Далее идут поля специфические для данного вида протогола */
	Classifier		varchar2(255),					/* классификатор, с которым выполняется действие */
	PumpHistoryID		number (10) not null,		/* Операция закачки. Ссылка на PumpHistory.ID */
	DataSourceID		number (10),				/* Источник. Ссылка на DataSource.ID */
	ObjectType			number (10),				/* Тип объекта БД */
	constraint PKSAT_ClassifiersOperations primary key ( ID ),
	constraint FKSAT_ClassifiersOperations foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* Представление на протокол "Классификаторы" для выборки, вставки и удаления записей */
create or replace view ClassifiersOperations
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	Classifier, PumpHistoryID, DataSourceID, ObjectType
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.Classifier, SAT.PumpHistoryID, SAT.DataSourceID, SAT.ObjectType
from HUB_EventProtocol HUB join SAT_ClassifiersOperations SAT on (HUB.ID = SAT.ID);

create or replace trigger t_ClassifiersOperations_i instead of insert on ClassifiersOperations
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	-- Получаем значение ключа из генератора
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- Если время события не указано, то устанавливаем в текущее
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- Вставляем запись с общими атрибутами в основной протокол
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 2, :new.SessionID);

	-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
	insert into SAT_ClassifiersOperations (ID, Classifier, PumpHistoryID, DataSourceID, ObjectType)
	values (NewID, :new.Classifier, :new.PumpHistoryID, :new.DataSourceID, :new.ObjectType);
end t_ClassifiersOperations_i;
/

create or replace trigger t_ClassifiersOperations_u instead of update on ClassifiersOperations
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end t_ClassifiersOperations_u;
/

create or replace trigger t_ClassifiersOperations_d instead of delete on ClassifiersOperations
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_ClassifiersOperations_d;
/

/* Обновление схемы */
create table SAT_UpdateScheme
(
	ID					number (10) not null,		/* PK */
/* Далее идут поля специфические для данного вида протогола */
	ObjectFullName		varchar2 (64) not null,		/* Английское наименование объекта. */
	ObjectFullCaption	varchar2 (255),				/* Русское наименование объекта. */
	ModificationType	number (1),					/* Тип модификации:
														0 - создание нового объекта,
														1 - изменение структуры,
														2 - удаление существующего объекта. */
	constraint PKSAT_UpdateScheme primary key ( ID ),
	constraint FKSAT_UpdateScheme foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* Представление на протокол "Обновление схемы" для выборки, вставки и удаления записей */
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
	-- Получаем значение ключа из генератора
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	-- Если время события не указано, то устанавливаем в текущее
	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	-- Вставляем запись с общими атрибутами в основной протокол
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 12, :new.SessionID);

	-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
	insert into SAT_UpdateScheme (ID, ModificationType, ObjectFullName, ObjectFullCaption)
	values (NewID, :new.ModificationType, :new.ObjectFullName, :new.ObjectFullCaption);
end t_UpdateSchemeProtocol_i;
/

create or replace trigger t_UpdateSchemeProtocol_u instead of update on UpdateSchemeProtocol
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end t_UpdateSchemeProtocol_u;
/

create or replace trigger t_UpdateSchemeProtocol_d instead of delete on UpdateSchemeProtocol
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_UpdateSchemeProtocol_d;
/


/* Протокол операций над источниками */
create table SAT_DataSourcesOperations
(
	ID					number (10) not null,
	DataSourceID		number (10) not null,
	constraint PKSAT_DataSourcesOperations primary key ( ID ),
	constraint FKSAT_DataSourcesOperations foreign key ( ID )
		references HUB_EventProtocol (ID) on delete cascade
);

/* И представление для них */
create or replace view DataSourcesOperations (ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, DataSourceID)
as
  select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.Sessionid,
	SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_DataSourcesOperations SAT on (HUB.ID = SAT.ID);

CREATE OR REPLACE TRIGGER T_DATASOURCESOPERATIONS_I instead of insert on DataSourcesOperations
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 13, :new.SessionID);

	insert into  SAT_DataSourcesOperations (ID, DataSourceID)
	values (NewID, :new.DataSourceID);
end T_DATASOURCESOPERATIONS_I;
/

create or replace trigger t_DataSourcesOperations_u instead of update on DataSourcesOperations
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end t_DataSourcesOperations_u;
/

create or replace trigger t_DataSourcesOperations_d instead of delete on DataSourcesOperations
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_DataSourcesOperations_d;
/

/* Протокол операций над функцией перевода базы на новый год */
create table SAT_NewYearOperations
(
	ID					number (10) not null,
	DataSourceID		number (10) not null,
	constraint PKSAT_NewYearOperations primary key ( ID ),
	constraint FKSAT_NewYearOperations foreign key ( ID )
		references HUB_EventProtocol (ID) on delete cascade
);

/* И представление для них */
create or replace view NewYearOperations (ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, DataSourceID)
as
  select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.Sessionid,
	SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_NewYearOperations SAT on (HUB.ID = SAT.ID);

CREATE OR REPLACE TRIGGER T_NewYearOperations_I instead of insert on NewYearOperations
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 13, :new.SessionID);

	insert into  SAT_NewYearOperations (ID, DataSourceID)
	values (NewID, :new.DataSourceID);
end T_NewYearOperations_I;
/

create or replace trigger t_NewYearOperations_u instead of update on NewYearOperations
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end t_NewYearOperations_u;
/

create or replace trigger t_NewYearOperations_d instead of delete on NewYearOperations
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_NewYearOperations_d;
/

/* Протоколы для сообщений */
create table SAT_MessageExchangeOperations
(
  ID          number (10) not null,
  constraint PKSAT_MessageOperations primary key ( ID ),
  constraint FKSAT_MessageOperations foreign key ( ID )
    references HUB_EventProtocol (ID) on delete cascade
);

/* И представление для них */
create or replace view MessageExchangeOperations (ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID)
as
  select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.Sessionid
from HUB_EventProtocol HUB join SAT_MessageExchangeOperations SAT on (HUB.ID = SAT.ID);

CREATE OR REPLACE TRIGGER T_MessageExchangeOperations_I instead of insert on MessageExchangeOperations
declare
	NewID pls_integer;
	NewEventDateTime DATE;
begin
	select g_HUB_EventProtocol.NextVal into NewID from Dual;

	NewEventDateTime := :new.EventDateTime;
	if NewEventDateTime is null then NewEventDateTime := SYSDATE; end if;

	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 13, :new.SessionID);

	insert into  SAT_MessageExchangeOperations (ID)
	values (NewID);
end T_MessageExchangeOperations_I;
/

create or replace trigger t_MessageExchangeOperations_u instead of update on MessageExchangeOperations
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end t_MessageExchangeOperations_u;
/

create or replace trigger t_MessageExchangeOperations_d instead of delete on MessageExchangeOperations
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_MessageExchangeOperations_d;
/

commit;

commit work;
