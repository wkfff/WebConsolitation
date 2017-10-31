/*******************************************************************
 Переводит базу Oracle из версии 2.1.13 в следующую версию 2.2.0
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */


/* Start - 2565 - Очередная реформа с закачками - процессы - mik-a-el - 8.09.2006 */

update PUMPREGISTRY
set STAGESPARAMS =
	'<?xml version="1.0" encoding="windows-1251"?>
	<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
		<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
		<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
		<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
	</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'LocalBudgetsDataPump' or PROGRAMIDENTIFIER = 'LeasePump' or PROGRAMIDENTIFIER = 'Form1NDPPump';

update PUMPREGISTRY
set STAGESPARAMS =
	'<?xml version="1.0" encoding="windows-1251"?>
	<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
		<PreviewData StageInitialState="InQueue" Comment="Предварительный просмотр данных для закачки."/>
		<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
		<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
		<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
	</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'Form16Pump' or PROGRAMIDENTIFIER = 'Form14Pump' or PROGRAMIDENTIFIER = 'Form13Pump'
	 or PROGRAMIDENTIFIER = 'Form1NApp7Pump' or PROGRAMIDENTIFIER = 'Form1OBLPump'
	 or PROGRAMIDENTIFIER = 'Form5NIOPump' or PROGRAMIDENTIFIER = 'Form1NMPump';

update PUMPREGISTRY
set STAGESPARAMS =
	'<?xml version="1.0" encoding="windows-1251"?>
	<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
		<PreviewData StageInitialState="InQueue" Comment="Предварительный просмотр данных для закачки."/>
		<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
		<ProcessData StageInitialState="InQueue" Comment="Выполняется установка соответствия операционных дней по закачанным данным."/>
		<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
		<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
	</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'FUVaultPump';

update PUMPREGISTRY
set STAGESPARAMS =
	'<?xml version="1.0" encoding="windows-1251"?>
	<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
		<PreviewData StageInitialState="InQueue" Comment="Предварительный просмотр данных для закачки."/>
		<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
		<ProcessData StageInitialState="InQueue" Comment="Выполняется расщепление сумм фактов по нормативам отчислений доходов и установка типа территории."/>
		<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
		<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
	</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'Form10Pump';

update PUMPREGISTRY
set STAGESPARAMS =
	'<?xml version="1.0" encoding="windows-1251"?>
	<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
		<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
		<ProcessData StageInitialState="InQueue" Comment="Выполняется корректировка сумм фактов по иерархии классификаторов."/>
		<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
		<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
	</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'FKMonthRepPump' or PROGRAMIDENTIFIER = 'SKIFMonthRepPump' or PROGRAMIDENTIFIER = 'SKIFYearRepPump';

update PUMPREGISTRY
set STAGESPARAMS =
	'<?xml version="1.0" encoding="windows-1251"?>
	<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
		<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
		<ProcessData StageInitialState="InQueue" Comment="Выполняется расщепление сумм фактов по нормативам отчислений доходов и установка типа территории."/>
		<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
		<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
	</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'FNS28nDataPump';

update PUMPREGISTRY
set STAGESPARAMS =
	'<?xml version="1.0" encoding="windows-1251"?>
	<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
		<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
		<ProcessData StageInitialState="InQueue" Comment="Заполняется классификатор показателей АС Бюджет."/>
		<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
		<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
	</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'BudgetDataPump';

commit;

alter table PUMPHISTORY
modify (SYSTEMVERSION varchar(12));

alter table PUMPHISTORY
modify (PROGRAMVERSION varchar(12));

commit;

/* End   - 2565 - Очередная реформа с закачками - процессы - mik-a-el - 8.09.2006 */



/* Start   - 3439 - Источники без параметров - gbelov - 18.09.2006 */

/* Вид параметров:
														0 - Бюджет, год,
														1 - Год,
														2 - Год, месяц,
														3 - Год, месяц, вариант,
														4 - Год, вариант,
														5 - Год, квартал
														6 - Год, Территория
														7 - Год, квартал, месяц
														8 - Без параметров
*/

