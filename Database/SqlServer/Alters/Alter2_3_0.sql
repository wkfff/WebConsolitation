/*******************************************************************
 Переводит базу Sql Server 2005 из версии 2.3.0 в следующую версию 2.3.1
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */



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

go

/* End   - 4578 - АДМИН_0003_Проект расходов - gbelov - 20.03.2007 15:23 */



/* Start   -  - Протокол "Обновление схемы" - gbelov - 20.03.2007 19:19 */

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
	ID					int not null,		/* PK */
/* Далее идут поля специфические для данного вида протогола */
	ObjectFullName		varchar (64) not null,		/* Английское наименование объекта. */
	ObjectFullCaption	varchar (255),				/* Русское наименование объекта. */
	ModificationType	tinyint,					/* Тип модификации:
														0 - создание нового объекта,
														1 - изменение структуры,
														2 - удаление существующего объекта. */
	constraint PKSAT_UpdateScheme primary key ( ID ),
	constraint FKSAT_UpdateScheme foreign key ( ID )
		references HUB_EventProtocol ( ID ) on delete cascade
);

go

/* Представление на протокол "Обновление схемы" для выборки, вставки и удаления записей */
create view UpdateSchemeProtocol
(
	ID, EventDateTime, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID,
	ModificationType, ObjectFullName, ObjectFullCaption
) as select
	HUB.ID, HUB.EventDateTime, HUB.Module, HUB.RefKindsOfEvents, HUB.InfoMessage, HUB.RefUsersOperations, HUB.SessionID,
	SAT.ModificationType, SAT.ObjectFullName, SAT.ObjectFullCaption
from HUB_EventProtocol HUB join SAT_UpdateScheme SAT on (HUB.ID = SAT.ID);

go

create trigger t_UpdateSchemeProtocol_i on UpdateSchemeProtocol instead of insert as
begin
	set nocount on;

	declare @nullsCount int;
	select @nullsCount = count(*) from inserted where ID is null;

	if @nullsCount = 0
	begin
		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select ID, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, 12 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_UpdateScheme (ID, ModificationType, ObjectFullName, ObjectFullCaption)
			select ID, ModificationType, ObjectFullName, ObjectFullCaption from inserted;
	end
	else
	if @nullsCount = 1
	begin
		insert into g.HUB_EventProtocol default values;
		delete from g.HUB_EventProtocol where ID = @@IDENTITY;

		-- Вставляем запись с общими атрибутами в основной протокол
		insert into HUB_EventProtocol (ID, Module, RefKindsOfEvents, InfoMessage, RefUsersOperations, SessionID, ClassOfProtocol)
			select @@IDENTITY, Module, KindsOfEvents, InfoMessage, RefUsersOperations, SessionID, 12 from inserted;

		-- Вставляем запись со специфическими атрибутами в конкретный вид протокола
		insert into SAT_UpdateScheme (ID, ModificationType, ObjectFullName, ObjectFullCaption)
			select @@IDENTITY, ModificationType, ObjectFullName, ObjectFullCaption from inserted;
	end else
		RAISERROR (90900, 1, 1);
end;

go

create trigger t_UpdateSchemeProtocol_u on UpdateSchemeProtocol instead of update as
	RAISERROR (90001, 1, 1);
go

create trigger t_UpdateSchemeProtocol_d on UpdateSchemeProtocol instead of delete as
	delete from HUB_EventProtocol where ID in (select ID from deleted);
go

/* Start   -  - Протокол "Обновление схемы" - gbelov - 20.03.2007 19:19 */


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

go

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

go

/* End  - 5752 - ФО_0002_МесОтч - добавляем параметр год и месяц - Feanor - 12.04.2007 */



/* Start  - 4121 - Добавлена новая сущность "Таблица данных" - gbelov - 24.04.2007 */

drop view MetaLinksWithRolesNames;

go

create view MetaLinksWithRolesNames (ID, Semantic, Name, Class, RoleAName, RoleBName, RefPackages, Configuration) as
select
	L.ID, L.Semantic, L.Name, L.Class,
	case OP.Class when 0 then 'b' when 1 then 'd' when 2 then 'fx' when 3 then 'f' when 10 then 't' end + '.' + OP.Semantic + '.' + OP.Name,
	case OC.Class when 0 then 'b' when 1 then 'd' when 2 then 'fx' when 3 then 'f' when 10 then 't' end + '.' + OC.Semantic + '.' + OC.Name,
	L.RefPackages, L.Configuration
