/* Start - 11471 - уфк 19 - изменение комментария к этапу обработки - feanor - 11.09.2009 */

whenever SQLError exit rollback;

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
				<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
					<ProcessData StageInitialState="InQueue" Comment="Выполняется установка иерархии и заполнение поля «Код_отчет» в классификаторе «Расходы.УФК»."/>
					<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
					<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников, выполняется расчет кубов."/>
				</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'UFK19Pump';

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (73, '2.6.0.2', To_Date('11.09.2009', 'dd.mm.yyyy'), SYSDATE, 'уфк 19 - изменение комментария к этапу обработки', 0);

commit;

/* End - 11471 - уфк 19 - изменение комментария к этапу обработки - feanor - 11.09.2009 */
