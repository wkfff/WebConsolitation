/*******************************************************************
 Переводит базу SqlServer из версии  в следующую версию
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */

/* Start - 7855 - Удаление источников вместе с данными - avolkov - 19.05.2008 */

/* Добавляем в таблицу поле - признак удаления.*/
alter table HUB_DataSources
   add Deleted NUMERIC(1) default 0 not null;

go

/* Добавляем это же поле в представление. */
drop view DataSources;

go

create view DataSources (
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory, Locked, Deleted, DataSourceName
) as
select
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory, Locked, Deleted,
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

/* Добавляем типы события удаления источника */
insert into kindsofevents (ID, ClassOfEvent, Name) values (1018, 13, 'Удаление источника данных' );

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (46, '2.4.0.12', CONVERT(datetime, '2008.05.19', 102), GETDATE(), 'Добавление поля - признака удаления источника в таблицу HUB_DataSources и представление DataSources; добавление события удаления источника.', 0);

go

/* End - 7855 - Удаление источников вместе с данными - avolkov - 19.05.2008 */



