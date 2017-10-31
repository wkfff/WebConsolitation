/*******************************************************************
 Переводит базу Oracle из версии 2.1.10 в следующую версию 2.1.11
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */


/* Start -  - Добавляем документы в метаданные - gbelov - 04.07.2006 */

whenever SQLError continue commit;

/* Документы */
create table MetaDocuments
(
	ID					number (10) not null,
	Name				varchar2 (100) not null,	/* Наименование документа */
	DocType				number (10) not null,		/* Тип документа */
	RefPackages			number (10) not null,		/* родительский пакет */
	Configuration		CLOB,						/* XML настройка элемента, часть XML=UML */
	constraint PKMetaDocuments primary key ( ID ),
	constraint FKMetaDocumentsRefPackages foreign key ( RefPackages )
		references MetaPackages ( ID ) on delete cascade
);

create sequence g_MetaDocuments;

whenever SQLError exit rollback;

/* End -  - Добавляем документы в метаданные - gbelov - 04.07.2006 */

/* Start - 2722 - Параметры в задачах - borisov - 06.07.2006 */

/* Таблица параметров задач*/
create table TasksParameters
(
	ID					number (10) not null,		/* PK */
	Name				varchar2 (1000) not null,	/* Название*/
	Dimension			varchar2 (1000),			/* Измерение и семантика*/
	AllowMultiSelect	number (1) default 0 not null, /* Разрешен ли множеств.выбор (0 - нет, 1 - да) */
	Description			varchar2 (2000),			/* Комментарий */
	ParamValues			CLOB,						/* Значение параметра */
	ParamType			number(1) default 0 not null, /* Тип параметра (0 - параметр, 1 - константа)*/
	RefTasks			number (10) not null,		/* Ссылка на задачу */
	constraint PKTasksParameters primary key ( ID ),
	constraint FKTasksParametersRefTasks foreign key ( RefTasks )
		references Tasks ( ID ) on delete cascade
);

/* Последовательность для генерации ID параметров */
create sequence g_TasksParameters;

/* Триггер на вставку в таблицу параметров задач */
create or replace trigger t_TasksParameters_bi before insert on TasksParameters for each row
begin
	if :new.ID is null then select g_TasksParameters.NextVal into :new.ID from Dual; end if;
end t_TasksParameters_bi;
/

/* Таблица-кэш для параметров задач */
create table TasksParametersTemp
(
	ID					number (10) not null,		/* PK */
	Name				varchar2 (1000) not null,	/* Название*/
	Dimension			varchar2 (1000),			/* Измерение и семантика*/
	AllowMultiSelect	number (1) default 0 not null, /* Разрешен ли множеств.выбор (0 - нет, 1 - да) */
	Description			varchar2 (2000),			/* Комментарий */
	ParamValues			CLOB,						/* Значение параметра */
	ParamType			number(1) default 0 not null, /* Тип параметра (0 - параметр, 1 - константа)*/
	RefTasks			number (10) not null,		/* Ссылка на задачу */
	constraint PKTasksParametersTemp primary key ( ID ),
	constraint FKTsksPrmtrsTmpRefTsksTmp foreign key ( RefTasks )
		references TasksTemp ( ID ) on delete cascade
);

/* Триггер на вставку в таблицу-кэш параметров задач */
create or replace trigger t_TasksParametersTemp_bi before insert on TasksParametersTemp for each row
begin
	if :new.ID is null then select g_TasksParameters.NextVal into :new.ID from Dual; end if;
end t_TasksParametersTemp_bi;
/

/* Процедура начала изменения задачи - делает копию записи в таблице временных записей, */
/* устанавливает ссылку на заблокировавшего пользователя */
create or replace procedure sp_BeginTaskUpdate(taskID in Tasks.ID%Type, userID in Users.ID%Type, CAct varchar2, CSt varchar2) as
  lockByUser Tasks.LockByUser%Type;
begin
	-- проверим не находится ли задача в режиме обновления
	select LockByUser into lockByUser from Tasks where ID = taskID;
	if not (lockByUser is null) then
		raise_application_error(-20800, 'Задача уже находится в режиме редактирования');
	end if;

	-- устанавливаем значение поля LockByUser в основной таблице
	update Tasks set LockByUser = userID where ID = taskID;

	-- копируем содержимое задачи во временную таблицу
	insert into TasksTemp
		(ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes, LockByUser)
	select
		ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes, LockByUser
	from Tasks
	where ID = taskID;

	-- устанавливаем значения текущего действия и состояния задачи
	update TasksTemp set CAction = CAct, CState = CSt where ID = taskID;

	-- копируем документы задачи во временную таблицу
	insert into DocumentsTemp
		(ID, RefTasksTemp, Name, SourceFileName, DocumentType, IsPublic, Version, Document, Description, Ownership)
	select
		ID, RefTasks, Name, SourceFileName, DocumentType, IsPublic, Version, Document, Description, Ownership
	from Documents
	where RefTasks = taskID;

	-- копируем параметры во временную таблицу
	insert into TasksParametersTemp
		(ID, Name, Dimension, AllowMultiSelect, Description, ParamValues, ParamType, RefTasks)
	select
		ID, Name, Dimension, AllowMultiSelect, Description, ParamValues, ParamType, RefTasks
	from TasksParameters
	where RefTasks = taskID;

