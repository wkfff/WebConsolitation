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

/* Start - 9445 - фк 1 мес отч и фо 2 мес отч - проверка данных - feanor - 30.10.2008 */

whenever SQLError continue commit;

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

commit;

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

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (55, '2.4.1.7', To_Date('30.10.2008', 'dd.mm.yyyy'), SYSDATE, 'фк 1, фо 2 - добавление этапа проверки данных', 0);

commit;

whenever SQLError exit rollback;

/* End - 9445 - фк 1 мес отч и фо 2 мес отч - проверка данных - feanor - 30.10.2008 */
