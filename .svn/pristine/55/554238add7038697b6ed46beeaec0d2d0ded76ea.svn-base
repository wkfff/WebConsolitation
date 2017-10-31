/********************************************************************
	Переводит базу SQL Server из версии 2.7 в следующую версию 2.8 
********************************************************************/

/* Start - Start - 15072 - уфк22 - добавлен этап обработки - shahov - 09.11.2010 */

whenever SQLError exit rollback;

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
				<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
					<ProcessData StageInitialState="InQueue" Comment="Выполняется установка ссылок с классификатора «Местные бюджеты.УФК» на классификатор «Фиксированный.Уровни бюджета»."/>
					<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
					<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников, выполняется расчет кубов."/>
				</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'UFK22Pump';

commit;

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (90, '2.7.0.11', To_Date('09.11.2010', 'dd.mm.yyyy'), SYSDATE, 'уфк22 - добавлен этап обработки', 0);

commit;

/* End - 15072 - уфк22 - добавлен этап обработки - shahov - 09.11.2010 */