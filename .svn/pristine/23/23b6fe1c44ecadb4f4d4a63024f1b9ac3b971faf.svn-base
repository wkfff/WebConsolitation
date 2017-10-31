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


/* Start 7855 - Удаление источников вместе с данными - avolkov - 19.05.2008 */

/* Добавляем в таблицу поле - признак удаления.*/
alter table HUB_DataSources
   add Deleted number(1) default 0 not null;

/* Добавляем это же поле в представление. */
create or replace view datasources
(id, suppliercode, datacode, dataname, kindsofparams, name, year, month, variant, quarter, territory, locked, deleted, datasourcename)
as
select
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory, Locked, Deleted,
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

/* Добавляем событие удаления источника */
insert into kindsofevents (ID, ClassOfEvent, Name) values (1018, 13, 'Удаление источника данных' );

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (46, '2.4.0.12', To_Date('19.05.2008', 'dd.mm.yyyy'), SYSDATE, 'Добавление поля - признака удаления источника в таблицу HUB_DataSources и представление DataSources; добавление события удаления источника.', 0);

commit;

/* End 7855 - Удаление источников вместе с данными - avolkov - 19.05.2008 */