create or replace view DataSources (
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory, DataSourceName
) as
select
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory,
	SupplierCode ||' '|| DataName ||
		CASE KindsOfParams
			WHEN 8 THEN ''
			ELSE ' - ' ||
				CASE KindsOfParams
					WHEN 0 THEN Name || ' ' || Year
					WHEN 1 THEN cast(Year as varchar(4))
					WHEN 2 THEN Year || ' ' || Month
					WHEN 3 THEN Year || ' ' || Month || ' ' || Variant
					WHEN 4 THEN Year || ' ' || Variant
					WHEN 5 THEN Year || ' ' || Quarter
					WHEN 6 THEN Year || ' ' || Territory
					WHEN 7 THEN Year || ' ' || Quarter || ' ' || Month
				END
		END
from HUB_DataSources;

/* End   - 3439 - Источники без параметров - gbelov - 18.09.2006 */



/* Start   - 3454 - Копирование вариантов - gbelov - 05.10.2006 */

/* Протоколирование операций интерфейса администрирования  */

insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40401, 0, 'Создана копия варианта');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40402, 0, 'Вариант закрыт');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40403, 0, 'Вариант открыт');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40404, 0, 'Вариант удален');

commit;

/* End   - 3454 - Копирование вариантов - gbelov - 05.10.2006 */


/* Start - 3472 - Аудит - gbelov - 05.10.2006 */

/* Операции по изменению данных */
create table DataOperations
(
	ID number(10) not null ,
	ChangeTime date default SYSDATE not null,
	KindOfOperation number(1) not null,			/* Операция: 0 = insert, 1 = update, 2 = delete */
	ObjectName varchar2 (31) not null,			/* Объект БД */
	ObjectType number (10) default 0 not null,	/* Тип объекта БД. Перечисление Krista.FM.ServerLibrary.DataOperationsObjectTypes */
	UserName varchar2 (64) default USER not null,/* Пользователь */
	SessionID varchar2 (24) not null,			/* ID сессии */
	RecordID number(10) not null,				/* ID записи объекта */
	TaskID number(10),							/* ID задачи (только если с обратной записи; или в интерфейсе таблицы фактов с выбранной задачей) */
	PumpID number(10),							/* ID закачки (заполняется только при закачке) */
	constraint PKDataOperationsID primary key ( ID )
);

create sequence g_DataOperations;

create or replace trigger t_DataOperations_bi before insert on DataOperations for each row
begin
	if :new.ID is null then select g_DataOperations.NextVal into :new.ID from Dual; end if;
end t_DataOperations_bi;
/

/* Глобальный контекст для хранения параметров текущей сессии */
create or replace context DVContext using DVContext accessed globally;

/* Пакет для доступа к контексту параметров текущей сессии */
create or replace package DVContext as
   procedure SetValue(attribute varchar2, value varchar2, username varchar2, client_id varchar2);
end;
/

create or replace package body DVContext as
	/* Установка значения параметра сессии */
	procedure SetValue(attribute varchar2, value varchar2, username varchar2, client_id varchar2) as
	begin
		DBMS_SESSION.SET_CONTEXT ('DVContext', attribute, value, username, client_id);
	end;
end;
/

/* End - 3472 - Аудит - gbelov - 05.10.2006 */



/* Start - 2251 - СУБД MS SQL Server 2005 - адаптация - gbelov - 11.10.2006 */

/* Установку значения по умолчанию переносим из триггера в определение поля */
alter table Documents
	modify Ownership default 0;

create or replace trigger t_Documents_bi before insert on Documents for each row
begin
	if :new.ID is null then select g_Documents.NextVal into :new.ID from Dual; end if;
end t_Documents_bi;
/

/* Установку значения по умолчанию переносим из триггера в определение поля */
alter table DocumentsTemp
	modify Ownership default 0;

create or replace trigger t_DocumentsTemp_bi before insert on DocumentsTemp for each row
begin
	if :new.ID is null then select g_Documents.NextVal into :new.ID from Dual; end if;
end t_Documents_bi;
/

/* Удаляем триггера на вставку для временных таблиц, т.к. они не используются и в них совершенно нет смысла */
/* Еще бы желательно для всех временных таблиц подсистемы задач переделать ссылку на основную таблицу с вида 1:n на вид 1:1,
т.е. поле RefXXXX заменить на ID */

whenever SQLError continue commit;

drop trigger t_DocumentsTemp_bi;
drop trigger t_TasksParametersTemp_bi;

whenever SQLError exit rollback;

/* За ненадобностью */
drop sequence g_ConversionInputAttributes;