end sp_BeginTaskUpdate;
/

/* Процедура сохранения/отката изменений задачи - в зависимости от параметров обновляет */
/* запись в основной таблице, удаляет копию записи в таблице временных записей, */
/* обнуляет ссылку на заблокировавшего пользователя */
create or replace procedure sp_EndTaskUpdate(taskID in Tasks.ID%Type, canceled in number) as
    recCount number;
  	lockByUser Tasks.LockByUser%Type;
	newState Tasks.State%Type;
	newFromDate Tasks.FromDate%Type;
	newToDate Tasks.ToDate%Type;
	newDoer Tasks.Doer%Type;
	newOwner Tasks.Owner%Type;
	newCurator Tasks.Curator%Type;
	newHeadline Tasks.Headline%Type;
	newJob Tasks.Job%Type;
	newDescription Tasks.Description%Type;
	newRefTasks Tasks.RefTasks%Type;
	newRefTasksTypes Tasks.RefTasksTypes%Type;
begin
	-- смотрим была ли такая запись в основной таблице
	select count(ID) into recCount from Tasks where ID = taskID;

	-- если действие не отменено - обновляем запись в основной таблице
	if canceled = 0 then

		-- в зависимости от наличия вставляем/обновляем запись
		if (recCount = 0) then
			insert into Tasks
				(ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes)
			select
				ID, State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes
			from TasksTemp
			where ID = taskID;
		else
    		-- получаем состояния полей
	      	select
	       		State, FromDate, ToDate, Doer, Owner, Curator, Headline, Job, Description, RefTasks, RefTasksTypes
    		into
    			newState, newFromDate, newToDate, newDoer, newOwner, newCurator, newHeadline, newJob, newDescription, newRefTasks, newRefTasksTypes
	       	from
		      	TasksTemp
    		where
	       		ID = taskID;
			update Tasks
			set
				State = newState, FromDate = newFromDate, ToDate = newToDate, Doer = newDoer,
				Owner = newOwner, Curator = newCurator, Headline = newHeadline, Job = newJob,
				Description = newDescription, RefTasks = newRefTasks, RefTasksTypes = newRefTasksTypes
			where
				ID = taskID;
		end if; -- if (recCount = 0)

		-- переносим документы из временной таблицы в основную
		-- удаляем что было
		delete from Documents where RefTasks = taskID;

		-- переносим что стало
		insert into Documents
			(ID, RefTasks, Name, SourceFileName, DocumentType, IsPublic, Version, Document, Description, Ownership)
		select
			ID, RefTasksTemp, Name, SourceFileName, DocumentType, IsPublic, Version, Document, Description, Ownership
		from DocumentsTemp
		where RefTasksTemp = taskID;

		-- переносим параметры из временной таблицы в основную
		-- удаляем что было
		delete from TasksParameters where RefTasks = taskID;

		-- переносим что стало
		insert into TasksParameters
			(ID, Name, Dimension, AllowMultiSelect, Description, ParamValues, ParamType, RefTasks)
		select
			ID, Name, Dimension, AllowMultiSelect, Description, ParamValues, ParamType, RefTasks
		from TasksParametersTemp
		where RefTasks = taskID;

	end if; -- if canceled = 0

	-- обнуляем поле LockByUser в основной таблице
	update Tasks set LockByUser = null where ID = taskID;

	-- удаляем запись из временной таблицы задач
	delete from TasksTemp where ID = taskID;

	-- удаляем закэшированные документы
	delete from DocumentsTemp where RefTasksTemp = taskID;

	-- удаляем закэшированные параметры
	delete from TasksParametersTemp where RefTasks = taskID;

end sp_EndTaskUpdate;
/

/* End - 2722 - Параметры в задачах - borisov - 06.07.2006 */



/* Start - 3213 - Заключительные обороты в классификаторе Период.День сделать вместо 13 месяца 32 декабря - gbelov - 19.07.2006 */

