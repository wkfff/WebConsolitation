/********************************************************************
	Переводит базу SQL Server из версии 2.7 в следующую версию 2.8 
********************************************************************/

/* Start - 13711 - эо5 - добавлен этап сопоставления - shahov - 20.05.2010 */

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
				<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
					<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
					<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников, выполняется расчет кубов."/>
				</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'EO5Pump';

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (86, '2.7.0.7', CONVERT(datetime, '2010.05.20', 102), GETDATE(), 'эо5 - добавлен этап сопоставление', 0);

go

/* End - 13711 - эо5 - добавлен этап сопоставления - shahov - 20.05.2010 */