/* Представление для получения имен ролей */
create view MetaLinksWithRolesNames (ID, Semantic, Name, Class, RoleAName, RoleBName, RefPackages, Configuration) as
select
	L.ID, L.Semantic, L.Name, L.Class,
	case OP.Class when 0 then 'b' when 1 then 'd' when 2 then 'fx' when 3 then 'f' end || '.' || OP.Semantic || '.' || OP.Name,
	case OC.Class when 0 then 'b' when 1 then 'd' when 2 then 'fx' when 3 then 'f' end || '.' || OC.Semantic || '.' || OC.Name,
	L.RefPackages, L.Configuration
from MetaLinks L join MetaObjects OP on (OP.ID = L.RefParent) join MetaObjects OC on (OC.ID = L.RefChild);

create view MetaConversionTablesCatalog (ID, AssociationName, RuleName) as
select CT.ID, 'a.' || L.Semantic || '.' || L.Name, CT.Rule from MetaConversionTable CT join MetaLinks L on (CT.RefAssociation = L.ID);

/* За ненадобностью */
drop trigger t_DataSources_bi;
drop trigger t_PumpHistory_bi;
drop trigger t_DisintRules_KD_bi;
drop trigger t_DisintRules_AltKD_bi;
drop trigger t_DisintRules_Ex_bi;

/* End   - 2251 - СУБД MS SQL Server 2005 - адаптация - gbelov - 11.10.2006 */

/* start - изменения в протоколах - paluh - 24.10.2006 */

-- перенесено из предыдущего альтера --

-- меняем ссылку в протоколах на новое событие
update HUB_EVENTPROTOCOL set REFKINDSOFEVENTS = 40100 where (REFKINDSOFEVENTS >= 40101 and REFKINDSOFEVENTS <= 40108) or (REFKINDSOFEVENTS >= 40201 and REFKINDSOFEVENTS <= 40208)

commit;

delete from KINDSOFEVENTS where (ID >= 40101 and ID <= 40108) or (ID >= 40201 and ID <= 40208);

commit;

-- изменяем события для предварительного просмотра--
update KindsOfEvents set Name = 'Начало операции' where ID = 901;
update KindsOfEvents set Name = 'Успешное окончание операции' where ID = 904;
update KindsOfEvents set Name = 'Окончание операции с ошибкой' where ID = 905;
update KindsOfEvents set Name = 'Ошибка в процессе' where ID = 906;
update KindsOfEvents set Name = 'Критическая ошибка в процессе' where ID = 907;

commit;

/* end - изменения в протоколах - paluh - 24.10.2006 */

/* start - 4110, 1753 - Протокол Классификаторы, добавление поля Идентификатор сессии - paluh - 24.10.2006 */


/* Добавление поля 'Идентификатор сессии' в протоколы */
alter table HUB_EventProtocol
    	add SessionID varchar2 (24);

commit;


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


/* Изменение на протокол "Сверки данных" */
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


/* Изменение на протокол "Сопоставление классификаторов" */
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


/* Изменения на протокол "Расчет многомерной базы" */
create or replace view MDProcessing
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	MDObjectName, PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.MDObjectName, SAT.PumpHistoryID, SAT.DataSourceID
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
	insert into SAT_MDProcessing (ID, MDObjectName, PumpHistoryID, DataSourceID)
	values (NewID, :new.MDObjectName, :new.PumpHistoryID, :new.DataSourceID);
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


/* Представление на протокол "Предпросмотр данных" для выборки, вставки и удаления записей */
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


/* Представление на протокол "Удаление данных" для выборки, вставки и удаления записей */
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


/* Протокол "Классификаторы" */

-- сообщения --
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



-- создание таблицы
create table SAT_ClassifiersOperations
(
	ID					number (10) not null,		/* PK */
/* Далее идут поля специфические для данного вида протогола */
	Classifier		varchar2(255),			/* классификатор, с которым выполняется действие */
	PumpHistoryID		number (10) not null,		/* Операция закачки. Ссылка на PumpHistory.ID */
	DataSourceID		number (10),				/* Источник. Ссылка на DataSource.ID */
	constraint PKSAT_ClassifiersOperations primary key ( ID ),
	constraint FKSAT_ClassifiersOperations foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

-- создание представления на протокол "Классификаторы"
create or replace view ClassifiersOperations
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	Classifier, PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.Classifier, SAT.PumpHistoryID, SAT.DataSourceID
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
	insert into SAT_ClassifiersOperations (ID, Classifier, PumpHistoryID, DataSourceID)
	values (NewID, :new.Classifier, :new.PumpHistoryID, :new.DataSourceID);
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

commit;


/* end - 4110, 1753 - Протокол Классификаторы, добавление поля Идентификатор сессии - paluh - 24.10.2006 */


/* start - 1753 - Протокол Классификаторы: перенос сообщений для вариантов - paluh - 25.10.2006 */

insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1012, 0, 'Создана копия варианта');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1013, 0, 'Вариант закрыт');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1014, 0, 'Вариант открыт');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1015, 0, 'Вариант удален');

