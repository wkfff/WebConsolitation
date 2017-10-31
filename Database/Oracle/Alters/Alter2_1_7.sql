/*******************************************************************
 Переводит базу Oracle из версии 2.1.7 в следующую версию 2.1.8
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */


/* Start - 2456 - Поле JOB в таблице TASKS не должно быть nullable - Борисов - 11.04.2006 */

update Tasks set Job = 'Не указано' where Job is null;
alter table Tasks modify Job not null;

commit;

/* End - 2456 - Поле JOB в таблице TASKS не должно быть nullable - Борисов - 11.04.2006 */



/* Start - 2527 - Фиксированный Период.День - заключительные обороты - gbelov - 18.04.2006 */

create or replace procedure sp_FillDate as
tmpMaxDay pls_integer;
tmpNewDate pls_integer;
tmpHalfYear varchar2(22);
tmpQuarter varchar2(22);
tmpMonthName varchar2(22);
begin
  --insert into fx_Date_YearDay (ID, Year, HalfYear, Quarter, Month, MonthName, Day) values (-1, 0, ' ', ' ', 0, ' ', 0);

	for tmpYear in 1998..2010 loop

  	for tmpMonth in 13..13 loop

      if tmpMonth = 0 then
         tmpHalfYear := 'Остатки на начало года';
      elsif tmpMonth = 13 then
         tmpHalfYear := 'Заключительные обороты';
      elsif tmpMonth > 6 then
         tmpHalfYear := 'Полугодие 2';
      else
         tmpHalfYear := 'Полугодие 1';
      end if;

      if tmpMonth = 0 then
         tmpQuarter := 'Остатки на начало года';
      elsif tmpMonth = 13 then
         tmpQuarter := 'Заключительные обороты';
      elsif tmpMonth < 4 then
         tmpQuarter := 'Квартал 1';
      elsif tmpMonth < 7 then
         tmpQuarter := 'Квартал 2';
      elsif tmpMonth < 10 then
         tmpQuarter := 'Квартал 3';
      else
         tmpQuarter := 'Квартал 4';
      end if;

	  tmpMaxDay := case tmpMonth
			when 1 then 31
			when 2 then 29
			when 3 then 31
			when 4 then 30
			when 5 then 31
			when 6 then 30
			when 7 then 31
			when 8 then 31
			when 9 then 30
			when 10 then 31
			when 11 then 30
			when 12 then 31
			else 0
		end;
	  tmpMonthName := case tmpMonth
			when 0 then 'Остатки на начало года'
			when 1 then 'Январь'
			when 2 then 'Февраль'
			when 3 then 'Март'
			when 4 then 'Апрель'
			when 5 then 'Май'
			when 6 then 'Июнь'
			when 7 then 'Июль'
			when 8 then 'Август'
			when 9 then 'Сентябрь'
			when 10 then 'Октябрь'
			when 11 then 'Ноябрь'
			when 12 then 'Декабрь'
			when 13 then 'Заключительные обороты'
			else '0'
		end;

      for tmpDay in 0..tmpMaxDay loop
        tmpNewDate := tmpYear * 10000 + tmpMonth * 100 + tmpDay;

            insert into fx_Date_YearDay (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth, DateDay)
            values (tmpNewDate, tmpYear, tmpHalfYear, tmpQuarter, tmpMonth, tmpMonthName, tmpDay);

      end loop;

	  end loop;

	end loop;

end sp_FillDate;
/

exec sp_FillDate;

commit;

create or replace procedure sp_FillDate as
tmpMaxDay pls_integer;
tmpNewDate pls_integer;
tmpHalfYear varchar2(22);
tmpQuarter varchar2(22);
tmpMonthName varchar2(22);
begin
	for tmpYear in 1998..2010 loop

  	for tmpMonth in 0..13 loop

      if tmpMonth = 0 then
         tmpHalfYear := 'Остатки на начало года';
      elsif tmpMonth = 13 then
         tmpHalfYear := 'Заключительные обороты';
      elsif tmpMonth > 6 then
         tmpHalfYear := 'Полугодие 2';
      else
         tmpHalfYear := 'Полугодие 1';
      end if;

      if tmpMonth = 0 then
         tmpQuarter := 'Остатки на начало года';
      elsif tmpMonth = 13 then
         tmpQuarter := 'Заключительные обороты';
      elsif tmpMonth < 4 then
         tmpQuarter := 'Квартал 1';
      elsif tmpMonth < 7 then
         tmpQuarter := 'Квартал 2';
      elsif tmpMonth < 10 then
         tmpQuarter := 'Квартал 3';
      else
         tmpQuarter := 'Квартал 4';
      end if;

	  tmpMaxDay := case tmpMonth
			when 1 then 31
			when 2 then 29
			when 3 then 31
			when 4 then 30
			when 5 then 31
			when 6 then 30
			when 7 then 31
			when 8 then 31
			when 9 then 30
			when 10 then 31
			when 11 then 30
			when 12 then 31
			else 0
		end;
	  tmpMonthName := case tmpMonth
			when 0 then 'Остатки на начало года'
			when 1 then 'Январь'
			when 2 then 'Февраль'
			when 3 then 'Март'
			when 4 then 'Апрель'
			when 5 then 'Май'
			when 6 then 'Июнь'
			when 7 then 'Июль'
			when 8 then 'Август'
			when 9 then 'Сентябрь'
			when 10 then 'Октябрь'
			when 11 then 'Ноябрь'
			when 12 then 'Декабрь'
			when 13 then 'Заключительные обороты'
			else '0'
		end;

      for tmpDay in 0..tmpMaxDay loop
        tmpNewDate := tmpYear * 10000 + tmpMonth * 100 + tmpDay;

            insert into fx_Date_YearDay (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth, DateDay)
            values (tmpNewDate, tmpYear, tmpHalfYear, tmpQuarter, tmpMonth, tmpMonthName, tmpDay);

      end loop;

	  end loop;

	end loop;

