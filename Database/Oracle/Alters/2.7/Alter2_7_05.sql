/********************************************************************
	Переводит базу SQL Server из версии 2.7 в следующую версию 2.8 
********************************************************************/

/* Start - 13213 - уфк20 - добавлен этап обработки - shahov - 09.04.2010 */

whenever SQLError exit rollback;

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
				<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
					<ProcessData StageInitialState="InQueue" Comment="Выполняется установка ссылок на уровни бюджета в классификаторе «Местные бюджеты.УФК»"/>
					<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
					<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников, выполняется расчет кубов."/>
					<CheckData StageInitialState="InQueue" Comment="Выполняется сверка сумм по кассовым поступлениям с суммами, подлежащих к перечислению."/>
				</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'UFK20Pump';

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (85, '2.7.0.5', To_Date('09.04.2010', 'dd.mm.yyyy'), SYSDATE, 'уфк20 - добавлен этап обработки', 0);

commit;

/* End - 13213 - уфк20 - добавлен этап обработки - shahov - 09.04.2010 */
