/*******************************************************************
 Переводит базу Sql Server 2005 из версии 2.3.1 в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */

/* Start - 9445 - фк 1 мес отч и фо 2 мес отч - проверка данных - feanor - 30.10.2008 */

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
				<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
					<ProcessData StageInitialState="InQueue" Comment="Выполняется корректировка сумм фактов по иерархии классификаторов."/>
					<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
					<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников, выполняется расчет кубов."/>
					<CheckData StageInitialState="InQueue" Comment="Проверка скорректированных сумм."/>
				</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'SKIFMonthRepPump';

go

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
				<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
					<ProcessData StageInitialState="InQueue" Comment="Выполняется корректировка сумм фактов по иерархии классификаторов."/>
					<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
					<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников, выполняется расчет кубов."/>
					<CheckData StageInitialState="InQueue" Comment="Проверка скорректированных сумм."/>
				</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'FKMonthRepPump';

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (55, '2.4.1.7', CONVERT(datetime, '2008.10.30', 102), GETDATE(), 'фк 1, фо 2 - добавление этапа проверки данных', 0);

go

/* End - 9445 - фк 1 мес отч и фо 2 мес отч - проверка данных - feanor - 30.10.2008 */


