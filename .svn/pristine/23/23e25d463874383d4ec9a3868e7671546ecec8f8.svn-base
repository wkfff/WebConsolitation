/*******************************************************************
 Переводит базу Sql Server 2005 из версии 2.3.1 в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */

/* Start - 7431 - УФК 11 - отказ от обработки дня ФО, изменение комментария к этапу обработки - feanor - 1.02.2008 */

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
				<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
					<ProcessData StageInitialState="InQueue" Comment="Заполняется поле ''Тип территории'' ОКАТО и корректируется уровень бюджета факта."/>
					<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
					<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников, выполняется расчет кубов."/>
				</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'Form1NApp7DayPump';

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate)
values (36, '2.4.0.2', CONVERT(datetime, '2008.02.01', 102), GETDATE(), 'УФК 11 - отказ от обработки дня ФО, изменение комментария к этапу обработки', 0);

go

/* End - 7431 - УФК 11 - отказ от обработки дня ФО, изменение комментария к этапу обработки - feanor - 1.02.2008 */
