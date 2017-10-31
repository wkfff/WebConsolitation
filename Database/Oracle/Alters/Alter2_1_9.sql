/*******************************************************************
 Переводит базу Oracle из версии 2.1.9 в следующую версию 2.1.10
*******************************************************************/

/* Шаблон оформления альтера: */
/* Start - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */
    /* Ваш SQL-скрипт */
/* End   - <Номер задачи в ClearQuest> - <Суть задачи> - <Фамилия> - <Дата: DD.MM.YYYY> */


/* Start - Стандартная часть */

/* Выходим по любой ошибке */
whenever SQLError exit rollback;
/* End   - Стандартная часть */


/* Start - 2765 - УФК_0002_СводФУ - изменение закачки - mik-a-el - 9.06.2005 */

update PUMPREGISTRY
set PROGRAMCONFIG =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="Помещать обработанные файлы в папку архива." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="Individual">
		<Check Name="ucbFinalOverturn" Text="Закачка заключительных оборотов." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="Individual">
		<Control Name="lbComment" Type="Label" Text="Параметры установки соответствия операционных дней используются только при запуске обработки данных." LocationX="13" LocationY="40" Width="600" Height="20" Value=""/>
	</ControlsGroup>
	<ControlsGroup Name="gbYear" Text="Год" LocationX="13" LocationY="70" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="umeYears" Text="" Type="Combo_Years" Value="-1" LocationX="6" LocationY="20" Width="220" Height="20"/>
	</ControlsGroup>
	<ControlsGroup Name="gbMonth" Text="Месяц" LocationX="251" LocationY="70" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="ucMonths" Text="" Type="Combo_Months" Value="-1" LocationX="6" LocationY="19" Width="220" Height="21"/>
	</ControlsGroup>
</DataPumpParams>'
where PROGRAMIDENTIFIER = 'FUVaultPump';

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PumpData State="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
	<ProcessData State="InQueue" Comment="Выполняется установка соответствия операционных дней по закачанным данным."/>
	<AssociateData State="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
	<ProcessCube State="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
	<CheckData State="InQueue" Comment="Никаких действий не выполняется."/>
</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'FUVaultPump';

commit;

/* End   - 2765 - УФК_0002_СводФУ - изменение закачки - mik-a-el - 9.06.2005 */


/* Start - 2887 - Новый этап закачки - предварительный просмотр - mik-a-el - 21.06.2006 */

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PreviewData State="InQueue" Comment="Предварительный просмотр данных для закачки."/>
	<PumpData State="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
	<ProcessData State="InQueue" Comment="Никаких действий не выполняется."/>
	<AssociateData State="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
	<ProcessCube State="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
	<CheckData State="InQueue" Comment="Никаких действий не выполняется."/>
</DataPumpStagesParams>';

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PreviewData State="InQueue" Comment="Предварительный просмотр данных для закачки."/>
	<PumpData State="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
	<ProcessData State="InQueue" Comment="Выполняется установка соответствия операционных дней по закачанным данным."/>
	<AssociateData State="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
	<ProcessCube State="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
	<CheckData State="InQueue" Comment="Никаких действий не выполняется."/>
</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'FUVaultPump';

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PreviewData State="InQueue" Comment="Никаких действий не выполняется."/>
	<PumpData State="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
	<ProcessData State="InQueue" Comment="Выполняется корректировка сумм фактов по иерархии классификаторов."/>
	<AssociateData State="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
	<ProcessCube State="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
	<CheckData State="InQueue" Comment="Никаких действий не выполняется."/>
</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'SKIFMonthRepPump' or PROGRAMIDENTIFIER = 'SKIFYearRepPump' or PROGRAMIDENTIFIER = 'FKMonthRepPump';

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PreviewData State="InQueue" Comment="Никаких действий не выполняется."/>
	<PumpData State="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
	<ProcessData State="InQueue" Comment="Выполняется расщепление сумм фактов по нормативам отчислений доходов."/>
	<AssociateData State="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
	<ProcessCube State="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
	<CheckData State="InQueue" Comment="Никаких действий не выполняется."/>
</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'FNS28nDataPump' or PROGRAMIDENTIFIER = 'Form10Pump';

update PUMPREGISTRY
set STAGESPARAMS =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpStagesParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpStagesParams.xsd">
	<PreviewData State="InQueue" Comment="Никаких действий не выполняется."/>
	<PumpData State="InQueue" Comment="Выполняется закачка данных из источников, формируется иерархия классификаторов."/>
	<ProcessData State="InQueue" Comment="Никаких действий не выполняется."/>
	<AssociateData State="InQueue" Comment="Выполняется сопоставление данных классификаторов по закачанным источникам."/>
	<ProcessCube State="InQueue" Comment="Формируется иерархия для построения измерений по данным закачанных источников."/>
	<CheckData State="InQueue" Comment="Никаких действий не выполняется."/>