commit;

/* end - 1753 - Протокол Классификаторы: перенос сообщений для вариантов - paluh - 25.10.2006 */



/* Start - 4221 - УФК_0009_Доходы_Результаты регулировки - закачка - mik-a-el - 8.09.2006 */

insert into PUMPREGISTRY (SUPPLIERCODE, DATACODE, PROGRAMIDENTIFIER, NAME, PROGRAMCONFIG, STAGESPARAMS, SCHEDULE, COMMENTS)
values ('УФК', 0009, 'TaxesRegulationDataPump', 'Закачка результатов регулировки налогов между уровнями бюджетов',
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="Помещать обработанные файлы в папку архива." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="Individual">
		<Check Name="ucbFinalOverturn" Text="Закачка заключительных оборотов." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="Individual">
		<Control Name="lbComment" Type="Label" Text="Параметры установки соответствия операционных дней используются только при запуске обработки данных." LocationX="13" LocationY="40" Width="600" Height="20" Value=""/>
	</ControlsGroup>
	<ControlsGroup Name="gbYear" Text="Год" LocationX="13" LocationY="70" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="umeYears" Text="" Type="Combo_Years" Value="-1" LocationX="6" LocationY="20" Width="220" Height="20"/>
	</ControlsGroup>
	<ControlsGroup Name="gbMonth" Text="Месяц" LocationX="251" LocationY="70" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="ucMonths" Text="" Type="Combo_Months" Value="-1" LocationX="6" LocationY="19" Width="220" Height="21"/>
	</ControlsGroup>
</DataPumpParams>',
'<?xml version="1.0" encoding="windows-1251"?>
	<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
		<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
		<ProcessData StageInitialState="InQueue" Comment="Выполняется установка соответствия операционных дней по закачанным данным."/>
		<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
		<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
	</DataPumpStagesParams>',
'<?xml version="1.0" encoding="windows-1251"?>
<PumpSchedule xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="PumpSchedule.xsd">
	<ScheduleSettings Enabled="false" StartDate="0" StartTime="0" Periodicity="Once"/>
</PumpSchedule>',
'Данные о поступлении доходов в бюджеты в разрезе классификации доходов и ОКАТО');

commit;

/* End   - 4221 - УФК_0009_Доходы_Результаты регулировки - закачка - mik-a-el - 8.09.2006 */



/* Start -  - Сортировка пакетов - gbelov - 10.10.2006 */

whenever SQLError continue commit;

alter table MetaPackages
	add OrderBy number (10) default 0;

whenever SQLError exit rollback;

