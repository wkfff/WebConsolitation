/*******************************************************************
 Переводит базу Oracle из версии 2.1.11 в следующую версию 2.1.12
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */


/* Start - 3226 - ФНС_0003_1НМ - закачка - mik-a-el - 24.07.2006 */

insert into PUMPREGISTRY (SUPPLIERCODE, DATACODE, PROGRAMIDENTIFIER, NAME, PROGRAMCONFIG, COMMENTS, STAGESPARAMS, SCHEDULE)
values ('ФНС', 0003, 'Form1NMPump', 'Закачка отчетов формы 1НМ',
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="Помещать обработанные файлы в папку архива." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
</DataPumpParams>',
'Отчет о зачислении налогов, сборов и иных обязательных платежей в бюджетную систему РФ, Приказ Федеральной налоговой службы от 19 ноября 2004 г. N САЭ-3-10/108@  "Об утверждении форм статистической налоговой отчетности Федеральной налоговой службы на 2005 год".',
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PreviewData State="InQueue" Comment="Предварительный просмотр данных для закачки."/>
	<PumpData State="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
	<ProcessData State="InQueue" Comment="Никаких действий не выполняется."/>
	<AssociateData State="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
	<ProcessCube State="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
	<CheckData State="InQueue" Comment="Никаких действий не выполняется."/>
</DataPumpStagesParams>',
'<?xml version="1.0" encoding="windows-1251"?>
<PumpSchedule xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="PumpSchedule.xsd">
	<ScheduleSettings Enabled="false" StartDate="0" StartTime="0" Periodicity="Once"/>
</PumpSchedule>');

commit;

/* End   - 3226 - ФНС_0003_1НМ - закачка - mik-a-el - 24.07.2006 */

/* Start - 3296 - УФК_0008_1Н - mik-a-el - 28.07.2006 */

insert into PUMPREGISTRY (SUPPLIERCODE, DATACODE, PROGRAMIDENTIFIER, NAME, PROGRAMCONFIG, COMMENTS, STAGESPARAMS, SCHEDULE)
values ('УФК', 0007, 'Form1NApp7Pump', 'Закачка отчетов формы 1Н, приложение 7',
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="Помещать обработанные файлы в папку архива." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
</DataPumpParams>',
'Приказ Федерального казначейства от 22 марта 2005 г. № 1Н "Об утверждении порядка кассового обслуживания исполнения бюджетов субъектов Российской Федерации и местных бюджетов территориальными органами Федерального казначейства". Сводная ведомость по кассовым поступлениям (ежемесячная) по форме согласно приложению № 7 к 1Н',
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PreviewData State="InQueue" Comment="Предварительный просмотр данных для закачки."/>
	<PumpData State="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
	<ProcessData State="InQueue" Comment="Никаких действий не выполняется."/>
	<AssociateData State="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
	<ProcessCube State="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
	<CheckData State="InQueue" Comment="Никаких действий не выполняется."/>
</DataPumpStagesParams>',
'<?xml version="1.0" encoding="windows-1251"?>
<PumpSchedule xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="PumpSchedule.xsd">
	<ScheduleSettings Enabled="false" StartDate="0" StartTime="0" Periodicity="Once"/>
</PumpSchedule>');

insert into PUMPREGISTRY (SUPPLIERCODE, DATACODE, PROGRAMIDENTIFIER, NAME, PROGRAMCONFIG, COMMENTS, STAGESPARAMS, SCHEDULE)
values ('УФК', 0008, 'Form1NDPPump', 'Закачка отчетов формы 1Н, Реестр перечисленных поступлений',
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="Помещать обработанные файлы в папку архива." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
</DataPumpParams>',
'Приказ Федерального казначейства от 22 марта 2005 г. № 1Н "Об утверждении порядка кассового обслуживания исполнения бюджетов субъектов Российской Федерации и местных бюджетов территориальными органами Федерального казначейства". Реестр перечисленных поступлений',
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PreviewData State="InQueue" Comment="Предварительный просмотр данных для закачки."/>
	<PumpData State="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
	<ProcessData State="InQueue" Comment="Никаких действий не выполняется."/>
	<AssociateData State="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
	<ProcessCube State="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
	<CheckData State="InQueue" Comment="Никаких действий не выполняется."/>
</DataPumpStagesParams>',
'<?xml version="1.0" encoding="windows-1251"?>
<PumpSchedule xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="PumpSchedule.xsd">
	<ScheduleSettings Enabled="false" StartDate="0" StartTime="0" Periodicity="Once"/>
</PumpSchedule>');

commit;

/* End   - 3296 - УФК_0008_1Н - mik-a-el - 28.07.2006 */



/* Start -  - Изменение структуры метаданных пакетов - gbelov - 29.07.2006 */

alter table MetaPackages
	add PrivatePath varchar2 (1000);

update MetaPackages set PrivatePath = 'Packages\ФО_0001_АС Бюджет\ФО_0001_АС Бюджет.xml' where RefParent is null and BuiltIn = 0 and Name = 'ФО_0001_АС Бюджет';
update MetaPackages set PrivatePath = 'Packages\' || Name || '.xml' where PrivatePath is null and RefParent is null and BuiltIn = 0;
update MetaPackages set PrivatePath = Name || '.xml' where RefParent is not null and BuiltIn = 0;

commit;

/* End -  - Изменение структуры метаданных пакетов - gbelov - 29.07.2006 */

insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (17, '2.1.12', SYSDATE, SYSDATE, '');

commit;
