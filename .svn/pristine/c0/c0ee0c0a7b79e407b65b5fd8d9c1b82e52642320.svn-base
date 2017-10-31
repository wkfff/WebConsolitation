/* Start - 12021 - мофо 16 - добавлен этап обработки - shahov - 12.11.2009 */

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
			   <DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
					<PreviewData StageInitialState="InQueue" Comment="Предварительный просмотр данных для закачки."/>
					<PumpData StageInitialState="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
					<ProcessData StageInitialState="InQueue" Comment="Выполняется проставление ссылок с таблицы фактов на классификатор данных «Фиксированный уровни бюджета»."/>
					<AssociateData StageInitialState="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
					<ProcessCube StageInitialState="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников, выполняется расчет кубов."/>
				</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'MOFO16Pump';

go

insert into DatabaseVersions (ID, Name, Released, Updated, Comments, NeedUpdate) 
values (76, '2.6.0.5', CONVERT(datetime, '2009.11.12', 102), GETDATE(), 'мофо 16 - добавлен этап обработки', 0);

go

/* End - 12021 - мофо 16 - добавлен этап обработки - shahov - 12.11.2009 */