whenever SQLError continue commit;

delete from d_date_conversionfk;

whenever SQLError exit rollback;

update fx_date_yearday set DateMonthID = 12, DateDay = 32 where DateMonth = 'Заключительные обороты';

update fx_date_yearday set ID = 19981232, DateHalfYear = 'Полугодие 2', DateQuarter = 'Квартал 4', DateMonth = 'Декабрь' where ID = 19981300;
update fx_date_yearday set ID = 19991232, DateHalfYear = 'Полугодие 2', DateQuarter = 'Квартал 4', DateMonth = 'Декабрь' where ID = 19991300;
update fx_date_yearday set ID = 20001232, DateHalfYear = 'Полугодие 2', DateQuarter = 'Квартал 4', DateMonth = 'Декабрь' where ID = 20001300;
update fx_date_yearday set ID = 20011232, DateHalfYear = 'Полугодие 2', DateQuarter = 'Квартал 4', DateMonth = 'Декабрь' where ID = 20011300;
update fx_date_yearday set ID = 20021232, DateHalfYear = 'Полугодие 2', DateQuarter = 'Квартал 4', DateMonth = 'Декабрь' where ID = 20021300;
update fx_date_yearday set ID = 20031232, DateHalfYear = 'Полугодие 2', DateQuarter = 'Квартал 4', DateMonth = 'Декабрь' where ID = 20031300;
update fx_date_yearday set ID = 20041232, DateHalfYear = 'Полугодие 2', DateQuarter = 'Квартал 4', DateMonth = 'Декабрь' where ID = 20041300;
update fx_date_yearday set ID = 20051232, DateHalfYear = 'Полугодие 2', DateQuarter = 'Квартал 4', DateMonth = 'Декабрь' where ID = 20051300;
update fx_date_yearday set ID = 20061232, DateHalfYear = 'Полугодие 2', DateQuarter = 'Квартал 4', DateMonth = 'Декабрь' where ID = 20061300;
update fx_date_yearday set ID = 20071232, DateHalfYear = 'Полугодие 2', DateQuarter = 'Квартал 4', DateMonth = 'Декабрь' where ID = 20071300;
update fx_date_yearday set ID = 20081232, DateHalfYear = 'Полугодие 2', DateQuarter = 'Квартал 4', DateMonth = 'Декабрь' where ID = 20081300;
update fx_date_yearday set ID = 20091232, DateHalfYear = 'Полугодие 2', DateQuarter = 'Квартал 4', DateMonth = 'Декабрь' where ID = 20091300;
update fx_date_yearday set ID = 20101232, DateHalfYear = 'Полугодие 2', DateQuarter = 'Квартал 4', DateMonth = 'Декабрь' where ID = 20101300;

create or replace procedure sp_FillDate as
tmpMaxDay pls_integer;
tmpNewDate pls_integer;
tmpHalfYear varchar2(22);
tmpQuarter varchar2(22);
tmpMonthName varchar2(22);
begin
	for tmpYear in 1998..2010 loop

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
			when 12 then 32
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

      		/*if (tmpDay = 32) and (tmpMonth = 12) then
      			tmpHalfYear := 'Заключительные обороты';
      			tmpQuarter := 'Заключительные обороты';
      			tmpMonthName := 'Заключительные обороты';
      		end if;*/

      		tmpNewDate := tmpYear * 10000 + tmpMonth * 100 + tmpDay;

            insert into fx_Date_YearDay (ID, DateYear, DateHalfYear, DateQuarter, DateMonthID, DateMonth, DateDay)
            values (tmpNewDate, tmpYear, tmpHalfYear, tmpQuarter, tmpMonth, tmpMonthName, tmpDay);

      end loop;

	  end loop;

	end loop;

end sp_FillDate;
/

whenever SQLError continue commit;

