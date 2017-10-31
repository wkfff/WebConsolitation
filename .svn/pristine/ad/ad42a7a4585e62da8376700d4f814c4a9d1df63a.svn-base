/********************************************************************
	Переводит базу SQL Server из версии 2.7 в следующую версию 2.8 
********************************************************************/

/* Start - 14136 - фо 1 - добавлен этап проверки - vpetrov - 28.06.2010 */

whenever SQLError exit rollback;

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
				<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
					<ProcessData StageInitialState="InQueue" Comment="Заполняется классификатор показателей АС Бюджет, формируется иерархия классификаторов."/>
					<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
					<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников, выполняется расчет кубов."/>
					<CheckData StageInitialState="InQueue" Comment="Проверка закачанных сумм."/>
				</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'BudgetDataPump';

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (87, '2.7.0.8', To_Date('28.06.2010', 'dd.mm.yyyy'), SYSDATE, 'фо1 - добавлен этап проверки', 0);

commit;

/* End - 14136 - фо 1 - добавлен этап проверки - vpetrov - 28.06.2010 */
