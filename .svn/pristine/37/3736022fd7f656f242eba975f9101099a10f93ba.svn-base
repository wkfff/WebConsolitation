/*******************************************************************
 Переводит DV-базу Oracle из версии 2.0.1 в следующую версию 2.0.2
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */

/* Start - Нет - На каждое правило сопоставления своя таблица перекодировок - gbelov - 10.10.05 19:09 */

whenever SQLError continue commit;
/* Возможно уже выполнено в патче */
delete from MetaConversionTable;

alter table MetaConversionTable
	add Rule varchar2 (50) not null;

commit;

whenever SQLError exit rollback;

/* End - Нет - На каждое правило сопоставления своя таблица перекодировок - gbelov - 10.10.05 19:09 */

/* Start - Нет - Вьюшки с укороченными наименования для построения кубов. - gbelov - 18.10.05 13:59 */

whenever SQLError continue commit;

create or replace view dv_KD_MonthRep2005 as select T.*, cast(T.Name as varchar2(255)) as ShortName from d_KD_MonthRep2005 T;
create or replace view dv_KD_MonthRep2004 as select T.*, cast(T.Name as varchar2(255)) as ShortName from d_KD_MonthRep2004 T;

whenever SQLError exit rollback;

/* End - Нет - Вьюшки с укороченными наименования для построения кубов. - gbelov - 18.10.05 13:59 */

commit;

/* Start - 1180 - Во все логи, где нет полей sourceID, pumpID, нужно их добавить. - paluh - 19.10.05 */

/* Изменения на протокол "Расчет многомерной базы" */
alter table SAT_MDProcessing
    	add PumpHistoryID number (10);

alter table SAT_MDProcessing
    	add DataSourceID number (10);

create or replace view MDProcessing
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations,
	MDObjectName, PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations,
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
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 3);

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

/* Изменение на протокол "Расчет многомерной базы" */
alter table SAT_BridgeOperations
    	add PumpHistoryID number (10);

alter table SAT_BridgeOperations
    	add DataSourceID number (10);

create or replace view BridgeOperations
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations,
	BridgeRoleA, BridgeRoleB, PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations,
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
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 2);

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

/* Изменение на протокол "Сверки данных" */
alter table SAT_ReviseData
    	add PumpHistoryID number (10);

alter table SAT_ReviseData
    		add DataSourceID number (10);

create or replace view ReviseDataProtocol
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, PumpHistoryID, DataSourceID
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations,
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
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 4);

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

commit;


/* End  - 1180 - Во все логи, где нет полей sourceID, pumpID, нужно их добавить. - paluh - 19.10.05 */

/* Start - 1236 - Добавление в нормативы отчислений нового поля "Отчисления в областной бюджет Тюменской обл" - paluh - 26.10.2005 */

alter table DisintRules_KD
    	add TumenAccount_Percent number (5,2) default 0 not null;

alter table DisintRules_Ex
    	add TumenAccount_Percent number (5,2) default 0 not null;

commit;

/* End   - 1236 - Добавление в нормативы отчислений нового поля "Отчисления в областной бюджет Тюменской обл" - paluh - 26.10.2005 */


/* Start -  - Вьюшка с укороченным наименованием - gbelov - 27.10.05 */

whenever SQLError continue commit;

create or replace view dv_kd_PlanIncomes as select T.*, cast(T.Name as varchar2(255)) as ShortName from d_kd_PlanIncomes T;
create or replace view dv_kd_analysis as select T.*, cast(T.Name as varchar2(255)) as ShortName from d_kd_analysis T;

commit;

whenever SQLError exit rollback;

/* End   -  - Вьюшка с укороченным наименованием - gbelov - 27.10.05 */

/* Start -  - Дополнительные поля для разбиения кода - gbelov - 27.10.05 */

alter table SAT_BridgeCls
	add (Code10 number (5), Code11 number (5));

alter table SAT_DataCls
	add (Code10 number (5), Code11 number (5));

commit;

/* End -  - Дополнительные поля для разбиения кода - gbelov - 27.10.05 */

insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (3, '2.0.2', SYSDATE, SYSDATE, '');

commit;