from MetaLinks L join MetaObjects OP on (OP.ID = L.RefParent) join MetaObjects OC on (OC.ID = L.RefChild);

go

/* End  - 4121 - Добавлена новая сущность "Таблица данных" - gbelov - 24.04.2007 */


/* Start - 5834 - добавляем закачку "Распределение Доходов"  - Feanor - 27.04.2007 */

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

/* End - 5834 - добавляем закачку "Распределение Доходов" - Feanor - 27.04.2007 */

-- Добавляем новую колонку для названия файла настроек
alter table PumpRegistry add PumpProgram varchar(255);

-- Расширяем Идентификтор Закачки до 255 символов
alter table PumpRegistry alter column ProgramIdentifier varchar(255) not null;

-- Добавляем ограничение уникальности на Идентификатор Закачки (странно почему его не было раньше)
alter table PumpRegistry add constraint UKPumpRegistry unique (ProgramIdentifier);

/* End - 5987 - добавляем возможность регистрации одной закачки с разными параметрами  - Борисов - 04.05.2007 */


/* Start -  - Представление для поиска объектов в базе данных - gbelov - 07.05.2007 */

go

create view DVDB_Objects (Name, Type, Schema_id) as
select name, type, schema_id from sys.objects
union
select I.name, 'INDEX', O.schema_id from sys.indexes I join sys.objects O on (I.object_id = O.object_id and I.type = 2);

go

/* End -  - Представление для поиска объектов в базе данных - gbelov - 07.05.2007 */


/* Start - 5459 - Автоматический расчет кубов - tartushkin - 14.05.2007 */
/* Пакеты. */
create table Batch
(
	ID					int not null,				/* PK */
	RefUser				int not null,				/* Пользователь, добавивщий пакет. */
	AdditionTime		datetime not null,			/* Время создания пакета. */
	BatchState			int not null,				/* Состояние пакета:
															редактируется = 0,
															выполняется = 1,
															завершен = 2,
															отменен = 3. */
	SessionId			varchar (132),				/* Идентификатор сессии, в которой идет расчет. */
	constraint PKBatch primary key ( ID ),
	constraint FKBatchRefUser foreign key ( RefUser )
		references Users ( ID )
);

create table g.Batch ( ID int identity not null );

go

create trigger t_Batch_bi on Batch instead of insert as
begin
	declare @nullsCount int;
	set nocount on;
	select @nullsCount = count(*) from inserted where ID is null;
	if @nullsCount = 0 insert into Batch (ID, RefUser, AdditionTime, BatchState, SessionId) select ID, RefUser, AdditionTime, BatchState, SessionId from inserted;
	else if @nullsCount = 1
	begin
		insert into g.Batch default values;
		delete from g.Batch where ID = @@IDENTITY;
		insert into Batch (ID, RefUser, AdditionTime, BatchState, SessionId) select @@IDENTITY, RefUser, AdditionTime, BatchState, SessionId from inserted;
	end else
		RAISERROR (500001, 1, 1);
end;

go

