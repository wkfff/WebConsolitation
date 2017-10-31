/* Start - 11715 и 11813 - фо 35 - добавлен этап обработки - shahov - 26.10.2009 */

whenever SQLError exit rollback;

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
               <DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
					<ProcessData StageInitialState="InQueue" Comment="Выполняется добавление записей в таблицу фактов."/>
					<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
					<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников, выполняется расчет кубов."/>
			   </DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'FO35Pump';

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (75, '2.6.0.4', To_Date('26.10.2009', 'dd.mm.yyyy'), SYSDATE, 'фо 35 - добавлен этап обработки', 0);

commit;

/* End - 11715 и 11813 - фо 35 - добавлен этап обработки - shahov - 26.10.2009 */