update MetaPackages set OrderBy = '1'  where Name = 'Системные объекты';
update MetaPackages set OrderBy = '2'  where Name = '_Общие классификаторы_Межпроектный';
update MetaPackages set OrderBy = '3'  where Name = '_Общие классификаторы_Анализ';
update MetaPackages set OrderBy = '4'  where Name = '_Общие классификаторы_Планирование';
update MetaPackages set OrderBy = '5'  where Name = '_Общие классификаторы_МРР';
update MetaPackages set OrderBy = '6'  where Name = 'АДМИН';
update MetaPackages set OrderBy = '7'  where Name = 'АДМИН_0001_Отчет об освоении средств';
update MetaPackages set OrderBy = '8'  where Name = 'АДМИН_0002_Результат доходов';
update MetaPackages set OrderBy = '9'  where Name = 'АДМИН_0003_Проект расходов';
update MetaPackages set OrderBy = '10' where Name = 'АДМИН_0004_Результат ИФ';
update MetaPackages set OrderBy = '11' where Name = 'МОФО';
update MetaPackages set OrderBy = '12' where Name = 'МОФО_0001_Недоимка';
update MetaPackages set OrderBy = '13' where Name = 'МОФО_0002_Задолженность';
update MetaPackages set OrderBy = '14' where Name = 'МОФО_0003_Уточненный план';
update MetaPackages set OrderBy = '15' where Name = 'МОФО_0004_Результат доходов';
update MetaPackages set OrderBy = '16' where Name = 'МОФО_0005_Субсидии населению';
update MetaPackages set OrderBy = '17' where Name = 'МОФО_0006_Потребление и оплата услуг';
update MetaPackages set OrderBy = '18' where Name = 'МОФО_0007_Сельские специалисты';
update MetaPackages set OrderBy = '19' where Name = 'МОФО_0008_Результат доходов_краткосрочное';
update MetaPackages set OrderBy = '20' where Name = 'МФРФ';
update MetaPackages set OrderBy = '21' where Name = 'МФРФ_0001_Фонды';
update MetaPackages set OrderBy = '22' where Name = 'МФРФ_0002_Мониторинг требований БК';
update MetaPackages set OrderBy = '23' where Name = 'РЕГИОН';
update MetaPackages set OrderBy = '24' where Name = 'РЕГИОН_0002_131ФЗ';
update MetaPackages set OrderBy = '25' where Name = 'РЕГИОН_0003_Паспорт региона';
update MetaPackages set OrderBy = '26' where Name = 'РЕГИОН_0007_131ФЗ Полномочия МОРФ';
update MetaPackages set OrderBy = '27' where Name = 'РЕГИОН_0008_131ФЗ Полномочия субъектов РФ';
update MetaPackages set OrderBy = '28' where Name = 'РЕГИОН_0009_131ФЗ_Бюджет МО';
update MetaPackages set OrderBy = '29' where Name = 'СТАТ';
update MetaPackages set OrderBy = '30' where Name = 'СТАТ_0001_Ресурсы';
update MetaPackages set OrderBy = '31' where Name = 'СТАТ_0002_Экология';
update MetaPackages set OrderBy = '32' where Name = 'СТАТ_0003_Население';
update MetaPackages set OrderBy = '33' where Name = 'СТАТ_0004_Труд';
update MetaPackages set OrderBy = '34' where Name = 'СТАТ_0005_Уровень жизни';
update MetaPackages set OrderBy = '35' where Name = 'СТАТ_0006_ЖКХ';
update MetaPackages set OrderBy = '36' where Name = 'СТАТ_0007_Образование';
update MetaPackages set OrderBy = '37' where Name = 'СТАТ_0008_Здравоохранение';
update MetaPackages set OrderBy = '38' where Name = 'СТАТ_0009_Спорт';
update MetaPackages set OrderBy = '39' where Name = 'СТАТ_0010_Туризм';
update MetaPackages set OrderBy = '40' where Name = 'СТАТ_0011_Культура';
update MetaPackages set OrderBy = '41' where Name = 'СТАТ_0012_ВРП';
update MetaPackages set OrderBy = '42' where Name = 'СТАТ_0013_Организации';
update MetaPackages set OrderBy = '43' where Name = 'СТАТ_0014_Производство';
update MetaPackages set OrderBy = '44' where Name = 'СТАТ_0015_АПК';
update MetaPackages set OrderBy = '45' where Name = 'СТАТ_0016_Строительство';
update MetaPackages set OrderBy = '46' where Name = 'СТАТ_0017_Транспорт';
update MetaPackages set OrderBy = '47' where Name = 'СТАТ_0018_Связь';
update MetaPackages set OrderBy = '48' where Name = 'СТАТ_0019_Торговля';
update MetaPackages set OrderBy = '49' where Name = 'СТАТ_0020_Платные услуги';
update MetaPackages set OrderBy = '50' where Name = 'СТАТ_0021_Наука';
update MetaPackages set OrderBy = '51' where Name = 'СТАТ_0022_Финансы';
update MetaPackages set OrderBy = '52' where Name = 'СТАТ_0023_Инвестиции';
update MetaPackages set OrderBy = '53' where Name = 'СТАТ_0024_Цены';
update MetaPackages set OrderBy = '54' where Name = 'СТАТ_0025_ВЭД';
update MetaPackages set OrderBy = '55' where Name = 'СТАТ_0026_Приватизация';
update MetaPackages set OrderBy = '56' where Name = 'СТАТ_0027_Паспорт МО';
update MetaPackages set OrderBy = '57' where Name = 'УФК';
update MetaPackages set OrderBy = '58' where Name = '_Общие классификаторы_УФК';
update MetaPackages set OrderBy = '59' where Name = '_Общие классификаторы_УФК_Планирование';
update MetaPackages set OrderBy = '60' where Name = 'УФК_0001_Форма 16';
update MetaPackages set OrderBy = '61' where Name = 'УФК_0001_Форма 16_Планирование';
update MetaPackages set OrderBy = '62' where Name = 'УФК_0002_СводФУ';
update MetaPackages set OrderBy = '63' where Name = 'УФК_0002_СводФУ_Планирование';
update MetaPackages set OrderBy = '64' where Name = 'УФК_0003_Форма 14';
update MetaPackages set OrderBy = '65' where Name = 'УФК_0004_Аренда';
update MetaPackages set OrderBy = '66' where Name = 'УФК_0005_Доходы';
update MetaPackages set OrderBy = '67' where Name = 'УФК_0006_Форма 13';
update MetaPackages set OrderBy = '68' where Name = 'УФК_0007_1Н 7 Сводная ведомость поступлений (ежемесячная)';
update MetaPackages set OrderBy = '69' where Name = 'УФК_0008_1Н DP Реестр перечисленных поступлений';
update MetaPackages set OrderBy = '70' where Name = 'УФК_0009_Доходы_Результаты регулировки';
update MetaPackages set OrderBy = '71' where Name = 'ФК';
update MetaPackages set OrderBy = '72' where Name = 'ФК_0001_МесОтч';
update MetaPackages set OrderBy = '73' where Name = 'ФНС';
update MetaPackages set OrderBy = '74' where Name = 'ФНС_0001_28н';
update MetaPackages set OrderBy = '75' where Name = 'ФНС_0001_28н_Планирование';
update MetaPackages set OrderBy = '76' where Name = 'ФНС_0002_1 ОБЛ';
update MetaPackages set OrderBy = '77' where Name = 'ФНС_0003_1 НМ';
update MetaPackages set OrderBy = '78' where Name = 'ФНС_0004_5 НИО';
update MetaPackages set OrderBy = '79' where Name = 'ФНС_0005_5 ИБ';
update MetaPackages set OrderBy = '80' where Name = 'ФО';
update MetaPackages set OrderBy = '81' where Name = 'ФО_0001_АС Бюджет';
update MetaPackages set OrderBy = '82' where Name = 'Общие классификаторы';
update MetaPackages set OrderBy = '83' where Name = 'Доходы';
update MetaPackages set OrderBy = '84' where Name = 'План доходов';
update MetaPackages set OrderBy = '85' where Name = 'План расходов';
update MetaPackages set OrderBy = '86' where Name = 'Расходы казначейство';
update MetaPackages set OrderBy = '87' where Name = 'Взаимные расчеты_Средства полученные';
update MetaPackages set OrderBy = '88' where Name = 'Взаимные расчеты_Уведомления';
update MetaPackages set OrderBy = '89' where Name = 'Финансирование';
update MetaPackages set OrderBy = '90' where Name = 'Отчеты организаций';
update MetaPackages set OrderBy = '91' where Name = 'Операции со счетами';
update MetaPackages set OrderBy = '92' where Name = 'ФО_0001_АС Бюджет_Планирование';
update MetaPackages set OrderBy = '93' where Name = 'ФО_0002_МесОтч';
update MetaPackages set OrderBy = '94' where Name = 'ФО_0002_МесОтч_Планирование';
update MetaPackages set OrderBy = '95' where Name = 'ФО_0003_Проект доходов';
update MetaPackages set OrderBy = '96' where Name = 'ФО_0005_ГодОтч';
update MetaPackages set OrderBy = '97' where Name = 'ФО_0005_ГодОтч_Планирование';
update MetaPackages set OrderBy = '98' where Name = 'ФО_0007_Проект расходов';
update MetaPackages set OrderBy = '99' where Name = 'ФО_0008_Проект ИФ';
update MetaPackages set OrderBy = '100' where Name = 'ФО_0009_Фонды';
update MetaPackages set OrderBy = '101' where Name = 'ФО_0010_Поступление доходов';
update MetaPackages set OrderBy = '102' where Name = 'ФО_0011_Проект доходов_краткосрочное';
update MetaPackages set OrderBy = '103' where Name = 'ФО_0012_Проект доходов по объектам';
update MetaPackages set OrderBy = '104' where Name = 'ФО_0014_Предельные объемы бюджетного финансирования';
update MetaPackages set OrderBy = '105' where Name = 'ФО_0015_Показатели для планирования';
update MetaPackages set OrderBy = '106' where Name = 'ФО_0016_Мониторинг_БК_КУ';
update MetaPackages set OrderBy = '107' where Name = 'ЭО';
update MetaPackages set OrderBy = '108' where Name = 'ЭО_0001_Экономические показатели';
update MetaPackages set OrderBy = '109' where Name = 'ЭО_0003_СЭР';
update MetaPackages set OrderBy = '110' where Name = 'МЭРиТ';
update MetaPackages set OrderBy = '111' where Name = 'МО_Ленинградская область';
update MetaPackages set OrderBy = '112' where Name = 'Тест';
update MetaPackages set OrderBy = '113' where Name = 'Новый пакет 1';