/* Накопитель. Содержит сведения обо всех запросах на расчет объектов многомерной базы */
create table Accumulator
(
	ID					int not null,				/* PK */
	ObjectType			int not null,				/* Тип объекта:
																база = 0,
																куб = 1,
																группа мер = 2,
																раздел = 3,
																измерение = 4*/
	DatabaseId			varchar (132) not null,		/* Идентификатор базы. */
	CubeId				varchar (132),				/* Идентификатор куба. Пуст, если объект - измерение. */
	MeasureGroupId		varchar (132),				/* Идентификатор группы мер. Пуст, если объект - измерение. */
	ObjectId			varchar (132) not null,		/* Идентификатор объекта. */
	ObjectName			varchar (132) not null,		/* Наименование объекта. */
	CubeName			varchar (132),				/* Имя куба. Пусто, если объект - измерение. */
	ProcessType			int not null,				/* Тип рассчета, согласно перечислению Microsoft.AnalysisServices.ProcessType. */
	RefBatch			int,						/* Ссылка на пакет */
	State				int not null,				/* Состояние объекта, согласно перечислению Microsoft.AnalysisServices.AnalysisState.  */
	LastProcessed		datetime,					/* Дата последнего расчета */
	RecordStatus		int not null,				/* Состояние записи:
																ожидание = 0,
																в пакете = 1,
																в процессе расчета = 2,
																расчет завершен успешно = 3,
																расчет завершен с ошибками = 4,
																расчет отменен пользователем = 5,
																расчет отменен оптимизацией = 6. */
	ProcessReason		int not null,				/* Причина расчета:
																обратная запись из листа планирования = 0,
																изменен классификатор = 1,
																изменена ссылка на сопоставимый = 2,
																желание пользователя = 3,
																вызов из закачки = 4. */
	RefUser				int not null,				/* Пользователь, добавивщий задачу в накопитель. */
	AdditionTime		datetime not null,			/* Время добавления записи в накопитель. */
	ErrorMessage		varchar (255),				/* Сообщение об ошибке, если таковая произошла в процессе расчета. */
	OptimizationMember	tinyint default 1 not null,	/* 1 - если запись должна обрабатываться оптимизатором, 0 - в противном случае. */
	constraint PKAccumulator primary key ( ID ),
	constraint FKAccumulatorRefUser foreign key ( RefUser )
		references Users ( ID ),
	constraint FKAccumulatorRefBatch foreign key ( RefBatch )
		references Batch ( ID )
);

create table g.Accumulator ( ID int identity not null );

go

create trigger t_Accumulator_bi on Accumulator instead of insert as
begin
	declare @nullsCount int;
	set nocount on;
	select @nullsCount = count(*) from inserted where ID is null;
	if @nullsCount = 0 insert into Accumulator (ID, ObjectType, DatabaseId, CubeId, MeasureGroupId, ObjectId, ObjectName, CubeName, ProcessType, RefBatch, State, LastProcessed, RecordStatus, ProcessReason, RefUser, AdditionTime, ErrorMessage, OptimizationMember) select ID, ObjectType, DatabaseId, CubeId, MeasureGroupId, ObjectId, ObjectName, CubeName, ProcessType, RefBatch, State, LastProcessed, RecordStatus, ProcessReason, RefUser, AdditionTime, ErrorMessage, OptimizationMember from inserted;
	else if @nullsCount = 1
	begin
		insert into g.Accumulator default values;
		delete from g.Accumulator where ID = @@IDENTITY;
		insert into Accumulator (ID, ObjectType, DatabaseId, CubeId, MeasureGroupId, ObjectId, ObjectName, CubeName, ProcessType, RefBatch, State, LastProcessed, RecordStatus, ProcessReason, RefUser, AdditionTime, ErrorMessage, OptimizationMember) select @@IDENTITY, ObjectType, DatabaseId, CubeId, MeasureGroupId, ObjectId, ObjectName, CubeName, ProcessType, RefBatch, State, LastProcessed, RecordStatus, ProcessReason, RefUser, AdditionTime, ErrorMessage, OptimizationMember from inserted;
	end else
		RAISERROR (500001, 1, 1);
end;

go

/* End - 5459 - Автоматический расчет кубов - tartushkin - 14.05.2007 */



/* Start -  - Признак указывающий серверу на необходимость произвести обновление схемы - gbelov - 18.05.2007 */

/* Признак указывающий серверу на необходимость произвести обновление схемы
	0 - версия обновлена,
	1 - необходимо обновление
   после обновления сервер должен сбросить признак. */
alter table DatabaseVersions
	add NeedUpdate tinyint default 0 not null;

go

/* End   -  - Признак указывающий серверу на необходимость произвести обновление схемы - gbelov - 18.05.2007 */



/* Start - 5977 - Новый вид параметра источника - Вариант - gbelov - 24.05.2007 */

create view DataSources (
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory, DataSourceName
) as
select
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory,
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

/* End   - 5977 - Новый вид параметра источника - Вариант - gbelov - 24.05.2007 */

go

insert into DatabaseVersions (ID, Name, Released, Updated, [Comments], NeedUpdate) values (21, '2.3.1.0', GETDATE(), GETDATE(), '', 1);

go
