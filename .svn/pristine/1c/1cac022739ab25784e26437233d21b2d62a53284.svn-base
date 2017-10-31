/*******************************************************************
 Переводит базу Oracle из версии 2.3.0 в следующую версию 2.3.1
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */


/* Start - 5267 - Аудит по глобальным константам - Paluh - 13.03.2007 */

create or replace trigger t_GlobalConsts_AA before insert OR update OR delete on GlobalConsts for each row
declare
	UserName varchar2(64);
	SessionID varchar2(24);
begin
	SELECT SYS_CONTEXT ('DVContext', 'USERNAME'), SYS_CONTEXT('DVContext', 'SESSIONID') into UserName, SessionID FROM dual;
	if inserting then insert into DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (0, 'GlobalConsts', 5, UserName, SessionID, :new.ID);
	elsif updating then insert into DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (1, 'GlobalConsts', 5, UserName, SessionID, :new.ID);
	elsif deleting then insert into DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (2, 'GlobalConsts', 5, UserName, SessionID, :old.ID); end if;
end;
/

commit;

/* End 5267 - Аудит по глобальным константам - Paluh - 13.03.2007 */



/* Start - 4578 - АДМИН_0003_Проект расходов - gbelov - 20.03.2007 15:23 */

update PUMPREGISTRY
set PROGRAMCONFIG =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="Помещать обработанные файлы в папку архива." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Name="gbVariant" Text="Вариант" LocationX="13" LocationY="0" Width="320" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="edVariant" Text="Основной" Type="Edit" Value="Основной" LocationX="6" LocationY="20" Width="300" Height="20"/>
	</ControlsGroup>
</DataPumpParams>'
where PROGRAMIDENTIFIER = 'GRBSOutcomesProjectPump';

commit;

/* End   - 4578 - АДМИН_0003_Проект расходов - gbelov - 20.03.2007 15:23 */



/* Start   -  - Протокол "Обновление схемы" - gbelov - 20.03.2007 19:19 */

whenever SQLError continue commit;

/* Протоколирование операций пользователя */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (40501, 0, 'Обновление схемы');

/* Виды событий для протокола обновления структуры схемы (50xxx) */
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (50000, 0, 'Сообщение');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (50001, 0, 'Начало обновления схемы');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (50002, 5, 'Обновление успешно закончено');
insert into KindsOfEvents (ID, ClassOfEvent, Name) values (50003, 6, 'Обновление закончено с ошибкой');

