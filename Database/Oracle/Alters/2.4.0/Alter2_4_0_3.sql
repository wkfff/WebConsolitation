/*******************************************************************
 Переводит базу Oracle из версии 2.X.X в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */


/* Start - 1176 - Источники по которым ввод - закрывать от записи - avolkov - 06.02.2008 */

whenever SQLError continue commit;

/* Добавляем в таблицу поле - признак блокировки.*/
alter table HUB_DataSources
   add locked number(1) default 0 not null;

/* Добавляем это же поле в представление. */
create or replace view datasources
(id, suppliercode, datacode, dataname, kindsofparams, name, year, month, variant, quarter, territory, locked, datasourcename)
as
select
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory, Locked,
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
					WHEN 9 THEN Variant
				END
		END
from HUB_DataSources;

commit;

/* Создаем таблицу для протокола операций над источниками. */
create table SAT_DataSourcesOperations
(
	ID					number (10) not null,
	DataSourceID		number (10) not null,
	constraint PKSAT_DataSourcesOperations primary key ( ID ),
	constraint FKSAT_DataSourcesOperations foreign key ( ID )
		references HUB_EventProtocol (ID) on delete cascade
);

commit;

/* И представление для них */
create or replace view DataSourcesOperations (ID, EventDateTime, module, kindsOfEvents, infoMessage, refUsersOperations, sessionID, datasourceID)
as
  select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.Sessionid,
	SAT.DataSourceID
from HUB_EventProtocol HUB join SAT_DataSourcesOperations SAT on (HUB.ID = SAT.ID);

commit;

/* Создаем триггеры для представления*/
CREATE OR REPLACE TRIGGER T_DATASOURCESOPERATIONS_D instead of delete on DataSourcesOperations
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end T_DATASOURCESOPERATIONS_D;
/

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

CREATE OR REPLACE TRIGGER T_DATASOURCESOPERATIONS_U instead of update on DataSourcesOperations
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end T_DATASOURCESOPERATIONS_U;
/

/* Добавляем типы событий блокировки и открытия источника */
insert into kindsofevents (ID, ClassOfEvent, Name) values (1016, 0, 'Блокировка источника данных' );

commit;

insert into kindsofevents (ID, ClassOfEvent, Name) values (1017, 0, 'Открытие источника данных' );

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (37, '2.4.0.3', To_Date('06.02.2008', 'dd.mm.yyyy'), SYSDATE, 'Добавление поля - признака блокировки источника в таблицу HUB_DataSources и представление DataSources; Добавление события блокировки источника.', 0);

commit;

whenever SQLError exit rollback;

/* End 1176 - Источники по которым ввод - закрывать от записи - avolkov - 06.02.2008 */


