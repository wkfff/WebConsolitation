/* Start - 11970 - фо28 - неактивность этапов сопоставления и расчета кубов - vpetrov - 12.03.2010 */

whenever SQLError exit rollback;

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
				<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
					<ProcessData StageInitialState="InQueue" Comment="Выполняется обработка фактов."/>
				</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'FO28Pump';

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (81, '2.7.0.1', To_Date('12.03.2010', 'dd.mm.yyyy'), SYSDATE, 'фо28 - неактивность этапов сопоставления и расчета кубов', 0);

commit;

/* End - 11970 - фо28 - неактивность этапов сопоставления и расчета кубов - vpetrov - 12.03.2010 */