end sp_FillDate;
/

/* End - 2527 - Фиксированный Период.День - заключительные обороты - gbelov - 18.04.2006 */


/* Start - NoNumber - Поддержка закачки бюджета 33.00 - mik-a-el - 11.05.2006 */

update PUMPREGISTRY set
     COMMENTS = 'Поддерживаемые версии БД АС Бюджет: 27.02, 28.00, 29.01, 29.02, 30.01, 31.00, 31.01, 32.02, 32.04, 32.05, 32.07, 32.08, 32.09, 32.10, 33.00'
where PROGRAMIDENTIFIER = 'BudgetDataPump';

/* End - NoNumber - Поддержка закачки бюджета 33.00 - mik-a-el - 11.05.2006 */


/* Start - 2685 - Добавление в нормативы отчислений новых полей и переименование старых" - paluh - 17.05.2006 */

-- переименование полей
alter table DisintRules_KD
	rename column CONSMO_PERCENT to CONSMR_PERCENT;

alter table DisintRules_Ex
	rename column CONSMO_PERCENT to CONSMR_PERCENT;

alter table DisintRules_Ex
	rename column MO_PERCENT to MR_PERCENT;

alter table DisintRules_KD
	rename column MO_PERCENT to MR_PERCENT;

-- добавление полей
alter table DisintRules_KD
	add CONSMO_Percent number (5,2) default 0 not null;

alter table DisintRules_Ex
	add CONSMO_Percent number (5,2) default 0 not null;

alter table DisintRules_KD
	add GO_Percent number (5,2) default 0 not null;

alter table DisintRules_Ex
	add GO_Percent number (5,2) default 0 not null;

commit;

/* End   - 2685 - Добавление в нормативы отчислений новых полей и переименование старых" - paluh - 17.05.2006 */



/* Start - 2715 - Изменение Уровни бюджетов - обновление базы - paluh - 22.05.2006 */

-- Обновление базы. Пересчет значения поля, все остальные либо заполняются нулями либо просто переименованы с сохранением значения

update DisintRules_KD
	set CONSMO_Percent = CONSMR_Percent;

update DisintRules_Ex
	set CONSMO_Percent = CONSMR_Percent;

commit;

/* End   - 2715 - Изменение Уровни бюджетов - обновление базы - paluh - 22.05.2006 */



/* Start -  - Изменение описания этапов - mik-a-el - 22.05.2006 */

update PUMPREGISTRY
set STAGESPARAMS =
	'<?xml version="1.0" encoding="windows-1251"?>
	<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
		<PumpData State="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
		<ProcessData State="InQueue" Comment="Выполняется корректировка сумм фактов по иерархии классификаторов."/>
		<AssociateData State="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
		<ProcessCube State="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
		<CheckData State="InQueue" Comment="Никаких действий не выполняется."/>
	</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'SKIFMonthRepPump' or	PROGRAMIDENTIFIER = 'SKIFYearRepPump' or PROGRAMIDENTIFIER = 'FKMonthRepPump';

commit;

/* End -  - Изменение описания этапов - mik-a-el - 22.05.2006 */



/* Start -  - Добавляем каскадное удаление - gbelov - 22.05.2006 */

alter table MetaObjects
	drop constraint FKMetaObjectsRefPackages;

alter table MetaObjects
	add constraint FKMetaObjectsRefPackages foreign key ( RefPackages )
		references MetaPackages ( ID ) on delete cascade;

alter table MetaLinks
	drop constraint FKMetaLinksRefPackages;

alter table MetaLinks
	add constraint FKMetaLinksRefPackages foreign key ( RefPackages )
		references MetaPackages ( ID ) on delete cascade;

/* Ссылка на ассоциацию к которой привязана таблица перекодировок */
alter table MetaConversionTable
	add RefAssociation number (10);

alter table MetaConversionTable
	add constraint FKMetaCTRefAssociation foreign key ( RefAssociation )
		references MetaLinks ( ID ) on delete cascade;

declare
	LinkID pls_integer;
begin
	-- для каждой таблицы перекодировок по значениям полей Semantic и Association
	-- ищем ID соответствующие ассоциации и записываем его в RefAssociation
	for CT in (select ID, Association from MetaConversionTable)
	loop
		begin
			select ID into LinkID from MetaLinks where ('a.' || Semantic || '.' || Name) = CT.Association and Class = 2;
			update MetaConversionTable set RefAssociation = LinkID where ID = CT.ID;
		exception
			when No_Data_Found then
				null;
		end;
	end loop;
end;
/

alter table MetaConversionTable
	modify RefAssociation not null;

alter table MetaConversionTable
	drop column Association;

alter table MetaConversionTable
	drop column Semantic;

/* End -  - Добавляем каскадное удаление - gbelov - 22.05.2006 */


insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (13, '2.1.8', SYSDATE, SYSDATE, '');

commit;