/* Обновление схемы */
create table SAT_UpdateScheme
(
	ID					number (10) not null,		/* PK */
/* Далее идут поля специфические для данного вида протогола */
	ObjectFullName		varchar2 (64) not null,		/* Английское наименование объекта. */
	ObjectFullCaption	varchar2 (255),				/* Русское наименование объекта. */
	ModificationType	number (1),					/* Тип модификации:
														0 - создание нового объекта,
														1 - изменение структуры,
														2 - удаление существующего объекта. */
	constraint PKSAT_UpdateScheme primary key ( ID ),
	constraint FKSAT_UpdateScheme foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

/* Представление на протокол "Обновление схемы" для выборки, вставки и удаления записей */
create or replace view UpdateSchemeProtocol
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	ModificationType, ObjectFullName, ObjectFullCaption
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.ModificationType, SAT.ObjectFullName, SAT.ObjectFullCaption
from HUB_EventProtocol HUB join SAT_UpdateScheme SAT on (HUB.ID = SAT.ID);

create or replace trigger t_UpdateSchemeProtocol_i instead of insert on UpdateSchemeProtocol
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
	insert into HUB_EventProtocol (ID, EventDateTime, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, ClassOfProtocol, SessionID)
	values (NewID, NewEventDateTime, :new.Module, :new.KindsOfEvents, :new.InfoMessage, :new.RefUsersOperations, 12, :new.SessionID);

	-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
	insert into SAT_UpdateScheme (ID, ModificationType, ObjectFullName, ObjectFullCaption)
	values (NewID, :new.ModificationType, :new.ObjectFullName, :new.ObjectFullCaption);
end t_UpdateSchemeProtocol_i;
/

create or replace trigger t_UpdateSchemeProtocol_u instead of update on UpdateSchemeProtocol
begin
	raise_application_error(-20001, 'Запись протокола не может быть изменена.');
end t_UpdateSchemeProtocol_u;
/

create or replace trigger t_UpdateSchemeProtocol_d instead of delete on UpdateSchemeProtocol
begin
	delete from HUB_EventProtocol where ID = :old.ID;
end t_UpdateSchemeProtocol_d;
/

commit;

whenever SQLError exit rollback;

/* Start   -  - Протокол "Обновление схемы" - gbelov - 20.03.2007 19:19 */


/* Start - 5267 - Аудит по глобальным константам. - Paluh - 25.03.2007 */

-- Триггер срабатывает после изменений, а не до них, как было раньше
create or replace trigger t_GlobalConsts_AA after insert OR update OR delete on GlobalConsts for each row
declare
	UserName varchar2(64);
	SessionID varchar2(24);
begin
	SELECT SYS_CONTEXT ('DVContext', 'USERNAME'), SYS_CONTEXT('DVContext', 'SESSIONID') into UserName, SessionID FROM dual;
	if inserting then insert into DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (0, 'GlobalConsts', 5, UserName, SessionID, :new.ID);
	elsif updating then insert into DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (1, 'GlobalConsts', 5, UserName, SessionID, :new.ID);
	elsif deleting then insert into DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (2, 'GlobalConsts', 5, UserName, SessionID, :old.ID); end if;
end;
/

commit;

/* End 5267 - Аудит по глобальным константам. - Paluh - 25.03.2007 */


/* Start - 5750 - ФНС_0003_1-НМ - добавляем параметр год и месяц - Feanor - 12.04.2007 */

update PUMPREGISTRY
set PROGRAMCONFIG =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="Помещать обработанные файлы в папку архива." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbDeleteEarlierData" Text="Удалять закачанные ранее данные из того же источника." LocationX="13" LocationY="20" Width="500" Height="20" Value="True"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="Individual">
		<Control Name="lbComment" Type="Label" Text="Параметры используются только при запуске этапа обработки данных." LocationX="13" LocationY="10" Width="600" Height="20" Value=""/>
	</ControlsGroup>
	<ControlsGroup Name="gbYear" Text="Год" LocationX="13" LocationY="40" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="umeYears" Text="" Type="Combo_Years" Value="-1" LocationX="6" LocationY="20" Width="220" Height="20"/>
	</ControlsGroup>
	<ControlsGroup Name="gbMonth" Text="Месяц" LocationX="251" LocationY="40" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="ucMonths" Text="" Type="Combo_Months" Value="-1" LocationX="6" LocationY="19" Width="220" Height="21"/>
	</ControlsGroup>
</DataPumpParams>'
where PROGRAMIDENTIFIER = 'Form1NMPump';

commit;

/* End  - 5750 - ФНС_0003_1-НМ - добавляем параметр год и месяц - Feanor - 12.04.2007 */


/* Start - 5752 - ФО_0002_МесОтч - добавляем параметр год и месяц - Feanor - 12.04.2007 */

update PUMPREGISTRY
set PROGRAMCONFIG =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="Помещать обработанные файлы в папку архива." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbDeleteEarlierData" Text="Удалять закачанные ранее данные из того же источника." LocationX="13" LocationY="20" Width="500" Height="20" Value="True"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="Individual">
		<Control Name="lbComment" Type="Label" Text="Параметры используются только при запуске этапа обработки данных." LocationX="13" LocationY="10" Width="600" Height="20" Value=""/>
	</ControlsGroup>
	<ControlsGroup Name="gbYear" Text="Год" LocationX="13" LocationY="40" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="umeYears" Text="" Type="Combo_Years" Value="-1" LocationX="6" LocationY="20" Width="220" Height="20"/>
	</ControlsGroup>
	<ControlsGroup Name="gbMonth" Text="Месяц" LocationX="251" LocationY="40" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="ucMonths" Text="" Type="Combo_Months" Value="-1" LocationX="6" LocationY="19" Width="220" Height="21"/>
	</ControlsGroup>
</DataPumpParams>'
where PROGRAMIDENTIFIER = 'SKIFMonthRepPump';

commit;

/* End  - 5752 - ФО_0002_МесОтч - добавляем параметр год и месяц - Feanor - 12.04.2007 */



/* Start  - 4121 - Добавлена новая сущность "Таблица данных" - gbelov - 24.04.2007 */

create or replace view MetaLinksWithRolesNames (ID, Semantic, Name, Class, RoleAName, RoleBName, RefPackages, Configuration) as
select
	L.ID, L.Semantic, L.Name, L.Class,
	case OP.Class when 0 then 'b' when 1 then 'd' when 2 then 'fx' when 3 then 'f' when 10 then 't' end || '.' || OP.Semantic || '.' || OP.Name,
	case OC.Class when 0 then 'b' when 1 then 'd' when 2 then 'fx' when 3 then 'f' when 10 then 't' end || '.' || OC.Semantic || '.' || OC.Name,
	L.RefPackages, L.Configuration
from MetaLinks L join MetaObjects OP on (OP.ID = L.RefParent) join MetaObjects OC on (OC.ID = L.RefChild);

/* End  - 4121 - Добавлена новая сущность "Таблица данных" - gbelov - 24.04.2007 */



/* Start - 5459 - Автоматический расчет кубов - tartushkin - 24.04.2007 */

/* Накопитель. Содержит сведения обо всех запросах на расчет объектов многомерной базы */
create table Accumulator
(
	ID					number (10) not null,			/* PK */
	ObjectType			number (10) not null,			/* Тип объекта:
																база = 0,
																куб = 1,
																группа мер = 2,
																раздел = 3,
																измерение = 4*/
	DatabaseId			varchar2 (132) not null,		/* Идентификатор базы. */
	CubeId				varchar2 (132),					/* Идентификатор куба. Пуст, если объект - измерение. */
	MeasureGroupId		varchar2 (132),					/* Идентификатор группы мер. Пуст, если объект - измерение. */
	ObjectId			varchar2 (132) not null,		/* Идентификатор объекта. */
	ObjectName			varchar2 (132) not null,		/* Наименование объекта. */
	CubeName			varchar2 (132),					/* Имя куба. Пусто, если объект - измерение. */
	ProcessType			number (10) not null,			/* Тип рассчета, согласно перечислению Microsoft.AnalysisServices.ProcessType. */
	RefBatch			number (10),					/* Ссылка на пакет */
	State				number (10) not null,			/* Состояние объекта, согласно перечислению Microsoft.AnalysisServices.AnalysisState.  */
	LastProcessed		date,							/* Дата последнего расчета */
	RecordStatus		number (10) not null,			/* Состояние записи:
																ожидание = 0,
																в пакете = 1,
																в процессе расчета = 2,
																расчет завершен успешно = 3,
																расчет завершен с ошибками = 4,
																расчет отменен пользователем = 5,
																расчет отменен оптимизацией = 6. */
	ProcessReason		number (10) not null,			/* Причина расчета:
																обратная запись из листа планирования = 0,
																изменен классификатор = 1,
																изменена ссылка на сопоставимый = 2,
																желание пользователя = 3,
																вызов из закачки = 4. */
	RefUser				number (10) not null,			/* Пользователь, добавивщий задачу в накопитель. */
	AdditionTime		date not null,					/* Время добавления записи в накопитель. */
	ErrorMessage		varchar2 (255),					/* Сообщение об ошибке, если таковая произошла в процессе расчета. */
	OptimizationMember	number (1) default 1 not null	/* 1 - если запись должна обрабатываться оптимизатором, 0 - в противном случае. */
);

create sequence g_Accumulator;

create or replace trigger t_Accumulator_bi before insert on Accumulator for each row
begin
	if :new.ID is null then select g_Accumulator.NextVal into :new.ID from Dual; end if;
end t_Accumulator_bi;
/

/* Пакеты. */
create table Batch
(
	ID					number (10) not null,		/* PK */
	RefUser				number (10) not null,		/* Пользователь, добавивщий пакет. */
	AdditionTime		date not null,				/* Время создания пакета. */
	BatchState			number (10) not null,		/* Состояние пакета:
															редактируется = 0,
															выполняется = 1,
															завершен = 2,
															отменен = 3. */
	SessionId			varchar2 (132)				/* Идентификатор сессии, в которой идет расчет. */
);

create sequence g_Batch;

create or replace trigger t_Batch_bi before insert on Batch for each row
begin
	if :new.ID is null then select g_Batch.NextVal into :new.ID from Dual; end if;
end t_Batch_bi;
/

alter table Batch
	add constraint PKBatch primary key ( ID );
alter table Batch
	add constraint FKBatchRefUser foreign key ( RefUser )
	references Users ( ID );

alter table Accumulator
	add constraint PKAccumulator primary key ( ID );
alter table Accumulator
	add constraint FKAccumulatorRefUser foreign key ( RefUser )
	references Users ( ID );
alter table Accumulator
	add constraint FKAccumulatorRefBatch foreign key ( RefBatch )
	references Batch ( ID );

/* End - 5459 - Автоматический расчет кубов - tartushkin - 24.04.2007 */


/* Start - 5834 - добавляем закачку "Распределение Доходов"  - Feanor - 27.04.2007 */

-- Для предотвращения дублирования делаем проверку на существование закачки в
-- репозитории, т.к. добавление этой записи было выслано в Надым патчем 2.3.0.30.
declare
	cnt pls_integer;
begin
	select count(*) into cnt from PumpRegistry where ProgramIdentifier = 'IncomesDistributionPump';
	if (cnt = 0) then
		insert into PUMPREGISTRY (SUPPLIERCODE, DATACODE, PROGRAMIDENTIFIER, NAME, PROGRAMCONFIG, COMMENTS, STAGESPARAMS, SCHEDULE)
		values ('УФК', 00012, 'IncomesDistributionPump', 'Закачка распределения доходов (ежедневная)',
		'<?xml version="1.0" encoding="windows-1251"?>
		<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
			<ControlsGroup Type="Control" ParamsKind="General">
				<Check Name="ucbMoveFilesToArchive" Text="Помещать обработанные файлы в папку архива." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
			</ControlsGroup>
			<ControlsGroup Type="Control" ParamsKind="Individual">
				<Control Name="lbComment" Type="Label" Text="Параметры используются только при запуске этапа обработки данных." LocationX="13" LocationY="10" Width="600" Height="20" Value=""/>
			</ControlsGroup>
			<ControlsGroup Name="gbYear" Text="Год" LocationX="13" LocationY="40" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
				<Control Name="umeYears" Text="" Type="Combo_Years" Value="-1" LocationX="6" LocationY="20" Width="220" Height="20"/>
			</ControlsGroup>
			<ControlsGroup Name="gbMonth" Text="Месяц" LocationX="251" LocationY="40" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
				<Control Name="ucMonths" Text="" Type="Combo_Months" Value="-1" LocationX="6" LocationY="19" Width="220" Height="21"/>
			</ControlsGroup>
		</DataPumpParams>',
		'Информация по распределению в разрезе ОКАТО, кода дохода, ИМНС, документов и плательщиков.',
		'<?xml version="1.0" encoding="windows-1251"?>
		<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
			<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников."/>
			<ProcessData StageInitialState="InQueue" Comment="Заполняется поле За период таблицы фактов, корректируется уровень бюджета в соответствии с Типом территории."/>
			<AssociateData StageInitialState="Skipped" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
			<ProcessCube StageInitialState="Skipped" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
		</DataPumpStagesParams>',
		'<?xml version="1.0" encoding="windows-1251"?>
		<PumpSchedule xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="PumpSchedule.xsd">
			<ScheduleSettings Enabled="false" StartDate="0" StartTime="0" Periodicity="Once"/>
		</PumpSchedule>');
	end if;
end;
/

commit;

/* End - 5834 - добавляем закачку "Распределение Доходов" - Feanor - 27.04.2007 */



/* Start - 5987 - добавляем возможность регистрации одной закачки с разными параметрами  - Борисов - 04.05.2007 */

-- Добавляем новую колонку для названия файла настроек
alter table PumpRegistry add PumpProgram varchar2 (255);

-- Расширяем Идентификтор Закачки до 255 символов
alter table PumpRegistry modify ProgramIdentifier varchar2 (255);

-- Добавляем ограничение уникальности на Идентификатор Закачки (странно почему его не было раньше)
alter table PumpRegistry add constraint UKPumpRegistry unique (ProgramIdentifier);

/* End - 5987 - добавляем возможность регистрации одной закачки с разными параметрами  - Борисов - 04.05.2007 */



/* Start -  - Представление для поиска объектов в базе данных - gbelov - 07.05.2007 */

create or replace view DVDB_Objects (Name, Type) as
select object_name, object_type from user_objects
union
select constraint_name, constraint_type from user_constraints;

/* End -  - Представление для поиска объектов в базе данных - gbelov - 07.05.2007 */



/* Start - 6016 - Аудит в отдельное табл.пространство - gbelov - 17.05.2007 */

whenever SQLError continue commit;

/* Удаляем данные аудита по закачкам */
delete from DataOperations where UserName like 'Krista.FM.Server.DataPumps%';

commit;

drop user DVAudit cascade;
drop tablespace DVAudit including contents and datafiles cascade constraints;

spool off;

/* -- Создаем табличное пространство DVAudit ------------------------------ */
define UserFile=TempScript.sql

rem Настройки отображения/протоколирования
set echo off;
set Heading off;
set LineSize 250;
set PageSize 2000;
set SQLPrompt "";
set Autoprint off;
set Feedback off;
set Verify off;

host copy &SpoolFile &SpoolFile.2
spool &UserFile;

select 'create tablespace DVAudit datafile ''' || substr(file_name, 1, length(file_name) - 6) || 'DVAudit.dbf'' size 3M reuse autoextend on next 1280K maxsize unlimited extent management local segment space management auto;' from sys.dba_data_files where tablespace_name = 'DV';
prompt

spool off;

set Heading on;
set Feedback on;
set Autoprint on;
set Verify on;
set LineSize 200;
set PageSize 20;
set SQLPrompt "SQL>";
set echo on;

spool &SpoolFile

@TempScript.sql

/* -- Создаем пользователя DVAudit и выделяем ему права ------------------- */
create user DVAudit identified by DVAudit default tablespace DVAudit;
alter user DVAudit quota unlimited on DVAudit;
grant connect to DVAudit;
grant create table to DVAudit;
grant create any index to DVAudit;
grant create trigger to DVAudit;

whenever SQLError continue commit;
	grant create sequence to DVAudit;
whenever SQLError exit rollback;

alter user DV quota unlimited on DVAudit;

whenever SQLError continue commit;
	grant drop sequence to DV;
	drop sequence g_DataOperations;
whenever SQLError exit rollback;

rename DataOperations to DataOperationsOld;
alter table DataOperationsOld drop constraint PKDataOperationsID;

grant ALL PRIVILEGES on DV.DataoperationsOld to PUBLIC;


/* -- Подключаемся под пользователем DVAudit ------------------------------ */
connect DVAudit/DVAudit@&DatabaseName;

/* -- Пересоздаем последовательность в табличном пространстве DVAudit ----- */
rem Настройки отображения/протоколирования
set echo off;
set Heading off;
set LineSize 250;
set PageSize 2000;
set SQLPrompt "";
set Autoprint off;
set Feedback off;
set Verify off;

spool off;
host copy &SpoolFile &SpoolFile.3
spool &UserFile;

define tableName = 'DV.DataOperationsOld';
select 'create sequence DVAudit.g_' || 'DataOperations' || ' start with ' || case (select count(ID) from &tableName) when 0 then 1 else (select max(ID) + 1 from &tableName) end || ';' from Dual;
prompt

spool off;

set Heading on;
set Feedback on;
set Autoprint on;
set Verify on;
set LineSize 200;
set PageSize 20;
set SQLPrompt "SQL>";
set echo on;

spool &SpoolFile

@TempScript.sql

/* Операции по изменению данных */
create table DataOperations
(
	ID number(10) not null ,
	ChangeTime date default SYSDATE not null,
	KindOfOperation number(1) not null,			/* Операция: 0 = insert, 1 = update, 2 = delete */
	ObjectName varchar2 (31) not null,			/* Объект БД */
	ObjectType number (10) default 0 not null,	/* Тип объекта БД. Перечисление Krista.FM.ServerLibrary.DataOperationsObjectTypes */
	UserName varchar2 (64) default USER not null,/* Пользователь */
	SessionID varchar2 (24) not null,			/* ID сессии */
	RecordID number(10) not null,				/* ID записи объекта */
	TaskID number(10),							/* ID задачи (только если с обратной записи; или в интерфейсе таблицы фактов с выбранной задачей) */
	PumpID number(10),							/* ID закачки (заполняется только при закачке) */
	constraint PKDataOperationsID primary key ( ID )
);


create or replace trigger t_DataOperations_bi before insert on DataOperations for each row
begin
	if :new.ID is null then select g_DataOperations.NextVal into :new.ID from Dual; end if;
end t_DataOperations_bi;
/

/* -- Подключаемся под пользователем DV ----------------------------------- */
connect DV/&UserPassword@&DatabaseName;

/* Выделяем права на таблицу DVAudit.Dataoperations */
grant ALL PRIVILEGES on DVAudit.Dataoperations to PUBLIC;

/* Перенос данных аудита в отдельное табличное пространство. Может идти длительное время.*/
insert into DVAudit.DataOperations select * from DataOperationsOld;

commit;

drop table DataOperationsOld;

-- Триггер срабатывает после изменений, а не до них, как было раньше
create or replace trigger t_GlobalConsts_AA after insert OR update OR delete on GlobalConsts for each row
declare
	UserName varchar2(64);
	SessionID varchar2(24);
begin
	SELECT SYS_CONTEXT ('DVContext', 'USERNAME'), SYS_CONTEXT('DVContext', 'SESSIONID') into UserName, SessionID FROM dual;
	if inserting then insert into DVAudit.DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (0, 'GlobalConsts', 5, UserName, SessionID, :new.ID);
	elsif updating then insert into DVAudit.DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (1, 'GlobalConsts', 5, UserName, SessionID, :new.ID);
	elsif deleting then insert into DVAudit.DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID) values (2, 'GlobalConsts', 5, UserName, SessionID, :old.ID); end if;
end;
/

/* End - 6016 - Аудит в отдельное табл.пространство - gbelov - 17.05.2007 */



/* Start -  - Признак указывающий серверу на необходимость произвести обновление схемы - gbelov - 18.05.2007 */

/* Признак указывающий серверу на необходимость произвести обновление схемы
	0 - версия обновлена,
	1 - необходимо обновление
   после обновления сервер должен сбросить признак. */
alter table DatabaseVersions
	add NeedUpdate number (1) default 0 not null;

/* End   -  - Признак указывающий серверу на необходимость произвести обновление схемы - gbelov - 18.05.2007 */



/* Start - 5977 - Новый вид параметра источника - Вариант - gbelov - 24.05.2007 */

create or replace view DataSources (
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory, DataSourceName
) as
select
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory,
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

/* End   - 5977 - Новый вид параметра источника - Вариант - gbelov - 24.05.2007 */


insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) values (21, '2.3.1.0', SYSDATE, SYSDATE, '', 1);

commit;
