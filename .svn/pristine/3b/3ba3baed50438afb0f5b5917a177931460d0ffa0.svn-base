/* Start - 11970 - фо28 - неактивность этапов сопоставления и расчета кубов - vpetrov - 12.03.2010 */

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
				<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
					<ProcessData StageInitialState="InQueue" Comment="Выполняется обработка фактов."/>
				</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'FO28Pump';

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (81, '2.7.0.1', CONVERT(datetime, '2010.03.12', 102), GETDATE(), 'фо28 - неактивность этапов сопоставления и расчета кубов', 0);

go

/* End - 11970 - фо28 - неактивность этапов сопоставления и расчета кубов - vpetrov - 12.03.2010 */
