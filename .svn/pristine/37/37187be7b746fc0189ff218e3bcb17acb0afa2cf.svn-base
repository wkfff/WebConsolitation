/********************************************************************
    Переводит базу MS SQL Server из версии 3.0 в следующую версию 3.x 
********************************************************************/

/* Start - #14339 - Добавлены протоколы для функции перевода базы а новый год - tsvetkov - 24.10.2011 */

insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1101, 0, 'Создание источника данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1102, 0, 'Экспорт классификатора данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1103, 0, 'Импорт классификатора данных');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1104, 0, 'Требование на расчет измерения');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1105, 0, 'Требование на расчет куба');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1106, 2, 'Предупреждение');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1107, 3, 'Ошибка');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1108, 1, 'Начало работы функции перехода на новый год');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (1109, 1, 'Окончание работы функции перехода на новый год');

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


go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (107, '3.0.0.11', CONVERT(datetime, '2011.11.07', 102), GETDATE(), 'Добавлены протоколы для функции перевода базы а новый год', 0);

go

/* End - #14339 - Добавлены протоколы для функции перевода базы а новый год - tsvetkov - 24.10.2011 */
