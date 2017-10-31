/*******************************************************************
 Переводит базу SQLиз версии 3.0 в следующую версию 3.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */

/* Start - Bug #18731 Количество символов в имени элементов измерения Источники данных - zaharchenko - 17.01.2012 */

drop view DataSources

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
		END) as varchar(1024))
from HUB_DataSources;

go

/* End - Bug #18731 Количество символов в имени элементов измерения Источники данных - zaharchenko - 17.01.2012 */