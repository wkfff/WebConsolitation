/*******************************************************************
 Переводит базу Oracle из версии 2.4.1 в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */

/* Start - 8521 - фнс рф 4 нм - изменение комментария к этапу обработки - feanor - 26.01.2009 */

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
				<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
					<ProcessData StageInitialState="InQueue" Comment="Выполняется установка иерархии в классификаторе «Задолженность.ФНС» и коррекция сумм фактов по данным источника."/>
					<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
					<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников, выполняется расчет кубов."/>
				</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'FNSRF1Pump';

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (63, '2.5.0.7', CONVERT(datetime, '2009.01.26', 102), GETDATE(), 'фнс рф - 4 нм - изменение комментария к этапу обработки', 0);

go

/* End - 8521 - фнс рф 4 нм - изменение комментария к этапу обработки - feanor - 26.01.2009 */
