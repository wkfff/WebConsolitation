


/* Start - 14136 - фо 1 - добавлен этап проверки - vpetrov - 28.06.2010 */

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

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (87, '2.7.0.8', CONVERT(datetime, '2010.06.28', 102), GETDATE(), 'фо1 - добавлен этап проверки', 0);

go

/* End - 14136 - фо 1 - добавлен этап проверки - vpetrov - 28.06.2010 */
