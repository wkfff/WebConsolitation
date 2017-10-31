/*******************************************************************
 Переводит базу Oracle из версии 2.5 в следующую версию 2.X.X
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */

/* Start - Стандартная часть */
/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */

/* Start - 4661 - фнс1 - добавление этапа проверки данных - shahov - 03.06.2009 */

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
				<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
					<ProcessData StageInitialState="InQueue" Comment="Выполняется расщепление сумм фактов по нормативам отчислений доходов и установка типа территории, формируется иерархия классификаторов."/>
					<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
					<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников, выполняется расчет кубов."/>
					<CheckData StageInitialState="InQueue" Comment="Выполняется сравнение перечней КД, ОКАТО, ОКВЭД по закачиваемому источнику с кодами по предыдущему источнику."/>
				</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'FNS28nDataPump';

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (69, '2.5.0.13', To_Date('03.06.2009', 'dd.mm.yyyy'), SYSDATE, 'фнс1 - добавление этапа проверки данных', 0);

commit;

/* End - 4661 - фнс1 - добавление этапа проверки данных - shahov - 03.06.2009 */