commit;

/* End   -  - Сортировка пакетов - gbelov - 10.10.2006 */



/* Start   -  - Создание фиктивной задачи для закачки - gbelov - 14.11.2006 */

insert into Tasks (ID, State, Doer, Owner, Headline, Job, Curator, RefTasksTypes)
	values (-1, 'Создана', 3, 3, 'Закачка данных', 'Закачка данных', 3, 0);

commit;

/* End   -  - Создание фиктивной задачи для закачки - gbelov - 14.11.2006 */



/* Start   -  - Пароль 1 для пользователя "Закачка данных" - borisov - 14.11.2006 */

update Users set PwdHashSHA = 'Tf9Oo0DwqCPxXT9PAati6uDl2lecy4Ufjbnf6ExYsrN7iZA6dA4e4XLaeTpuedVg5ff5vQWKEqKAQz7W+kZRCg==' where ID = 3;

commit;

/* End   -  - Пароль 1 для пользователя "Закачка данных" - borisov - 14.11.2006 */



/* Start - 4386 - АДМИН_0003_Проект расходов - закачка - mik-a-el - 15.11.2006 */

insert into PUMPREGISTRY (SUPPLIERCODE, DATACODE, PROGRAMIDENTIFIER, NAME, PROGRAMCONFIG, STAGESPARAMS, SCHEDULE, COMMENTS)
values ('АДМИН', 0003, 'GRBSOutcomesProjectPump', 'Закачка проекта расходов',
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="Помещать обработанные файлы в папку архива." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="Individual">
		<Check Name="ucbFinalOverturn" Text="Закачка заключительных оборотов." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbDeleteEarlierData" Text="Удалять закачанные ранее данные из того же источника." LocationX="13" LocationY="20" Width="500" Height="20" Value="True"/>
	</ControlsGroup>
</DataPumpParams>',
'<?xml version="1.0" encoding="windows-1251"?>
	<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
		<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
		<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
		<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
	</DataPumpStagesParams>',
'<?xml version="1.0" encoding="windows-1251"?>
<PumpSchedule xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="PumpSchedule.xsd">
	<ScheduleSettings Enabled="false" StartDate="0" StartTime="0" Periodicity="Once"/>
</PumpSchedule>',
'Блок предназначен для сбора данных с ГРБС в Департаменте финансов Администрации Костромской области по финансированию из областного бюджета на следующий год. Данные предоставляются по каждому департаменту для последующего утверждения и корректировки. ');

commit;

/* End   - 4386 - АДМИН_0003_Проект расходов - закачка - mik-a-el - 15.11.2006 */


/* Start - 4457 - ФО_0001_АС Бюджет - версия 6.08 - адаптация - mik-a-el - 16.11.2006 */

update PUMPREGISTRY
set COMMENTS =
	'Поддерживаемые версии БД АС Бюджет: 27.02, 28.00, 29.01, 29.02, 30.01, 31.00, 31.01, 32.02, 32.04, 32.05, 32.07, 32.08, 32.09, 32.10, 33.00, 33.01, 33.02, 33.03, 34.00'
where PROGRAMIDENTIFIER = 'BudgetDataPump';

commit;

/* End   - 4457 - ФО_0001_АС Бюджет - версия 6.08 - адаптация - mik-a-el - 16.11.2006 */


insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (19, '2.2.0', SYSDATE, SYSDATE, '');

commit;

