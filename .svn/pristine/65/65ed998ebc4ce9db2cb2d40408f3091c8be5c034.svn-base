/*******************************************************************
 Переводит базу SqlServer из версии  в следующую версию
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */

/* Start - 1176 - Источники по которым ввод - закрывать от записи - avolkov - 06.02.2008 */

/* Добавляем в таблицу поле - признак блокировки.*/
alter table HUB_DataSources
   add locked NUMERIC(1) default 0 not null;

go

/* Добавляем это же поле в представление. */
drop view DataSources;

go

create view DataSources (
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory, Locked, DataSourceName
) as
select
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory, Locked,
	SupplierCode + ' ' + DataName + cast((
		CASE KindsOfParams
			WHEN 8 THEN ''
			ELSE ' - ' +
			CASE KindsOfParams
				WHEN 0 THEN Name + ' ' + cast(Year as varchar(4))
				WHEN 1 THEN cast(Year as varchar(4))
				WHEN 2 THEN cast(Year as varchar(4)) + ' ' + cast(Month as varchar(2))
				WHEN 3 THEN cast(Year as varchar(4)) + ' ' + cast(Month as varchar(2)) + ' ' + Variant
				WHEN 4 THEN cast(Year as varchar(4)) + ' ' + Variant
				WHEN 5 THEN cast(Year as varchar(4)) + ' ' + cast(Quarter as varchar(1))
				WHEN 6 THEN cast(Year as varchar(4)) + ' ' + Territory
				WHEN 7 THEN cast(Year as varchar(4)) + ' ' + cast(Quarter as varchar(1)) + ' ' + cast(Month as varchar(2))
				WHEN 9 THEN Variant
			END
		END) as varchar)
from HUB_DataSources;

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
CREATE TRIGGER T_DATASOURCESOPERATIONS_D on DataSourcesOperations instead of delete
as
	delete from HUB_EventProtocol where ID in (select ID from deleted);

go

CREATE TRIGGER T_DATASOURCESOPERATIONS_I on DataSourcesOperations instead of insert
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

CREATE TRIGGER T_DATASOURCESOPERATIONS_U on DataSourcesOperations instead of update
as
	RAISERROR (90001, 1, 1);

go

/* Добавляем типы событий блокировки и открытия источника */
insert into kindsofevents (ID, ClassOfEvent, Name) values (1016, 13, 'Блокировка источника данных' );

go

insert into kindsofevents (ID, ClassOfEvent, Name) values (1017, 13, 'Открытие источника данных' );

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (37, '2.4.0.3', CONVERT(datetime, '2007.02.06', 102), GETDATE(), 'Добавление поля - признака блокировки источника в таблицу HUB_DataSources и представление DataSources; добавление события блокировки источника.', 0);

go

/* End 1176 - Источники по которым ввод - закрывать от записи - avolkov - 06.02.2008 */
