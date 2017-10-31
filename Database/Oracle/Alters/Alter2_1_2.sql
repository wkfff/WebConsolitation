/*******************************************************************
 Переводит базу Oracle из версии 2.1.2 в следующую версию 2.1.3
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */


/* Start - 1506 - Запуск закачек по расписанию - kutnyashenko - 06.02.2006 */

alter table PUMPREGISTRY drop column PUMPDATE;

alter table PUMPREGISTRY drop column JOBNAME;

alter table PUMPREGISTRY drop column ISRUNNING;

alter table PUMPREGISTRY drop column LOCALMACHINENAME;

alter table PUMPREGISTRY add SCHEDULE CLOB;

commit;

update PUMPREGISTRY set SCHEDULE =
'<?xml version="1.0" encoding="windows-1251"?>
<PumpSchedule xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="PumpSchedule.xsd">
	<ScheduleSettings Enabled="false" StartDate="0" StartTime="0" Periodicity="Once"/>
</PumpSchedule>';

commit;

/* End - 1506 - Запуск закачек по расписанию - kutnyashenko - 06.02.2006 */



/* Start - 2028 - Расширение фик. классификатора "Период.Месяц" до 98,99 годов - gbelov - 08.02.2006 */

create or replace procedure sp_FillDate as
tmpMaxDay pls_integer;
tmpNewDate pls_integer;
tmpHalfYear varchar2(22);
tmpQuarter varchar2(22);
tmpMonthName varchar2(22);
begin
  --insert into fx_Date_YearDay (ID, Year, HalfYear, Quarter, Month, MonthName, Day) values (-1, 0, ' ', ' ', 0, ' ', 0);

	for tmpYear in 1998..1999 loop

  	for tmpMonth in 0..12 loop

      if tmpMonth = 0 then
         tmpHalfYear := 'Остатки на начало года';
      elsif tmpMonth > 6 then
         tmpHalfYear := 'Полугодие 2';
      else
         tmpHalfYear := 'Полугодие 1';
      end if;

      if tmpMonth = 0 then
         tmpQuarter := 'Остатки на начало года';
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
			else '0'
		end;

      for tmpDay in 0..tmpMaxDay loop
        tmpNewDate := tmpYear * 10000 + tmpMonth * 100 + tmpDay;

            insert into fx_Date_YearDay (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth, DateDay)
            values (tmpNewDate, tmpYear, tmpHalfYear, tmpQuarter, tmpMonth, tmpMonthName, tmpDay);

/*        	insert into HUB_DataCls (ID, TypeID)
        	values (tmpNewDate, 1);
			insert into SAT_DataCls (ID, TypeID, Code1, Code2, Code3, Name1, Name2, Name3)
		    values (tmpNewDate, 1, tmpYear, tmpMonth, tmpDay, tmpHalfYear, tmpQuarter, tmpMonthName);*/
      end loop;

	  end loop;

	end loop;

end sp_FillDate;
/

exec sp_FillDate;

create or replace procedure sp_FillDateMonth as
tmpNewDate pls_integer;
tmpHalfYear varchar2(22);
tmpQuarterNo pls_integer;
tmpQuarter varchar2(22);
tmpMonthName varchar2(22);
begin
  --insert into fx_Date_YearMonth (ID, Year, HalfYear, Quarter, Month, MonthName) values (-1, 0, ' ', ' ', 0, ' ');

  for tmpYear in 1998..1999 loop

  	for tmpMonth in 0..12 loop

      if tmpMonth = 0 then
         tmpHalfYear := 'Остатки на начало года';
      elsif tmpMonth > 6 then
         tmpHalfYear := 'Полугодие 2';
      else
         tmpHalfYear := 'Полугодие 1';
      end if;

      if tmpMonth = 0 then
         tmpQuarterNo := 0;
         tmpQuarter := 'Остатки на начало года';
      elsif tmpMonth < 4 then
         tmpQuarterNo := 1;
         tmpQuarter := 'Квартал 1';
      elsif tmpMonth < 7 then
         tmpQuarterNo := 2;
         tmpQuarter := 'Квартал 2';
      elsif tmpMonth < 10 then
         tmpQuarterNo := 3;
         tmpQuarter := 'Квартал 3';
      else
         tmpQuarterNo := 4;
         tmpQuarter := 'Квартал 4';
      end if;

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
			else '0'
      end;

	  if tmpMonth in (1, 4, 7, 10)  then

        tmpNewDate := tmpYear * 10000 + 9990 + tmpQuarterNo;

        insert into fx_Date_YearMonth (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth) values (tmpNewDate, tmpYear, tmpHalfYear, 'Квартал ' || tmpQuarterNo, 90 + tmpQuarterNo, 'Квартал ' || tmpQuarterNo);

	  end if;

      tmpNewDate := tmpYear * 10000 + tmpMonth * 100;

      insert into fx_Date_YearMonth (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth) values (tmpNewDate, tmpYear, tmpHalfYear, tmpQuarter, tmpMonth, tmpMonthName);

	end loop;

  end loop;