</DataPumpStagesParams>'
where PROGRAMIDENTIFIER = 'LocalBudgetsDataPump' or PROGRAMIDENTIFIER = 'LeasePump' or PROGRAMIDENTIFIER = 'BudgetDataPump';

update PUMPREGISTRY
set PROGRAMCONFIG =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="Помещать обработанные файлы в папку архива." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbDeleteEarlierData" Text="Удалять закачанные ранее данные из того же источника." LocationX="13" LocationY="20" Width="500" Height="20" Value="True"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="Individual">
		<Control Name="lbComment" Type="Label" Text="Параметры расщепления данных используются только при запуске обработки (расщепления) данных." LocationX="13" LocationY="40" Width="600" Height="20" Value=""/>
	</ControlsGroup>
	<ControlsGroup Name="gbDisintegrationMode" Text="Режим расщепления" LocationX="13" LocationY="70" Width="232" Height="70" Type="GroupBox" ParamsKind="Individual">
		<Radio Name="rbtnDisintegratedOnly" Text="Только нерасщепленные" LocationX="13" LocationY="20" Width="400" Height="20" Value="true" FontBold="false"/>
		<Radio Name="rbtnDisintAll" Text="Расщеплять все" LocationX="13" LocationY="40" Width="400" Height="20" Value="false" FontBold="false"/>
	</ControlsGroup>
	<ControlsGroup Name="gbYear" Text="Год" LocationX="251" LocationY="70" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="umeYears" Text="" Type="Combo_Years" Value="-1" LocationX="6" LocationY="20" Width="220" Height="20"/>
	</ControlsGroup>
	<ControlsGroup Name="gbMonth" Text="Месяц" LocationX="489" LocationY="70" Width="232" Height="59" Type="GroupBox" ParamsKind="Individual">
		<Control Name="ucMonths" Text="" Type="Combo_Months" Value="-1" LocationX="6" LocationY="19" Width="220" Height="21"/>
	</ControlsGroup>
</DataPumpParams>'
where PROGRAMIDENTIFIER = 'Form10Pump';

update PUMPREGISTRY
set PROGRAMCONFIG =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="Помещать обработанные файлы в папку архива." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
</DataPumpParams>'
where PROGRAMIDENTIFIER = 'Form13Pump' or PROGRAMIDENTIFIER = 'Form16Pump';

update PUMPREGISTRY
set PROGRAMCONFIG =
'<?xml version="1.0" encoding="windows-1251"?>
<DataPumpParams xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="DataPumpParams.xsd">
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbMoveFilesToArchive" Text="Помещать обработанные файлы в папку архива." LocationX="13" LocationY="0" Width="500" Height="20" Value="False"/>
	</ControlsGroup>
	<ControlsGroup Type="Control" ParamsKind="General">
		<Check Name="ucbDeleteEarlierData" Text="Удалять закачанные ранее данные из того же источника." LocationX="13" LocationY="20" Width="500" Height="20" Value="True"/>
	</ControlsGroup>
</DataPumpParams>'
where PROGRAMIDENTIFIER = 'Form1OBLPump' or PROGRAMIDENTIFIER = 'Form5NIOPump' or PROGRAMIDENTIFIER = 'Form14Pump';

commit;

/* End   - 2887 - Новый этап закачки - предварительный просмотр - mik-a-el - 21.06.2006 */



/* Start   - 2934 - Удаляем блок "Управление схемой" - gbelov - 22.06.2006 */

delete from RegisteredUIModules where Name = 'AdminConsole';

commit;

/* End   - 2934 - Удаляем блок "Управление схемой" - gbelov - 22.06.2006 */


/* Start   -  - Дабавляем забытый параметр "ГодТерритория" - gbelov - 22.06.2006 */

create or replace view datasources
(id, suppliercode, datacode, dataname, kindsofparams, name, year, month, variant, quarter, territory, datasourcename)
as
select
	ID, SupplierCode, DataCode, DataName, KindsOfParams,
	Name, Year, Month, Variant, Quarter, Territory,
	SupplierCode ||' '|| DataName ||' - '|| CASE KindsOfParams WHEN 0 THEN Name || ' ' || Year WHEN 1 THEN cast(Year as varchar(4)) WHEN 2 THEN Year || ' ' || Month  WHEN 3 THEN Year || ' ' || Month || ' ' || Variant WHEN 4 THEN Year || ' ' || Variant WHEN 5 THEN Year || ' ' || Quarter WHEN 6 THEN Year || ' ' || territory END
from HUB_DataSources;

/* End   -  - Дабавляем забытый параметр "ГодТерритория" - gbelov - 22.06.2006 */


insert into DatabaseVersions (ID, Name, Released, Updated, Comments) values (15, '2.1.10', SYSDATE, SYSDATE, '');

commit;