/* Заполняет классификатор соотвествие дней */
create or replace procedure sp_ConversionFKDate as
tmpMaxDay pls_integer;
tmpNewDate pls_integer;
tmpFODate pls_integer;
tmpFOYear pls_integer;
tmpFOMonth pls_integer;
tmpFODay pls_integer;
tmpDayOfWeek pls_integer;
begin
  tmpDayOfWeek := 4;
	for tmpYear in 1998..2010 loop

  	for tmpMonth in 0..12 loop

	  tmpMaxDay := case tmpMonth
			when 1 then 31
			when 2 then 28
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

      if (MOD(tmpYear, 4) = 0) and (tmpMonth = 2) then
        tmpMaxDay := 29;
      end if;

      for tmpDay in 0..tmpMaxDay loop
        tmpNewDate := tmpYear * 10000 + tmpMonth * 100 + tmpDay;

        if (tmpYear <> 2010) and (tmpDay <> 0) and (tmpMonth <> 0) then

          tmpFOMonth := tmpMonth;
          tmpFOYear := tmpYear;

          tmpFODay := tmpDay;
          if tmpDayOfWeek = 5 then
            tmpFODay := tmpFODay + 2;
          elsif tmpDayOfWeek = 6 then
            tmpFODay := tmpFODay + 1;
          elsif tmpDayOfWeek = 7 then
            tmpDayOfWeek := 0;
          end if;

          if tmpFODay >= tmpMaxDay then
            tmpFODay := tmpFODay - tmpMaxDay + 1;
            if tmpFOMonth = 12 then
              tmpFOMonth := 1;
              tmpFOYear := tmpFOYear + 1;
            else
              tmpFOMonth := tmpFOMonth + 1;
            end if;
          else
            tmpFODay := tmpFODay + 1;
          end if;

          tmpFODate := tmpFOYear * 10000 + tmpFOMonth * 100 + tmpFODay;
          tmpDayOfWeek := tmpDayOfWeek + 1;

          insert into d_Date_ConversionFK (ID, RefFKDate, RefFODate)
          values (tmpNewDate, tmpNewDate, tmpFODate);

		  if (tmpDay = 31) and (tmpMonth = 12) then
		  	  tmpFODay := 32;
	          tmpNewDate := tmpYear * 10000 + tmpMonth * 100 + tmpFODay;
	          insert into d_Date_ConversionFK (ID, RefFKDate, RefFODate)
    	      values (tmpNewDate, tmpNewDate, tmpNewDate);
		  end if;

        else

          insert into d_Date_ConversionFK (ID, RefFKDate, RefFODate)
          values (tmpNewDate, tmpNewDate, tmpNewDate);

        end if;

      end loop;

	  end loop;

	end loop;

end sp_ConversionFKDate;
/

begin sp_ConversionFKDate; end;
/

whenever SQLError exit rollback;

commit;

/* End - 3213 - Заключительные обороты в классификаторе Период.День сделать вместо 13 месяца 32 декабря - gbelov - 19.07.2006 */


/* Start - 3208 - 28н - выборка типа территории на этап обработки - mik-a-el - 19.07.2006 */

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PreviewData State="InQueue" Comment="Предварительный просмотр данных для закачки."/>
	<PumpData State="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
	<ProcessData State="InQueue" Comment="Выполняется расщепление сумм фактов по нормативам отчислений доходов и установка типа территории."/>
	<AssociateData State="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
	<ProcessCube State="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
	<CheckData State="InQueue" Comment="Никаких действий не выполняется."/>
</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'FNS28nDataPump';

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PreviewData State="InQueue" Comment="Предварительный просмотр данных для закачки."/>
	<PumpData State="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
	<ProcessData State="InQueue" Comment="Выполняется расщепление сумм фактов по нормативам отчислений доходов и установка типа территории."/>
	<AssociateData State="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
	<ProcessCube State="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
	<CheckData State="InQueue" Comment="Никаких действий не выполняется."/>
</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'Form10Pump';

commit;

/* End   - 3208 - 28н - выборка типа территории на этап обработки - mik-a-el - 19.07.2006 */


/* Start - 3080 - АС Бюджет - версия 33.03 (06.07.03) - mik-a-el - 21.07.2006 */

update PUMPREGISTRY set COMMENTS =
'Поддерживаемые версии БД АС Бюджет: 27.02, 28.00, 29.01, 29.02, 30.01, 31.00, 31.01, 32.02, 32.04, 32.05, 32.07, 32.08, 32.09, 32.10, 33.00, 33.01, 33.02, 33.03'
where PROGRAMIDENTIFIER = 'BudgetDataPump';

commit;

/* End   - 3080 - АС Бюджет - версия 33.03 (06.07.03) - mik-a-el - 21.07.2006 */

create or replace view DataSources (
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory, DataSourceName
) as
select
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory,
	SupplierCode ||' '|| DataName ||' - '|| CASE KindsOfParams WHEN 0 THEN Name || ' ' || Year WHEN 1 THEN cast(Year as varchar(4)) WHEN 2 THEN Year || ' ' || Month  WHEN 3 THEN Year || ' ' || Month || ' ' || Variant WHEN 4 THEN Year || ' ' || Variant WHEN 5 THEN Year || ' ' || Quarter WHEN 6 THEN Year || ' ' || Territory END
from HUB_DataSources;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (16, '2.1.11', SYSDATE, SYSDATE, '');

commit;