end sp_FillDateMonth;
/

exec sp_FillDateMonth;

create or replace procedure sp_FillDateYear as
begin
    --insert into fx_Date_Year (ID, Year) values (-1, 0);
	for tmpYear in 1998..1999 loop
        insert into fx_Date_Year (ID, DateYear) values (tmpYear, tmpYear);
	end loop;
end sp_FillDateYear;
/

exec sp_FillDateYear;

commit;

/* End - 2028 - Расширение фик. классификатора "Период.Месяц" до 98,99 годов - gbelov - 08.02.2006 */

/* Begin - 2059 - Протоколирование операций интерфейса администрирования - borisov - 09.02.2006 */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40301, 0, 'Изменен список пользователей');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40302, 0, 'Изменен список групп');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40303, 0, 'Изменен справочник отделов');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40304, 0, 'Изменен справочник организаций');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40305, 0, 'Изменен справочник типов задач');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40306, 0, 'Изменено членство в группах');
/* End - 2059 - Протоколирование операций интерфейса администрирования - borisov - 09.02.2006 */

/* Begin - 2058 - Хранение списка объектов просмотра в базе - borisov - 10.02.2006 */
create table RegisteredUIModules
(
		ID					number (10) not null,			 /* PK */
		Name				varchar2 (255) not null,		 /* Имя объекта просмотра */
		Description			varchar2 (500),		 /* Описание объекта просмотра */
		constraint PKRegisteredUIModules primary key ( ID ), /* ID Должно быть уникальным */
		constraint UKRegisteredUIModules unique ( Name ) /* Имя должно быть уникальным */
);

insert into RegisteredUIModules (ID, Name, Description) values (0,   'AdministrationUI', 		'Интерфейс администрирования');
insert into RegisteredUIModules (ID, Name, Description) values (10,  'ProtocolsViewObject', 	'Протоколы');
insert into RegisteredUIModules (ID, Name, Description) values (20,  'FixedClsUI',	 			'Интерфейс фиксированных классификаторов');
insert into RegisteredUIModules (ID, Name, Description) values (30,  'DataClsUI', 				'Интерфейс классификаторов данных');
insert into RegisteredUIModules (ID, Name, Description) values (40,  'AssociatedClsUI', 		'Интерфейс сопоставимых классификаторов');
insert into RegisteredUIModules (ID, Name, Description) values (50,  'AssociationUI', 			'Интерфейс сопоставления классификаторов');
insert into RegisteredUIModules (ID, Name, Description) values (60,  'TranslationsTablesUI', 	'Интерфейс таблиц перекодировок');
insert into RegisteredUIModules (ID, Name, Description) values (70,  'DataPumpUI', 				'Интерфейс закачки данных');
insert into RegisteredUIModules (ID, Name, Description) values (80,  'DataSourcesUI', 			'Интерфейс источников данных');
insert into RegisteredUIModules (ID, Name, Description) values (90,  'DisintRulesUI', 			'Интерфейс нормативов отчисления доходов');
insert into RegisteredUIModules (ID, Name, Description) values (100, 'TasksViewObj', 			'Интерфейс задач');
insert into RegisteredUIModules (ID, Name, Description) values (110, 'FactTablesUI', 			'Интерфейс таблиц фактов');
insert into RegisteredUIModules (ID, Name, Description) values (120, 'AdminConsole', 			'Интерфейс управления схемой');

commit;

/* End - 2058 - Хранение списка объектов просмотра в базе - borisov - 10.02.2006 */


insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (8, '2.1.3', SYSDATE, SYSDATE, '');

commit;






